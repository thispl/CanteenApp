using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class CanteenManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly CompanyDataSeedContributor _companySeed;
    private readonly DesignationDataSeedContributor _designationSeed;
    private readonly CategoryDataSeedContributor _categorySeed;
    private readonly DepartmentDataSeedContributor _departmentSeed;
    private readonly ItemDataSeedContributor _itemSeed;
    private readonly DeviceDataSeedContributor _deviceSeed;
    private readonly TimeScheduleDataSeedContributor _timeScheduleSeed;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDesignationRepository _designationRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ITimeScheduleRepository _timeScheduleRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICanteenCheckInRepository _checkInRepository;
    private readonly IMealTransactionRepository _mealTransactionRepository;
    private readonly ICashDepositRepository _cashDepositRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<CanteenManagementDataSeedContributor> _logger;

    public CanteenManagementDataSeedContributor(
        CompanyDataSeedContributor companySeed,
        DesignationDataSeedContributor designationSeed,
        CategoryDataSeedContributor categorySeed,
        DepartmentDataSeedContributor departmentSeed,
        ItemDataSeedContributor itemSeed,
        DeviceDataSeedContributor deviceSeed,
        TimeScheduleDataSeedContributor timeScheduleSeed,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        ICategoryRepository categoryRepository,
        IDesignationRepository designationRepository,
        IDeviceRepository deviceRepository,
        ITimeScheduleRepository timeScheduleRepository,
        IItemRepository itemRepository,
        ICanteenCheckInRepository checkInRepository,
        IMealTransactionRepository mealTransactionRepository,
        ICashDepositRepository cashDepositRepository,
        IGuidGenerator guidGenerator,
        ILogger<CanteenManagementDataSeedContributor> logger)
    {
        _companySeed = companySeed;
        _designationSeed = designationSeed;
        _categorySeed = categorySeed;
        _departmentSeed = departmentSeed;
        _itemSeed = itemSeed;
        _deviceSeed = deviceSeed;
        _timeScheduleSeed = timeScheduleSeed;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _categoryRepository = categoryRepository;
        _designationRepository = designationRepository;
        _deviceRepository = deviceRepository;
        _timeScheduleRepository = timeScheduleRepository;
        _itemRepository = itemRepository;
        _checkInRepository = checkInRepository;
        _mealTransactionRepository = mealTransactionRepository;
        _cashDepositRepository = cashDepositRepository;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await _companySeed.SeedAsync(context);
        await _designationSeed.SeedAsync(context);
        await _categorySeed.SeedAsync(context);
        await _departmentSeed.SeedAsync(context);
        await _itemSeed.SeedAsync(context);
        await _deviceSeed.SeedAsync(context);
        await _timeScheduleSeed.SeedAsync(context);
        await SeedDemoDataAsync();
    }

    private async Task SeedDemoDataAsync()
    {
        var designDepartment = await FindDepartmentAsync("Design");
        var financeDepartment = await FindDepartmentAsync("Fin & Acc");
        var ictDepartment = await FindDepartmentAsync("ICT");
        var staffCategory = await FindCategoryAsync("Staff");
        var traineeCategory = await FindCategoryAsync("Trainee");
        var engineerDesignation = await _designationRepository.FindByCodeAsync("ENG");
        var executiveDesignation = await _designationRepository.FindByCodeAsync("EXE");
        var traineeDesignation = await _designationRepository.FindByCodeAsync("TRA");
        var device = await _deviceRepository.FindByDeviceIdAsync("DEV-002");

        if (designDepartment == null || financeDepartment == null || ictDepartment == null ||
            staffCategory == null || traineeCategory == null || engineerDesignation == null ||
            executiveDesignation == null || traineeDesignation == null || device == null)
        {
            _logger.LogWarning("Demo data was not seeded because one or more required master records are missing.");
            return;
        }

        var employees = new[]
        {
            await GetOrCreateEmployeeAsync("DEMO-001", "Demo Employee One", designDepartment.Id, staffCategory.Id, engineerDesignation.Id),
            await GetOrCreateEmployeeAsync("DEMO-002", "Demo Employee Two", financeDepartment.Id, staffCategory.Id, executiveDesignation.Id),
            await GetOrCreateEmployeeAsync("DEMO-003", "Demo Employee Three", ictDepartment.Id, traineeCategory.Id, traineeDesignation.Id)
        };

        var demoPunches = new[]
        {
            (employees[0], "BRK", new DateTime(2026, 7, 15, 8, 15, 0)),
            (employees[1], "LUN", new DateTime(2026, 7, 15, 12, 45, 0)),
            (employees[2], "SNK", new DateTime(2026, 7, 15, 16, 0, 0)),
            (employees[0], "DIN", new DateTime(2026, 7, 15, 19, 30, 0))
        };

        foreach (var (employee, scheduleCode, punchTime) in demoPunches)
        {
            var schedule = await _timeScheduleRepository.FindByCodeAsync(scheduleCode);
            if (schedule?.ItemId == null)
            {
                _logger.LogWarning("Demo punch was not seeded because schedule {ScheduleCode} has no item.", scheduleCode);
                continue;
            }

            if (!await _checkInRepository.ExistsAsync(employee.EmployeeId, device.DeviceId, punchTime))
            {
                await _checkInRepository.InsertAsync(
                    new CanteenCheckIn(_guidGenerator.Create(), employee.EmployeeId, device.DeviceId, punchTime),
                    autoSave: true);
            }

            if (!await _mealTransactionRepository.ExistsAsync(employee.Id, punchTime))
            {
                var item = await _itemRepository.GetAsync(schedule.ItemId.Value);
                await _mealTransactionRepository.InsertAsync(
                    new MealTransaction(
                        _guidGenerator.Create(),
                        employee.Id,
                        device.Id,
                        schedule.Id,
                        item.Id,
                        item.Price,
                        punchTime,
                        MealTransactionSource.AutoSync),
                    autoSave: true);
            }
        }

        var deposits = new[]
        {
            (employees[0], 500m),
            (employees[1], 350m),
            (employees[2], 250m)
        };
        var depositDate = new DateTime(2026, 7, 15, 7, 0, 0);

        foreach (var (employee, amount) in deposits)
        {
            var existing = await _cashDepositRepository.GetListAsync(
                employeeId: employee.Id,
                from: depositDate,
                to: depositDate,
                includeDetails: false);
            if (existing.Any(d => d.Amount == amount && d.Notes == "Demo opening balance"))
            {
                continue;
            }

            await _cashDepositRepository.InsertAsync(
                new CashDeposit(
                    _guidGenerator.Create(),
                    employee.Id,
                    amount,
                    depositDate,
                    "Demo opening balance"),
                autoSave: true);
        }
    }

    private async Task<Employee> GetOrCreateEmployeeAsync(
        string employeeId,
        string fullName,
        Guid departmentId,
        Guid categoryId,
        Guid designationId)
    {
        var employee = await _employeeRepository.FindByEmployeeIdAsync(employeeId);
        if (employee != null)
        {
            employee.SetFullName(fullName);
            employee.SetDepartment(departmentId);
            employee.SetCategory(categoryId);
            employee.SetDesignation(designationId);
            return await _employeeRepository.UpdateAsync(employee, autoSave: true);
        }

        employee = new Employee(
            _guidGenerator.Create(),
            employeeId,
            fullName,
            departmentId,
            categoryId,
            designationId);
        return await _employeeRepository.InsertAsync(employee, autoSave: true);
    }

    private async Task<Department?> FindDepartmentAsync(string name)
    {
        var departments = await _departmentRepository.GetListAsync(filter: name);
        return departments.FirstOrDefault(d => d.Name == name);
    }

    private async Task<Category?> FindCategoryAsync(string name)
    {
        var categories = await _categoryRepository.GetListAsync(filter: name);
        return categories.FirstOrDefault(c => c.CategoryName == name);
    }
}
