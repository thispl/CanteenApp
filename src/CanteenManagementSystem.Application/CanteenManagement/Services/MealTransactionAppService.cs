using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Services;

[Authorize]
public class MealTransactionAppService : ApplicationService, IMealTransactionAppService
{
    private readonly IMealTransactionRepository _mealTransactionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ITimeScheduleRepository _timeScheduleRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IGuidGenerator _guidGenerator;

    public MealTransactionAppService(
        IMealTransactionRepository mealTransactionRepository,
        IEmployeeRepository employeeRepository,
        IDeviceRepository deviceRepository,
        ITimeScheduleRepository timeScheduleRepository,
        IItemRepository itemRepository,
        IGuidGenerator guidGenerator)
    {
        _mealTransactionRepository = mealTransactionRepository;
        _employeeRepository = employeeRepository;
        _deviceRepository = deviceRepository;
        _timeScheduleRepository = timeScheduleRepository;
        _itemRepository = itemRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<MealTransactionDto> GetAsync(Guid id)
    {
        var transaction = await _mealTransactionRepository.GetAsync(id, includeDetails: true);
        return MapWithNames(transaction);
    }

    public virtual async Task<PagedResultDto<MealTransactionDto>> GetListAsync(MealTransactionListFilterDto input)
    {
        var count = await _mealTransactionRepository.GetCountAsync(
            input.EmployeeId,
            input.TimeScheduleId,
            input.From,
            input.To,
            input.Source);

        var transactions = await _mealTransactionRepository.GetListAsync(
            input.EmployeeId,
            input.TimeScheduleId,
            input.From,
            input.To,
            input.Source,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            includeDetails: true);

        return new PagedResultDto<MealTransactionDto>(
            count,
            transactions.Select(MapWithNames).ToList());
    }

    public virtual async Task<MealTransactionDto> CreateAsync(CreateMealTransactionDto input)
    {
        await ValidateForeignKeysAsync(input.EmployeeId, input.DeviceId, input.TimeScheduleId);

        var timeSchedule = await _timeScheduleRepository.GetAsync(input.TimeScheduleId);
        if (!timeSchedule.ItemId.HasValue)
        {
            throw new UserFriendlyException("Selected time schedule is not linked to an item.");
        }

        var item = await _itemRepository.GetAsync(timeSchedule.ItemId.Value);
        var transaction = new MealTransaction(
            _guidGenerator.Create(),
            input.EmployeeId,
            input.DeviceId,
            input.TimeScheduleId,
            item.Id,
            item.Price,
            input.PunchTime,
            input.Source);

        await _mealTransactionRepository.InsertAsync(transaction);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var createdTransaction = await _mealTransactionRepository.GetAsync(transaction.Id, includeDetails: true);
        return MapWithNames(createdTransaction);
    }

    public virtual async Task<MealTransactionDto> UpdateAsync(Guid id, UpdateMealTransactionDto input)
    {
        await ValidateForeignKeysAsync(input.EmployeeId, input.DeviceId, input.TimeScheduleId);

        var transaction = await _mealTransactionRepository.GetAsync(id);
        var timeSchedule = await _timeScheduleRepository.GetAsync(input.TimeScheduleId);
        if (!timeSchedule.ItemId.HasValue)
        {
            throw new UserFriendlyException("Selected time schedule is not linked to an item.");
        }

        var item = await _itemRepository.GetAsync(timeSchedule.ItemId.Value);

        transaction.SetEmployee(input.EmployeeId);
        transaction.SetDevice(input.DeviceId);
        transaction.SetPunchTime(input.PunchTime);
        transaction.SetTimeSchedule(input.TimeScheduleId, item.Id, item.Price);

        await _mealTransactionRepository.UpdateAsync(transaction);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var updatedTransaction = await _mealTransactionRepository.GetAsync(transaction.Id, includeDetails: true);
        return MapWithNames(updatedTransaction);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _mealTransactionRepository.DeleteAsync(id);
    }

    private async Task ValidateForeignKeysAsync(Guid employeeId, Guid deviceId, Guid timeScheduleId)
    {
        var employees = await _employeeRepository.GetQueryableAsync();
        if (!await employees.AnyAsync(e => e.Id == employeeId))
        {
            throw new UserFriendlyException("Selected employee does not exist.");
        }

        var devices = await _deviceRepository.GetQueryableAsync();
        if (!await devices.AnyAsync(d => d.Id == deviceId))
        {
            throw new UserFriendlyException("Selected device does not exist.");
        }

        var timeSchedules = await _timeScheduleRepository.GetQueryableAsync();
        if (!await timeSchedules.AnyAsync(t => t.Id == timeScheduleId))
        {
            throw new UserFriendlyException("Selected time schedule does not exist.");
        }
    }

    private MealTransactionDto MapWithNames(MealTransaction transaction)
    {
        var dto = ObjectMapper.Map<MealTransaction, MealTransactionDto>(transaction);
        dto.EmployeeName = transaction.Employee?.FullName ?? string.Empty;
        dto.EmployeeIdNumber = transaction.Employee?.EmployeeId ?? string.Empty;
        dto.DeviceName = transaction.Device?.Name ?? string.Empty;
        dto.TimeScheduleName = transaction.TimeSchedule?.Name ?? string.Empty;
        dto.ItemDescription = transaction.Item?.Description ?? string.Empty;
        return dto;
    }
}
