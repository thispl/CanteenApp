using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class TimeScheduleDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ITimeScheduleRepository _timeScheduleRepository;
    private readonly IGuidGenerator _guidGenerator;

    public TimeScheduleDataSeedContributor(
        ITimeScheduleRepository timeScheduleRepository,
        IGuidGenerator guidGenerator)
    {
        _timeScheduleRepository = timeScheduleRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _timeScheduleRepository.GetCountAsync() > 0)
        {
            return;
        }

        var timeSchedules = new List<TimeSchedule>
        {
            new TimeSchedule(_guidGenerator.Create(), "Breakfast", TimeOnly.Parse("07:00"), TimeOnly.Parse("09:30"), "BRK"),
            new TimeSchedule(_guidGenerator.Create(), "Lunch", TimeOnly.Parse("12:00"), TimeOnly.Parse("14:30"), "LUN"),
            new TimeSchedule(_guidGenerator.Create(), "Snacks", TimeOnly.Parse("15:30"), TimeOnly.Parse("16:30"), "SNK"),
            new TimeSchedule(_guidGenerator.Create(), "Dinner", TimeOnly.Parse("19:00"), TimeOnly.Parse("21:30"), "DIN")
        };

        foreach (var timeSchedule in timeSchedules)
        {
            await _timeScheduleRepository.InsertAsync(timeSchedule);
        }
    }
}
