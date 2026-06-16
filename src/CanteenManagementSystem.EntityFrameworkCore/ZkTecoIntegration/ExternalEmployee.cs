namespace CanteenManagementSystem.EntityFrameworkCore.ZkTecoIntegration;

/// <summary>
/// External Employee entity from ZKTeco personnel_employee table.
/// Read-only entity mapped to external database.
/// </summary>
public class ExternalEmployee
{
    /// <summary>
    /// EnrollNumber from ZKTeco - matches EmployeeId in our system
    /// </summary>
    public virtual string? EnrollNumber { get; set; }

    /// <summary>
    /// Name from ZKTeco - employee's full name
    /// </summary>
    public virtual string? Name { get; set; }
}
