using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

public interface IMealTransactionRepository : IRepository<MealTransaction, Guid>
{
    Task<bool> ExistsAsync(
        Guid employeeId,
        DateTime punchTime,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Guid? employeeId = null,
        Guid? timeScheduleId = null,
        DateTime? from = null,
        DateTime? to = null,
        MealTransactionSource? source = null,
        CancellationToken cancellationToken = default);

    Task<List<MealTransaction>> GetListAsync(
        Guid? employeeId = null,
        Guid? timeScheduleId = null,
        DateTime? from = null,
        DateTime? to = null,
        MealTransactionSource? source = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);
}
