using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

public interface ITimeScheduleAppService : IApplicationService
{
    Task<TimeScheduleDto?> GetAsync(Guid id);

    Task<TimeScheduleDto?> GetByCodeAsync(string code);

    Task<PagedResultDto<TimeScheduleDto>> GetListAsync(TimeScheduleListFilterDto input);

    Task<TimeScheduleDto> CreateAsync(CreateTimeScheduleDto input);

    Task<TimeScheduleDto> UpdateAsync(Guid id, UpdateTimeScheduleDto input);

    Task DeleteAsync(Guid id);

    Task<bool> ExistsByCodeAsync(string code);
}
