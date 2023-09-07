using AutoMapper;
using Domain.Entities.Identification;
using Domain.Models.Identification.Registration.Request;
using Domain.Models.Identification.Roles.Request;

namespace Domain;

public class AppMappingProfile: Profile
{
    public AppMappingProfile()
    {
        CreateMap<AddUserRequest, User>();
        CreateMap<AddRoleRequest, Role>();
    }
}
