using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class TimeScheduleDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ITimeScheduleRepository _timeScheduleRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<TimeScheduleDataSeedContributor> _logger;

    public TimeScheduleDataSeedContributor(
        ITimeScheduleRepository timeScheduleRepository,
        IItemRepository itemRepository,
        IGuidGenerator guidGenerator,
        ILogger<TimeScheduleDataSeedContributor> logger)
    {
        _timeScheduleRepository = timeScheduleRepository;
        _itemRepository = itemRepository;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _timeScheduleRepository.GetCountAsync() > 0)
        {
            return;
        }

        var scheduleDefinitions = new List<(string Name, TimeOnly Start, TimeOnly End, string Code, string ItemName)>
        {
            ("Breakfast", TimeOnly.Parse("07:00"), TimeOnly.Parse("09:30"), "BRK", "Breakfast/Tiffen"),
            ("Lunch", TimeOnly.Parse("12:00"), TimeOnly.Parse("14:30"), "LUN", "Veg-Lunch/Dinner"),
            ("Snacks", TimeOnly.Parse("15:30"), TimeOnly.Parse("16:30"), "SNK", "Snacks"),
            ("Dinner", TimeOnly.Parse("19:00"), TimeOnly.Parse("21:30"), "DIN", "Veg-Lunch/Dinner")
        };

        foreach (var (name, start, end, code, itemName) in scheduleDefinitions)
        {
            var items = await _itemRepository.GetListAsync(filter: itemName);
            var matchedItem = items.FirstOrDefault(i => i.Description == itemName);

            if (matchedItem == null)
            {
                _logger.LogWarning("Could not seed item link for TimeSchedule '{TimeScheduleName}': item '{ItemName}' not found.", name, itemName);
                continue;
            }

            var ambiguousItems = items.Where(i => i.Description.Contains(itemName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (ambiguousItems.Count > 1 || name is "Lunch" or "Dinner")
            {
                _logger.LogWarning(
                    "TimeSchedule '{TimeScheduleName}' was linked to '{ItemName}' during seeding. The name matches multiple items; please confirm/correct via the UI.",
                    name, itemName);
            }

            var timeSchedule = new TimeSchedule(
                _guidGenerator.Create(),
                name,
                start,
                end,
                matchedItem.Id,
                code);

            await _timeScheduleRepository.InsertAsync(timeSchedule);
        }
    }
}
