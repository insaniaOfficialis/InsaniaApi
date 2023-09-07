using Domain.Models.Base;
using Domain.Models.Identification.Registration.Request;

namespace Services.Identification.Registration;

/// <summary>
/// Интерфейс регистрации
/// </summary>
public interface IRegistration
{
    /// <summary>
    /// Метод добавления пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BaseResponse> AddUser(AddUserRequest? request);
}