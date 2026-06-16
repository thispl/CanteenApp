using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// Repository interface for CanteenCheckIn entity
/// </summary>
public interface ICanteenCheckInRepository : IRepository<CanteenCheckIn, Guid>
{
    /// <summary>
    /// Check if a check-in already exists for the given employee, device and timestamp
    /// Used for duplicate prevention during sync
    /// </summary>
    Task<bool> ExistsAsync(
        string employeeId,
        string deviceId,
        DateTime checkInTime,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get chronological list of check-ins (newest first)
    /// </summary>
    Task<List<CanteenCheckIn>> GetListChronologicalAsync(
        int maxResultCount = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get check-ins with employee details
    /// </summary>
    Task<List<CanteenCheckInWithEmployee>> GetListWithEmployeeAsync(
        int maxResultCount = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get filtered count of check-ins
    /// </summary>
    Task<long> GetCountAsync(
        string? employeeId = null,
        string? deviceId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO for check-in with employee information
/// </summary>
public class CanteenCheckInWithEmployee
{
    public Guid Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Department { get; set; }
    public string DeviceId { get; set; } = null!;
    public DateTime CheckInTime { get; set; }
}
