using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class CategoryDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGuidGenerator _guidGenerator;

    public CategoryDataSeedContributor(
        ICategoryRepository categoryRepository,
        IGuidGenerator guidGenerator)
    {
        _categoryRepository = categoryRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var categories = new[]
        {
            "Staff",
            "Workers",
            "Transport Dr",
            "Housekeeping",
            "Security",
            "InsideContr",
            "Suppliers",
            "Visitors",
            "Center-Cho",
            "Hancho",
            "Operator",
            "Apprentice",
            "Contract Labour",
            "Experts",
            "Trainee",
            "GUEST"
        };

        foreach (var categoryName in categories)
        {
            if (await _categoryRepository.GetCountAsync(filter: categoryName) == 0)
            {
                await _categoryRepository.InsertAsync(
                    new Category(_guidGenerator.Create(), categoryName),
                    autoSave: true);
            }
        }
    }
}
