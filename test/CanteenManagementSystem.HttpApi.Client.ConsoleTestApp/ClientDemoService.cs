using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Account;

namespace CanteenManagementSystem.HttpApi.Client.ConsoleTestApp;

public class ClientDemoService : ITransientDependency
{
    private readonly IProfileAppService _profileAppService;
    private readonly IIdentityUserAppService _identityUserAppService;
    private readonly ICategoryAppService _categoryAppService;

    public ClientDemoService(
        IProfileAppService profileAppService,
        IIdentityUserAppService identityUserAppService,
        ICategoryAppService categoryAppService)
    {
        _profileAppService = profileAppService;
        _identityUserAppService = identityUserAppService;
        _categoryAppService = categoryAppService;
    }

    public async Task RunAsync()
    {
        var profileDto = await _profileAppService.GetAsync();
        Console.WriteLine($"UserName : {profileDto.UserName}");
        Console.WriteLine($"Email    : {profileDto.Email}");
        Console.WriteLine($"Name     : {profileDto.Name}");
        Console.WriteLine($"Surname  : {profileDto.Surname}");
        Console.WriteLine();

        var resultDto = await _identityUserAppService.GetListAsync(new GetIdentityUsersInput());
        Console.WriteLine($"Total users: {resultDto.TotalCount}");
        foreach (var identityUserDto in resultDto.Items)
        {
            Console.WriteLine($"- [{identityUserDto.Id}] {identityUserDto.Name}");
        }

        Console.WriteLine();
        Console.WriteLine("Testing Category CRUD via dynamic proxy...");

        var created = await _categoryAppService.CreateAsync(new CreateCategoryDto
        {
            CategoryName = "Proxy Test Category",
            CategoryCode = "PTC"
        });
        Console.WriteLine($"Created category: {created.Id} - {created.CategoryName}");

        var updated = await _categoryAppService.UpdateAsync(created.Id, new UpdateCategoryDto
        {
            CategoryName = "Proxy Updated Category",
            CategoryCode = "PTC2"
        });
        Console.WriteLine($"Updated category: {updated.Id} - {updated.CategoryName}");

        await _categoryAppService.DeleteAsync(created.Id);
        Console.WriteLine("Deleted category successfully.");
    }
}
