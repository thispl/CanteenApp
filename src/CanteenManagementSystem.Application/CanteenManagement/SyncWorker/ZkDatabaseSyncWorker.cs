using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using CanteenManagementSystem.EntityFrameworkCore.ZkTecoIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Threading;

namespace CanteenManagementSystem.CanteenManagement.SyncWorker;

/// <summary>
/// Background worker that synchronizes data from ZKTeco biometric device database.
/// Runs every 15 seconds to fetch new punch logs and sync them to local database.
/// </summary>
public class ZkDatabaseSyncWorker : AsyncPeriodicBackgroundWorkerBase
{
    public const string SyncStateKey = "Last_Zk_Transaction_Id";
    public const int SyncIntervalMilliseconds = 15000; // 15 seconds

    public ZkDatabaseSyncWorker(
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
            var checkInRepository = workerContext.ServiceProvider.GetRequiredService<ICanteenCheckInRepository>();
            var zkTecoDbContext = workerContext.ServiceProvider.GetRequiredService<ZkTecoDbContext>();
            var guidGenerator = workerContext.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var logger = workerContext.ServiceProvider.GetRequiredService<ILogger<ZkDatabaseSyncWorker>>();

            logger.LogDebug("Starting ZKTeco database sync cycle...");

            // Step 1: Fetch Progress State
            var syncState = await syncStateRepository.GetOrCreateAsync(SyncStateKey, 0, cancellationToken);
            var lastProcessedId = syncState.LastProcessedValue;

            logger.LogDebug("Last processed transaction ID: {LastProcessedId}", lastProcessedId);

            // Step 2: Query New Punches from external database
            var newTransactions = await zkTecoDbContext.IclockTransactions
                .AsNoTracking()
                .Where(t => t.Id > lastProcessedId)
                .OrderBy(t => t.Id)
                .ToListAsync(cancellationToken);

            if (newTransactions.Count == 0)
            {
                logger.LogDebug("No new transactions found.");
                return;
            }

            logger.LogInformation("Found {Count} new transactions to process.", newTransactions.Count);

            int maxProcessedId = lastProcessedId;
            int processedCount = 0;
            int skippedCount = 0;

            // Step 3: Process Rows Line-by-Line
            foreach (var transaction in newTransactions)
            {
                try
                {
                    // Skip if required fields are null
                    if (string.IsNullOrEmpty(transaction.Pin))
                    {
                        logger.LogWarning("Skipping transaction {TransactionId} - Pin (emp_code) is null", transaction.Id);
                        continue;
                    }

                    // Check Employee: Search local AppEmployees table
                    var employee = await employeeRepository.FindByEmployeeIdAsync(transaction.Pin, cancellationToken);

                    if (employee == null)
                    {
                        // Employee does not exist locally, query external Personnel table
                        var externalEmployee = await zkTecoDbContext.PersonnelEmployees
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.EnrollNumber == transaction.Pin, cancellationToken);

                        if (externalEmployee != null && !string.IsNullOrEmpty(externalEmployee.EnrollNumber))
                        {
                            // Insert new employee from external source
                            var empId = externalEmployee.EnrollNumber;
                            var empName = externalEmployee.Name ?? $"Employee {empId}";
                            
                            employee = new Employee(
                                guidGenerator.Create(),
                                empId,
                                empName,
                                null); // Department not available in external table

                            await employeeRepository.InsertAsync(employee, autoSave: true, cancellationToken);
                            logger.LogInformation("Created new employee: {EmployeeId} - {FullName}",
                                employee.EmployeeId, employee.FullName);
                        }
                        else
                        {
                            // Employee not found in external table either, create with unknown name
                            employee = new Employee(
                                guidGenerator.Create(),
                                transaction.Pin,
                                $"Unknown ({transaction.Pin})",
                                null);

                            await employeeRepository.InsertAsync(employee, autoSave: true, cancellationToken);
                            logger.LogWarning("Created employee with unknown name: {EmployeeId}",
                                employee.EmployeeId);
                        }
                    }

                    // Prevent Duplicate Logs: Check if this exact punch already exists
                    var deviceId = transaction.AuthDeviceId ?? "Unknown";
                    var punchTime = transaction.AuthTime ?? DateTime.Now;
                    
                    var exists = await checkInRepository.ExistsAsync(
                        transaction.Pin,
                        deviceId,
                        punchTime,
                        cancellationToken);

                    if (exists)
                    {
                        logger.LogDebug("Skipping duplicate check-in for employee {EmployeeId} at {CheckInTime}",
                            transaction.Pin, transaction.AuthTime);
                        skippedCount++;
                    }
                    else
                    {
                        // Save Log: Insert new check-in
                        var checkIn = new CanteenCheckIn(
                            guidGenerator.Create(),
                            transaction.Pin,
                            deviceId,
                            punchTime);

                        await checkInRepository.InsertAsync(checkIn, false, cancellationToken);
                        processedCount++;

                        logger.LogDebug("Saved check-in: Employee {EmployeeId}, Device {DeviceId}, Time {CheckInTime}",
                            checkIn.EmployeeId, checkIn.DeviceId, checkIn.CheckInTime);
                    }

                    // Track the highest ID processed
                    if (transaction.Id > maxProcessedId)
                    {
                        maxProcessedId = transaction.Id;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing transaction ID {TransactionId}", transaction.Id);
                    // Continue with next transaction
                }
            }

            // Step 4: Commit State Progress
            if (maxProcessedId > lastProcessedId)
            {
                syncState.UpdateLastProcessedValue(maxProcessedId);
                await syncStateRepository.UpdateAsync(syncState, autoSave: true, cancellationToken);

                logger.LogInformation(
                    "Sync cycle completed. Processed: {ProcessedCount}, Skipped: {SkippedCount}, New Max ID: {MaxId}",
                    processedCount, skippedCount, maxProcessedId);
            }
            else
            {
                logger.LogDebug("No new transactions were successfully processed in this cycle.");
            }
        }
        catch (Exception ex)
        {
            var logger = workerContext.ServiceProvider.GetService<ILogger<ZkDatabaseSyncWorker>>();
            logger?.LogError(ex, "Error during ZKTeco sync cycle.");
        }
    }
}
