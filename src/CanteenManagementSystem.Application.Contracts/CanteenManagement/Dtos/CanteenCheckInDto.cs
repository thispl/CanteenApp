using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for CanteenCheckIn entity with employee details
/// </summary>
public class CanteenCheckInDto : AuditedEntityDto<Guid>
{
    public string EmployeeId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Department { get; set; }
    public string DeviceId { get; set; } = null!;
    public DateTime CheckInTime { get; set; }
}

/// <summary>
/// DTO for creating a manual check-in (simulation)
/// </summary>
public class CreateManualCheckInDto
{
    public string EmployeeId { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
    public DateTime? CheckInTime { get; set; }
}

/// <summary>
/// DTO for filtering check-ins
/// </summary>
public class CanteenCheckInListFilterDto : PagedAndSortedResultRequestDto
{
    public string? EmployeeId { get; set; }
    public string? DeviceId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
