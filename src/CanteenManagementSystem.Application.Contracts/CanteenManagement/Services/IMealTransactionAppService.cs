using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

public interface IMealTransactionAppService : IApplicationService
{
    Task<MealTransactionDto> GetAsync(Guid id);
    Task<PagedResultDto<MealTransactionDto>> GetListAsync(MealTransactionListFilterDto input);
    Task<MealTransactionDto> CreateAsync(CreateMealTransactionDto input);
    Task<MealTransactionDto> UpdateAsync(Guid id, UpdateMealTransactionDto input);
    Task DeleteAsync(Guid id);
}
