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
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Threading;

namespace CanteenManagementSystem.CanteenManagement.SyncWorker;

/// <summary>
/// Background worker that synchronizes employee data from ZKTeco personnel_employee table.
/// Runs every hour to fetch all employees and sync them to local database.
/// </summary>
public class EmployeeSyncWorker : AsyncPeriodicBackgroundWorkerBase
{
    public const int SyncIntervalMilliseconds = 3600000; // 1 hour = 60 * 60 * 1000 ms

    public EmployeeSyncWorker(
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
            var employeeRepository = workerContext.ServiceProvider.GetRequiredService<IEmployeeRepository>();
            var zkTecoDbContext = workerContext.ServiceProvider.GetRequiredService<ZkTecoDbContext>();
            var guidGenerator = workerContext.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var logger = workerContext.ServiceProvider.GetRequiredService<ILogger<EmployeeSyncWorker>>();

            logger.LogInformation("Starting Employee Sync from personnel_employee table...");

            // Step 1: Query all employees from external personnel_employee table
            var externalEmployees = await zkTecoDbContext.PersonnelEmployees
                .AsNoTracking()
                .Where(e => !string.IsNullOrEmpty(e.EnrollNumber))
                .ToListAsync(cancellationToken);

            if (externalEmployees.Count == 0)
            {
                logger.LogInformation("No employees found in personnel_employee table.");
                return;
            }

            logger.LogInformation("Found {Count} employees in personnel_employee table to sync.", externalEmployees.Count);

            int createdCount = 0;
            int updatedCount = 0;
            int unchangedCount = 0;

            // Step 2: Sync each employee
            foreach (var externalEmp in externalEmployees)
            {
                try
                {
                    var empId = externalEmp.EnrollNumber!;
                    var empName = externalEmp.Name ?? $"Employee {empId}";

                    // Check if employee exists locally
                    var existingEmployee = await employeeRepository.FindByEmployeeIdAsync(empId, cancellationToken);

                    if (existingEmployee == null)
                    {
                        // Create new employee
                        var newEmployee = new Employee(
                            guidGenerator.Create(),
                            empId,
                            empName,
                            null); // Department not available in external table

                        await employeeRepository.InsertAsync(newEmployee, autoSave: true, cancellationToken);
                        createdCount++;

                        logger.LogDebug("Created new employee: {EmployeeId} - {FullName}",
                            newEmployee.EmployeeId, newEmployee.FullName);
                    }
                    else if (existingEmployee.FullName != empName)
                    {
                        // Update employee name if changed
                        var oldName = existingEmployee.FullName;
                        existingEmployee.SetFullName(empName);
                        await employeeRepository.UpdateAsync(existingEmployee, autoSave: true, cancellationToken);
                        updatedCount++;

                        logger.LogInformation("Updated employee name: {EmployeeId} - '{OldName}' -> '{NewName}'",
                            existingEmployee.EmployeeId, oldName, empName);
                    }
                    else
                    {
                        unchangedCount++;
                        logger.LogDebug("Employee unchanged: {EmployeeId} - {FullName}",
                            existingEmployee.EmployeeId, existingEmployee.FullName);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error syncing employee {EnrollNumber}", externalEmp.EnrollNumber);
                    // Continue with next employee
                }
            }

            logger.LogInformation(
                "Employee sync completed. Created: {CreatedCount}, Updated: {UpdatedCount}, Unchanged: {UnchangedCount}, Total: {TotalCount}",
                createdCount, updatedCount, unchangedCount, externalEmployees.Count);
        }
        catch (Exception ex)
        {
            var logger = workerContext.ServiceProvider.GetService<ILogger<EmployeeSyncWorker>>();
            logger?.LogError(ex, "Error during Employee Sync cycle.");
        }
    }
}
