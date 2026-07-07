using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class ItemDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IItemRepository _itemRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ItemDataSeedContributor(
        IItemRepository itemRepository,
        IGuidGenerator guidGenerator)
    {
        _itemRepository = itemRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var items = new Dictionary<string, decimal>
        {
            { "Breakfast/Tiffen", 53 },
            { "Veg-Lunch/Dinner", 80 },
            { "Tea/Coffee", 10 },
            { "Snacks", 21 },
            { "Mini Snacks-T/C", 25 },
            { "Chicken Bry-Jeeraga Samba", 195 },
            { "Mutton Bry-Jeeraga Sam", 400 },
            { "Fresh Juice", 35 },
            { "Spl Veg-Lunch", 120 },
            { "EGG Boiled", 10 },
            { "Health Drinks", 25 },
            { "Ice Cream", 25 },
            { "Spl Non Veg", 630 }
        };

        foreach (var (description, price) in items)
        {
            if (await _itemRepository.GetCountAsync(filter: description) == 0)
            {
                await _itemRepository.InsertAsync(
                    new Item(_guidGenerator.Create(), description, price),
                    autoSave: true);
            }
        }
    }
}
