using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;

namespace CanteenManagementSystem;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CanteenManagementSystemApplicationMappers : MapperBase<Employee, EmployeeDto>
{
    public override partial EmployeeDto Map(Employee source);
    public override partial void Map(Employee source, EmployeeDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CanteenCheckInMappers : MapperBase<CanteenCheckIn, CanteenCheckInDto>
{
    public override partial CanteenCheckInDto Map(CanteenCheckIn source);
    public override partial void Map(CanteenCheckIn source, CanteenCheckInDto destination);
}
