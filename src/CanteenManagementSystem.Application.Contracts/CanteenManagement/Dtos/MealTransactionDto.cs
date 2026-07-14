using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

public class MealTransactionDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeIdNumber { get; set; } = null!;

    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = null!;

    public Guid TimeScheduleId { get; set; }
    public string TimeScheduleName { get; set; } = null!;

    public Guid ItemId { get; set; }
    public string ItemDescription { get; set; } = null!;

    public decimal Price { get; set; }
    public DateTime PunchTime { get; set; }
    public MealTransactionSource Source { get; set; }
}

public class CreateMealTransactionDto
{
    public Guid EmployeeId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid TimeScheduleId { get; set; }
    public DateTime PunchTime { get; set; }
    public MealTransactionSource Source { get; set; } = MealTransactionSource.ManualEntry;
}

public class UpdateMealTransactionDto
{
    public Guid EmployeeId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid TimeScheduleId { get; set; }
    public DateTime PunchTime { get; set; }
}

public class MealTransactionListFilterDto : PagedAndSortedResultRequestDto
{
    public Guid? EmployeeId { get; set; }
    public Guid? TimeScheduleId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public MealTransactionSource? Source { get; set; }
}
