using Domain.Models.Base;
using Domain.Models.Identification.Roles.Request;

namespace Services.Identification.Roles;

/// <summary>
/// Интерфейс ролей
/// </summary>
public interface IRoles
{
    /// <summary>
    /// Метод добавления роли
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BaseResponse> AddRole(AddRoleRequest? request);
}
