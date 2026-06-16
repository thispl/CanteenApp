using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service interface for Canteen Check-In management
/// </summary>
public interface ICanteenCheckInAppService : IApplicationService
{
    /// <summary>
    /// Get check-in by ID
    /// </summary>
    Task<CanteenCheckInDto?> GetAsync(Guid id);

    /// <summary>
    /// Get list of check-ins with filtering (chronological, newest first)
    /// </summary>
    Task<PagedResultDto<CanteenCheckInDto>> GetListAsync(CanteenCheckInListFilterDto input);

    /// <summary>
    /// Create a manual check-in (for simulation/testing)
    /// </summary>
    Task<CanteenCheckInDto> CreateManualCheckInAsync(CreateManualCheckInDto input);

    /// <summary>
    /// Delete a check-in record
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Get latest check-ins for real-time display
    /// </summary>
    Task<List<CanteenCheckInDto>> GetLatestCheckInsAsync(int count = 50);
}
