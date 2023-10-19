using AutoMapper;
using Domain.Entities.Politics;
using Domain.Entities.Identification;
using Domain.Models.Base;
using Domain.Models.Politics.Countries.Response;
using Domain.Models.Identification.Registration.Request;
using Domain.Models.Identification.Roles.Request;
using Domain.Entities.Sociology;
using Domain.Models.Identification.Users.Response;
using Domain.Models.General.Logs.Response;
using Domain.Entities.General.Log;
using Domain.Entities.Informations;

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
        CreateMap<Race, BaseResponseListItem>();
        CreateMap<Nation, BaseResponseListItem>();
        CreateMap<PersonalName, BaseResponseListItem>();
        CreateMap<User, UserInfoResponse>();
        CreateMap<Log, GetLogsResponseItem>();
        CreateMap<InformationArticle, BaseResponseListItem>().ForMember("Name", x => x.MapFrom(y => y.Title));
    }
}
