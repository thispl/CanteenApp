using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

public interface ICashDepositAppService : IApplicationService
{
    Task<CashDepositDto> GetAsync(Guid id);
    Task<PagedResultDto<CashDepositDto>> GetListAsync(CashDepositListFilterDto input);
    Task<CashDepositDto> CreateAsync(CreateCashDepositDto input);
    Task<CashDepositDto> UpdateAsync(Guid id, UpdateCashDepositDto input);
    Task DeleteAsync(Guid id);
}
