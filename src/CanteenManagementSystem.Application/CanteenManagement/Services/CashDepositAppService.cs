using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Services;

[Authorize]
public class CashDepositAppService : ApplicationService, ICashDepositAppService
{
    private readonly ICashDepositRepository _cashDepositRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IGuidGenerator _guidGenerator;

    public CashDepositAppService(
        ICashDepositRepository cashDepositRepository,
        IEmployeeRepository employeeRepository,
        IGuidGenerator guidGenerator)
    {
        _cashDepositRepository = cashDepositRepository;
        _employeeRepository = employeeRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<CashDepositDto> GetAsync(Guid id)
    {
        var deposit = await _cashDepositRepository.GetAsync(id, includeDetails: true);
        return MapWithNames(deposit);
    }

    public virtual async Task<PagedResultDto<CashDepositDto>> GetListAsync(CashDepositListFilterDto input)
    {
        var count = await _cashDepositRepository.GetCountAsync(input.EmployeeId, input.From, input.To);

        var deposits = await _cashDepositRepository.GetListAsync(
            input.EmployeeId,
            input.From,
            input.To,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            includeDetails: true);

        return new PagedResultDto<CashDepositDto>(count, deposits.Select(MapWithNames).ToList());
    }

    public virtual async Task<CashDepositDto> CreateAsync(CreateCashDepositDto input)
    {
        await ValidateEmployeeAsync(input.EmployeeId);
        ValidateAmount(input.Amount);

        var deposit = new CashDeposit(
            _guidGenerator.Create(),
            input.EmployeeId,
            input.Amount,
            input.DepositDate,
            input.Notes);

        await _cashDepositRepository.InsertAsync(deposit);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var createdDeposit = await _cashDepositRepository.GetAsync(deposit.Id, includeDetails: true);
        return MapWithNames(createdDeposit);
    }

    public virtual async Task<CashDepositDto> UpdateAsync(Guid id, UpdateCashDepositDto input)
    {
        await ValidateEmployeeAsync(input.EmployeeId);
        ValidateAmount(input.Amount);

        var deposit = await _cashDepositRepository.GetAsync(id);
        deposit.Update(input.EmployeeId, input.Amount, input.DepositDate, input.Notes);

        await _cashDepositRepository.UpdateAsync(deposit);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var updatedDeposit = await _cashDepositRepository.GetAsync(deposit.Id, includeDetails: true);
        return MapWithNames(updatedDeposit);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _cashDepositRepository.DeleteAsync(id);
    }

    private async Task ValidateEmployeeAsync(Guid employeeId)
    {
        var employees = await _employeeRepository.GetQueryableAsync();
        if (!await employees.AnyAsync(e => e.Id == employeeId))
        {
            throw new UserFriendlyException("Selected employee does not exist.");
        }
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new UserFriendlyException("Deposit amount must be greater than zero.");
        }
    }

    private CashDepositDto MapWithNames(CashDeposit deposit)
    {
        var dto = ObjectMapper.Map<CashDeposit, CashDepositDto>(deposit);
        dto.EmployeeName = deposit.Employee?.FullName ?? string.Empty;
        dto.EmployeeIdNumber = deposit.Employee?.EmployeeId ?? string.Empty;
        return dto;
    }
}
