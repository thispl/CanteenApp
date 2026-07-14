using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

public interface ICashDepositRepository : IRepository<CashDeposit, Guid>
{
    Task<long> GetCountAsync(
        Guid? employeeId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    Task<List<CashDeposit>> GetListAsync(
        Guid? employeeId = null,
        DateTime? from = null,
        DateTime? to = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);
}
