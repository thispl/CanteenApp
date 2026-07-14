using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

public class CashDepositDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeIdNumber { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime DepositDate { get; set; }

    public string? Notes { get; set; }
}

public class CreateCashDepositDto
{
    public Guid EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DepositDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}

public class UpdateCashDepositDto
{
    public Guid EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DepositDate { get; set; }
    public string? Notes { get; set; }
}

public class CashDepositListFilterDto : PagedAndSortedResultRequestDto
{
    public Guid? EmployeeId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
