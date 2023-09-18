using AutoMapper;
using Domain.Entities.Geography;
using Domain.Entities.Identification;
using Domain.Models.Base;
using Domain.Models.Geography.Countries.Response;
using Domain.Models.Identification.Registration.Request;
using Domain.Models.Identification.Roles.Request;

namespace Domain;

public class AppMappingProfile: Profile
{
    public AppMappingProfile()
    {
        CreateMap<AddUserRequest, User>();
        CreateMap<AddRoleRequest, Role>();
        CreateMap<Role, BaseResponseListItem>();
        CreateMap<Country, BaseResponseListItem>();
        CreateMap<Country, CountriesResponseListItem>();
    }
}
