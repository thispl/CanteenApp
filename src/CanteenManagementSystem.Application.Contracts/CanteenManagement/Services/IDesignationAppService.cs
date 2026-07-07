using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

public interface IDesignationAppService : IApplicationService
{
    Task<DesignationDto?> GetAsync(Guid id);

    Task<DesignationDto?> GetByCodeAsync(string code);

    Task<PagedResultDto<DesignationDto>> GetListAsync(DesignationListFilterDto input);

    Task<DesignationDto> CreateAsync(CreateDesignationDto input);

    Task<DesignationDto> UpdateAsync(Guid id, UpdateDesignationDto input);

    Task DeleteAsync(Guid id);

    Task<bool> ExistsByCodeAsync(string code);
}
