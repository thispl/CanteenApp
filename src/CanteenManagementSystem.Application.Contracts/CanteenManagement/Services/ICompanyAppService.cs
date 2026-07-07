using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

public interface ICompanyAppService : IApplicationService
{
    Task<CompanyDto?> GetAsync(Guid id);

    Task<CompanyDto?> GetByCodeAsync(string code);

    Task<PagedResultDto<CompanyDto>> GetListAsync(CompanyListFilterDto input);

    Task<CompanyDto> CreateAsync(CreateCompanyDto input);

    Task<CompanyDto> UpdateAsync(Guid id, UpdateCompanyDto input);

    Task DeleteAsync(Guid id);

    Task<bool> ExistsByCodeAsync(string code);
}
