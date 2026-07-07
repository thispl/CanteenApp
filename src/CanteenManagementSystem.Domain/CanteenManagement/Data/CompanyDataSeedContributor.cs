using System.Collections.Generic;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class CompanyDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IGuidGenerator _guidGenerator;

    public CompanyDataSeedContributor(
        ICompanyRepository companyRepository,
        IGuidGenerator guidGenerator)
    {
        _companyRepository = companyRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _companyRepository.GetCountAsync() > 0)
        {
            return;
        }

        var companies = new List<Company>
        {
            new Company(
                _guidGenerator.Create(),
                "Acme Corporation",
                "ACME",
                "123 Business Park, Suite 100",
                "+1-555-0100",
                "info@acme.example.com",
                "GST-0100",
                "https://acme.example.com"),
            new Company(
                _guidGenerator.Create(),
                "Globex Industries",
                "GLOB",
                "456 Industrial Way",
                "+1-555-0200",
                "contact@globex.example.com",
                "GST-0200",
                "https://globex.example.com")
        };

        foreach (var company in companies)
        {
            await _companyRepository.InsertAsync(company);
        }
    }
}
