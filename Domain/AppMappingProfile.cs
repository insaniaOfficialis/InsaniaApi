using AutoMapper;
using Domain.Entities.Identification;
using Domain.Models.Identification.Registration.Request;

namespace Domain;

public class AppMappingProfile: Profile
{
    public AppMappingProfile()
    {
        CreateMap<User, AddUserRequest>();
    }
}
