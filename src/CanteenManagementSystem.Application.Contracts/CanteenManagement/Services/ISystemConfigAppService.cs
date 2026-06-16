using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

public interface ISystemConfigAppService : IApplicationService
{
    Task<SystemConfigDto> GetConfigAsync();
    Task<SystemConfigResultDto> SaveConfigAsync(SystemConfigDto input);
    Task<SystemConfigResultDto> TestConnectionAsync(string connectionString);
}
