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

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CategoryMappers : MapperBase<Category, CategoryDto>
{
    public override partial CategoryDto Map(Category source);
    public override partial void Map(Category source, CategoryDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DepartmentMappers : MapperBase<Department, DepartmentDto>
{
    public override partial DepartmentDto Map(Department source);
    public override partial void Map(Department source, DepartmentDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ItemMappers : MapperBase<Item, ItemDto>
{
    public override partial ItemDto Map(Item source);
    public override partial void Map(Item source, ItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TimeScheduleMappers : MapperBase<TimeSchedule, TimeScheduleDto>
{
    public override partial TimeScheduleDto Map(TimeSchedule source);
    public override partial void Map(TimeSchedule source, TimeScheduleDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DesignationMappers : MapperBase<Designation, DesignationDto>
{
    public override partial DesignationDto Map(Designation source);
    public override partial void Map(Designation source, DesignationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CompanyMappers : MapperBase<Company, CompanyDto>
{
    public override partial CompanyDto Map(Company source);
    public override partial void Map(Company source, CompanyDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DeviceMappers : MapperBase<Device, DeviceDto>
{
    public override partial DeviceDto Map(Device source);
    public override partial void Map(Device source, DeviceDto destination);
}
