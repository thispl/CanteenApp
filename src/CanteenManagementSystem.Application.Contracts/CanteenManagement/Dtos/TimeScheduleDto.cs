using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for TimeSchedule entity
/// </summary>
public class TimeScheduleDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemName { get; set; }
}

/// <summary>
/// DTO for creating a new time schedule
/// </summary>
public class CreateTimeScheduleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid? ItemId { get; set; }
}

/// <summary>
/// DTO for updating a time schedule
/// </summary>
public class UpdateTimeScheduleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid? ItemId { get; set; }
}

/// <summary>
/// DTO for filtering time schedules
/// </summary>
public class TimeScheduleListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
