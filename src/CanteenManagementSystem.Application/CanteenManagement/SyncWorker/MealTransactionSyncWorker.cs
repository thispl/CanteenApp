using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using CanteenManagementSystem.EntityFrameworkCore.ZkTecoIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Threading;

namespace CanteenManagementSystem.CanteenManagement.SyncWorker;

/// <summary>
/// Background worker that reads ZKTeco punches and creates MealTransaction records.
/// Default interval is 5 minutes; confirm with the team before finalizing.
/// </summary>
public class MealTransactionSyncWorker : AsyncPeriodicBackgroundWorkerBase
{
    public const string SyncStateKey = "Last_Zk_Meal_Transaction_Id";
    public const int SyncIntervalMilliseconds = 300000; // 5 minutes

    public MealTransactionSyncWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory) :
        base(timer, serviceScopeFactory)
    {
        Timer.Period = SyncIntervalMilliseconds;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var cancellationToken = workerContext.CancellationToken;

        try
        {
            var syncStateRepository = workerContext.ServiceProvider.GetRequiredService<ISyncStateRepository>();
            var employeeRepository = workerContext.ServiceProvider.GetRequiredService<IEmployeeRepository>();
            var deviceRepository = workerContext.ServiceProvider.GetRequiredService<IDeviceRepository>();
            var timeScheduleRepository = workerContext.ServiceProvider.GetRequiredService<ITimeScheduleRepository>();
            var mealTransactionRepository = workerContext.ServiceProvider.GetRequiredService<IMealTransactionRepository>();
            var zkTecoDbContext = workerContext.ServiceProvider.GetRequiredService<ZkTecoDbContext>();
            var guidGenerator = workerContext.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var logger = workerContext.ServiceProvider.GetRequiredService<ILogger<MealTransactionSyncWorker>>();

            logger.LogDebug("Starting MealTransaction sync cycle...");

            var syncState = await syncStateRepository.GetOrCreateAsync(SyncStateKey, 0, cancellationToken);
            var lastProcessedId = syncState.LastProcessedValue;

            logger.LogDebug("Last processed meal transaction ID: {LastProcessedId}", lastProcessedId);

            var newTransactions = await zkTecoDbContext.IclockTransactions
                .AsNoTracking()
                .Where(t => t.Id > lastProcessedId)
                .OrderBy(t => t.Id)
                .ToListAsync(cancellationToken);

            if (newTransactions.Count == 0)
            {
                logger.LogDebug("No new punches found for meal transaction sync.");
                return;
            }

            logger.LogInformation("Found {Count} new punches to process for meal transactions.", newTransactions.Count);

            var timeSchedules = await timeScheduleRepository.GetListAsync(includeDetails: true, cancellationToken: cancellationToken);
            var schedulesWithItems = timeSchedules.Where(t => t.ItemId.HasValue).ToList();

            int maxProcessedId = lastProcessedId;
            int createdCount = 0;
            int skippedCount = 0;
            int noEmployeeCount = 0;
            int noScheduleCount = 0;

            foreach (var transaction in newTransactions)
            {
                try
                {
                    if (string.IsNullOrEmpty(transaction.Pin))
                    {
                        logger.LogWarning("Skipping meal transaction sync for punch {PunchId} - Pin (emp_code) is null", transaction.Id);
                        continue;
                    }

                    if (!transaction.AuthTime.HasValue)
                    {
                        logger.LogWarning("Skipping meal transaction sync for punch {PunchId} - Punch time is null", transaction.Id);
                        continue;
                    }

                    var employee = await employeeRepository.FindByEmployeeIdAsync(transaction.Pin, cancellationToken);
                    if (employee == null)
                    {
                        logger.LogWarning("No local employee found for punch {PunchId} with Pin {Pin}. Creating placeholder employee.", transaction.Id, transaction.Pin);
                        employee = new Employee(guidGenerator.Create(), transaction.Pin, $"Unknown ({transaction.Pin})");
                        await employeeRepository.InsertAsync(employee, autoSave: true, cancellationToken);
                        noEmployeeCount++;
                    }

                    var punchTime = transaction.AuthTime.Value;
                    var timeSchedule = FindMatchingTimeSchedule(punchTime, schedulesWithItems, transaction.Id, logger);
                    if (timeSchedule == null)
                    {
                        noScheduleCount++;
                        continue;
                    }

                    var itemId = timeSchedule.ItemId!.Value;
                    var item = timeSchedule.Item ?? await workerContext.ServiceProvider
                        .GetRequiredService<IItemRepository>()
                        .GetAsync(itemId, cancellationToken: cancellationToken);

                    var exists = await mealTransactionRepository.ExistsAsync(employee.Id, punchTime, cancellationToken);
                    if (exists)
                    {
                        logger.LogDebug(
                            "Skipping duplicate meal transaction for employee {EmployeeId} at {PunchTime}",
                            employee.EmployeeId, punchTime);
                        skippedCount++;
                    }
                    else
                    {
                        var device = await ResolveDeviceAsync(transaction.AuthDeviceId, deviceRepository, guidGenerator, cancellationToken);

                        var mealTransaction = new MealTransaction(
                            guidGenerator.Create(),
                            employee.Id,
                            device.Id,
                            timeSchedule.Id,
                            item.Id,
                            item.Price,
                            punchTime,
                            MealTransactionSource.AutoSync);

                        await mealTransactionRepository.InsertAsync(mealTransaction, false, cancellationToken);
                        createdCount++;

                        logger.LogDebug(
                            "Created meal transaction: Employee {EmployeeId}, Schedule {ScheduleName}, Item {ItemName}, Price {Price}, Time {PunchTime}",
                            employee.EmployeeId, timeSchedule.Name, item.Description, item.Price, punchTime);
                    }

                    if (transaction.Id > maxProcessedId)
                    {
                        maxProcessedId = transaction.Id;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing meal transaction for punch ID {PunchId}", transaction.Id);
                }
            }

            if (maxProcessedId > lastProcessedId)
            {
                syncState.UpdateLastProcessedValue(maxProcessedId);
                await syncStateRepository.UpdateAsync(syncState, autoSave: true, cancellationToken);

                logger.LogInformation(
                    "Meal transaction sync completed. Created: {CreatedCount}, Skipped: {SkippedCount}, No Employee: {NoEmployeeCount}, No Schedule: {NoScheduleCount}, New Max ID: {MaxId}",
                    createdCount, skippedCount, noEmployeeCount, noScheduleCount, maxProcessedId);
            }
            else
            {
                logger.LogDebug("No new punches were processed in meal transaction sync cycle.");
            }
        }
        catch (Exception ex)
        {
            var logger = workerContext.ServiceProvider.GetService<ILogger<MealTransactionSyncWorker>>();
            logger?.LogError(ex, "Error during MealTransaction sync cycle.");
        }
    }

    private TimeSchedule? FindMatchingTimeSchedule(
        DateTime punchTime,
        List<TimeSchedule> schedules,
        int punchId,
        ILogger<MealTransactionSyncWorker> logger)
    {
        var timeOfDay = TimeOnly.FromDateTime(punchTime);
        var matches = schedules
            .Where(s => IsTimeInSchedule(timeOfDay, s.StartTime, s.EndTime))
            .ToList();

        if (matches.Count == 0)
        {
            logger.LogWarning(
                "Punch {PunchId} at {PunchTime} does not fall into any known time schedule.",
                punchId, punchTime);
            return null;
        }

        if (matches.Count > 1)
        {
            logger.LogWarning(
                "Punch {PunchId} at {PunchTime} matched multiple schedules ({ScheduleNames}). Using {SelectedSchedule}.",
                punchId, punchTime, string.Join(", ", matches.Select(m => m.Name)), matches[0].Name);
        }

        return matches[0];
    }

    private bool IsTimeInSchedule(TimeOnly time, TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime >= startTime)
        {
            return time >= startTime && time <= endTime;
        }

        // Spans midnight, e.g. 21:50 - 03:00
        return time >= startTime || time <= endTime;
    }

    private async Task<Device> ResolveDeviceAsync(
        string? authDeviceId,
        IDeviceRepository deviceRepository,
        IGuidGenerator guidGenerator,
        CancellationToken cancellationToken)
    {
        var deviceId = authDeviceId ?? "Unknown";
        var queryable = await deviceRepository.GetQueryableAsync();
        var device = await queryable.FirstOrDefaultAsync(d => d.DeviceId == deviceId, cancellationToken);

        if (device != null)
        {
            return device;
        }

        device = new Device(
            guidGenerator.Create(),
            deviceId,
            $"Auto-created ({deviceId})",
            status: DeviceStatus.Active);

        await deviceRepository.InsertAsync(device, autoSave: true, cancellationToken);
        return device;
    }
}
