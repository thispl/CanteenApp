using System.Collections.Generic;
using AutoMapper;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;

namespace CanteenManagementSystem.CanteenManagement;

/// <summary>
/// AutoMapper profile for CanteenManagement module
/// </summary>
public class CanteenManagementApplicationAutoMapperProfile : Profile
{
    public CanteenManagementApplicationAutoMapperProfile()
    {
        // Employee mappings
        CreateMap<Employee, EmployeeDto>();
        CreateMap<List<Employee>, List<EmployeeDto>>();

        // CanteenCheckIn mappings
        CreateMap<CanteenCheckIn, CanteenCheckInDto>();
        CreateMap<List<CanteenCheckIn>, List<CanteenCheckInDto>>();
    }
}
