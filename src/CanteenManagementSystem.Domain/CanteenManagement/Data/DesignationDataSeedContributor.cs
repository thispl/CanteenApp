using System.Collections.Generic;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class DesignationDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IDesignationRepository _designationRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DesignationDataSeedContributor(
        IDesignationRepository designationRepository,
        IGuidGenerator guidGenerator)
    {
        _designationRepository = designationRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _designationRepository.GetCountAsync() > 0)
        {
            return;
        }

        var designations = new List<Designation>
        {
            new Designation(_guidGenerator.Create(), "Manager", "MGR", "Department manager"),
            new Designation(_guidGenerator.Create(), "Engineer", "ENG", "Software engineer"),
            new Designation(_guidGenerator.Create(), "Executive", "EXE", "General executive"),
            new Designation(_guidGenerator.Create(), "Supervisor", "SUP", "Team supervisor"),
            new Designation(_guidGenerator.Create(), "Trainee", "TRA", "New trainee")
        };

        foreach (var designation in designations)
        {
            await _designationRepository.InsertAsync(designation);
        }
    }
}
