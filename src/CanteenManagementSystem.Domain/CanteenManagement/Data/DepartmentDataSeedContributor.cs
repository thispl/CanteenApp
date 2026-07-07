using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class DepartmentDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DepartmentDataSeedContributor(
        IDepartmentRepository departmentRepository,
        IGuidGenerator guidGenerator)
    {
        _departmentRepository = departmentRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var departments = new[]
        {
            "Design",
            "Fin & Acc",
            "HR & GA",
            "ICT",
            "Int Audit",
            "Logistics",
            "LogisticsM.H",
            "Manuf Engg",
            "PE",
            "NTC-Accounts",
            "NTC-Design",
            "NTC-Marketin",
            "NTC-ProdSupp",
            "Production",
            "Gardening",
            "Housekeeping",
            "PCD",
            "PSD",
            "Purchase",
            "QA",
            "Security",
            "SERVICE",
            "Reman",
            "Drivers",
            "OHE",
            "VISITOR"
        };

        foreach (var name in departments)
        {
            if (await _departmentRepository.GetCountAsync(filter: name) == 0)
            {
                await _departmentRepository.InsertAsync(
                    new Department(_guidGenerator.Create(), name),
                    autoSave: true);
            }
        }
    }
}
