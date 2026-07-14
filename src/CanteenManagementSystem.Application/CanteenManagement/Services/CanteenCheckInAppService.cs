using System;
using System.Collections.Generic;
using System.Linq;
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
/// Application service for Canteen Check-In management
/// </summary>
[Authorize]
public class CanteenCheckInAppService : ApplicationService, ICanteenCheckInAppService
{
    private readonly ICanteenCheckInRepository _checkInRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IGuidGenerator _guidGenerator;

    public CanteenCheckInAppService(
        ICanteenCheckInRepository checkInRepository,
        IEmployeeRepository employeeRepository,
        IGuidGenerator guidGenerator)
    {
        _checkInRepository = checkInRepository;
        _employeeRepository = employeeRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<CanteenCheckInDto?> GetAsync(Guid id)
    {
        var checkIn = await _checkInRepository.GetAsync(id);
        var employee = await _employeeRepository.FindByEmployeeIdAsync(checkIn.EmployeeId);

        return MapToDto(checkIn, employee);
    }

    public virtual async Task<PagedResultDto<CanteenCheckInDto>> GetListAsync(CanteenCheckInListFilterDto input)
    {
        var count = await _checkInRepository.GetCountAsync(
            input.EmployeeId,
            input.DeviceId,
            input.FromDate,
            input.ToDate);

        var checkIns = await _checkInRepository.GetListWithEmployeeAsync(
            input.MaxResultCount * 2); // Get more to ensure we have enough after filtering

        // Apply additional filters in memory for complex filtering
        if (!string.IsNullOrWhiteSpace(input.EmployeeId))
        {
            checkIns = checkIns.Where(c => c.EmployeeId == input.EmployeeId).ToList();
        }

        if (!string.IsNullOrWhiteSpace(input.DeviceId))
        {
            checkIns = checkIns.Where(c => c.DeviceId == input.DeviceId).ToList();
        }

        if (input.FromDate.HasValue)
        {
            checkIns = checkIns.Where(c => c.CheckInTime >= input.FromDate.Value).ToList();
        }

        if (input.ToDate.HasValue)
        {
            checkIns = checkIns.Where(c => c.CheckInTime <= input.ToDate.Value).ToList();
        }

        var totalCount = checkIns.Count;
        var pagedCheckIns = checkIns
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<CanteenCheckInDto>(
            totalCount,
            pagedCheckIns.Select(MapToDto).ToList());
    }

    public virtual async Task<CanteenCheckInDto> CreateManualCheckInAsync(CreateManualCheckInDto input)
    {
        // Check if employee exists
        var employee = await _employeeRepository.FindByEmployeeIdAsync(input.EmployeeId);
        if (employee == null)
        {
            throw new UserFriendlyException($"Employee with ID '{input.EmployeeId}' not found.");
        }

        var checkInTime = input.CheckInTime ?? Clock.Now;

        // Check for duplicate
        if (await _checkInRepository.ExistsAsync(input.EmployeeId, input.DeviceId, checkInTime))
        {
            throw new UserFriendlyException("A check-in with the same employee, device, and time already exists.");
        }

        var checkIn = new CanteenCheckIn(
            _guidGenerator.Create(),
            input.EmployeeId,
            input.DeviceId,
            checkInTime);

        await _checkInRepository.InsertAsync(checkIn, autoSave: true);

        return MapToDto(checkIn, employee);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _checkInRepository.DeleteAsync(id);
    }

    public virtual async Task<List<CanteenCheckInDto>> GetLatestCheckInsAsync(int count = 50)
    {
        var checkIns = await _checkInRepository.GetListWithEmployeeAsync(count);
        return checkIns.Select(MapToDto).ToList();
    }

    private CanteenCheckInDto MapToDto(CanteenCheckIn checkIn, Employee? employee)
    {
        return new CanteenCheckInDto
        {
            Id = checkIn.Id,
            EmployeeId = checkIn.EmployeeId,
            FullName = employee?.FullName ?? "Unknown",
            Department = employee?.Department?.Name,
            DeviceId = checkIn.DeviceId,
            CheckInTime = checkIn.CheckInTime
        };
    }

    private CanteenCheckInDto MapToDto(CanteenCheckInWithEmployee checkIn)
    {
        return new CanteenCheckInDto
        {
            Id = checkIn.Id,
            EmployeeId = checkIn.EmployeeId,
            FullName = checkIn.FullName,
            Department = checkIn.Department,
            DeviceId = checkIn.DeviceId,
            CheckInTime = checkIn.CheckInTime
        };
    }
}
