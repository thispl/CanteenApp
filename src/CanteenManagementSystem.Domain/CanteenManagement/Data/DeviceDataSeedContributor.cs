using System.Collections.Generic;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Data;

public class DeviceDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DeviceDataSeedContributor(
        IDeviceRepository deviceRepository,
        IGuidGenerator guidGenerator)
    {
        _deviceRepository = deviceRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _deviceRepository.GetCountAsync() > 0)
        {
            return;
        }

        var devices = new List<Device>
        {
            new Device(
                _guidGenerator.Create(),
                "DEV-001",
                "Main Entrance Biometric",
                "192.168.1.101",
                4370,
                DeviceStatus.Active,
                "Main Entrance",
                "ZKTeco K40",
                "SN123456789"),
            new Device(
                _guidGenerator.Create(),
                "DEV-002",
                "Canteen Attendance",
                "192.168.1.102",
                4370,
                DeviceStatus.Active,
                "Canteen Counter",
                "ZKTeco F18",
                "SN987654321"),
            new Device(
                _guidGenerator.Create(),
                "DEV-003",
                "Back Door Access",
                "192.168.1.103",
                4370,
                DeviceStatus.Inactive,
                "Rear Exit",
                "ZKTeco K40",
                "SN555555555")
        };

        foreach (var device in devices)
        {
            await _deviceRepository.InsertAsync(device);
        }
    }
}
