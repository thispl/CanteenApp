using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service for dashboard statistics
/// </summary>
public interface IDashboardAppService : IApplicationService
{
    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
