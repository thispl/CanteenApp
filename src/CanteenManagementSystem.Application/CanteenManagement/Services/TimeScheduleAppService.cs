using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IGuidGenerator _guidGenerator;

    public TimeScheduleAppService(
        ITimeScheduleRepository timeScheduleRepository,
        IGuidGenerator guidGenerator)
    {
        _timeScheduleRepository = timeScheduleRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<TimeScheduleDto?> GetAsync(Guid id)
    {
        var timeSchedule = await _timeScheduleRepository.GetAsync(id);
        return ObjectMapper.Map<TimeSchedule, TimeScheduleDto>(timeSchedule);
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
            CancellationToken.None);

        return new PagedResultDto<TimeScheduleDto>(
            count,
            ObjectMapper.Map<List<TimeSchedule>, List<TimeScheduleDto>>(timeSchedules));
    }

    public virtual async Task<TimeScheduleDto> CreateAsync(CreateTimeScheduleDto input)
    {
        ValidateTimeRange(input);

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
            input.Code);

        await _timeScheduleRepository.InsertAsync(timeSchedule);

        return ObjectMapper.Map<TimeSchedule, TimeScheduleDto>(timeSchedule);
    }

    public virtual async Task<TimeScheduleDto> UpdateAsync(Guid id, UpdateTimeScheduleDto input)
    {
        ValidateTimeRange(input);

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

        await _timeScheduleRepository.UpdateAsync(timeSchedule);

        return ObjectMapper.Map<TimeSchedule, TimeScheduleDto>(timeSchedule);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _timeScheduleRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _timeScheduleRepository.ExistsByCodeAsync(code);
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
