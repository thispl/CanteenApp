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

/// <summary>
/// Application service for Time Schedule management
/// </summary>
[Authorize]
public class TimeScheduleAppService : ApplicationService, ITimeScheduleAppService
{
    private readonly ITimeScheduleRepository _timeScheduleRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IGuidGenerator _guidGenerator;

    public TimeScheduleAppService(
        ITimeScheduleRepository timeScheduleRepository,
        IItemRepository itemRepository,
        IGuidGenerator guidGenerator)
    {
        _timeScheduleRepository = timeScheduleRepository;
        _itemRepository = itemRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<TimeScheduleDto?> GetAsync(Guid id)
    {
        var timeSchedule = await _timeScheduleRepository.GetAsync(id, includeDetails: true);
        return MapWithItemName(timeSchedule);
    }

    private TimeScheduleDto MapWithItemName(TimeSchedule timeSchedule)
    {
        var dto = ObjectMapper.Map<TimeSchedule, TimeScheduleDto>(timeSchedule);
        dto.ItemName = timeSchedule.Item?.Description;
        return dto;
    }

    public virtual async Task<TimeScheduleDto?> GetByCodeAsync(string code)
    {
        var timeSchedule = await _timeScheduleRepository.FindByCodeAsync(code);
        if (timeSchedule == null)
        {
            return null;
        }
        return ObjectMapper.Map<TimeSchedule, TimeScheduleDto>(timeSchedule);
    }

    public virtual async Task<PagedResultDto<TimeScheduleDto>> GetListAsync(TimeScheduleListFilterDto input)
    {
        var count = await _timeScheduleRepository.GetCountAsync(input.Filter);
        var timeSchedules = await _timeScheduleRepository.GetListAsync(
            input.Filter,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            includeDetails: true,
            CancellationToken.None);

        return new PagedResultDto<TimeScheduleDto>(
            count,
            timeSchedules.Select(MapWithItemName).ToList());
    }

    public virtual async Task<TimeScheduleDto> CreateAsync(CreateTimeScheduleDto input)
    {
        ValidateTimeRange(input);
        await ValidateItemAsync(input.ItemId);

        if (!string.IsNullOrWhiteSpace(input.Code) &&
            await _timeScheduleRepository.ExistsByCodeAsync(input.Code))
        {
            throw new UserFriendlyException($"Time schedule with code '{input.Code}' already exists.");
        }

        var timeSchedule = new TimeSchedule(
            _guidGenerator.Create(),
            input.Name,
            input.StartTime,
            input.EndTime,
            input.ItemId,
            input.Code);

        await _timeScheduleRepository.InsertAsync(timeSchedule);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var createdTimeSchedule = await _timeScheduleRepository.GetAsync(timeSchedule.Id, includeDetails: true);
        return MapWithItemName(createdTimeSchedule);
    }

    public virtual async Task<TimeScheduleDto> UpdateAsync(Guid id, UpdateTimeScheduleDto input)
    {
        ValidateTimeRange(input);
        await ValidateItemAsync(input.ItemId);

        var timeSchedule = await _timeScheduleRepository.GetAsync(id);

        if (!string.IsNullOrWhiteSpace(input.Code) &&
            input.Code != timeSchedule.Code &&
            await _timeScheduleRepository.ExistsByCodeAsync(input.Code))
        {
            throw new UserFriendlyException($"Time schedule with code '{input.Code}' already exists.");
        }

        timeSchedule.SetName(input.Name);
        timeSchedule.SetCode(input.Code);
        timeSchedule.SetStartTime(input.StartTime);
        timeSchedule.SetEndTime(input.EndTime);
        timeSchedule.SetItem(input.ItemId);

        await _timeScheduleRepository.UpdateAsync(timeSchedule);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var updatedTimeSchedule = await _timeScheduleRepository.GetAsync(timeSchedule.Id, includeDetails: true);
        return MapWithItemName(updatedTimeSchedule);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _timeScheduleRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _timeScheduleRepository.ExistsByCodeAsync(code);
    }

    private async Task ValidateItemAsync(Guid? itemId)
    {
        if (!itemId.HasValue || itemId.Value == Guid.Empty)
        {
            throw new UserFriendlyException("Item is required.");
        }

        var queryable = await _itemRepository.GetQueryableAsync();
        if (!await queryable.AnyAsync(i => i.Id == itemId.Value))
        {
            throw new UserFriendlyException("Selected item does not exist.");
        }
    }

    private void ValidateTimeRange(CreateTimeScheduleDto input)
    {
        if (input.EndTime <= input.StartTime)
        {
            throw new UserFriendlyException("End time must be after start time.");
        }
    }

    private void ValidateTimeRange(UpdateTimeScheduleDto input)
    {
        if (input.EndTime <= input.StartTime)
        {
            throw new UserFriendlyException("End time must be after start time.");
        }
    }
}
