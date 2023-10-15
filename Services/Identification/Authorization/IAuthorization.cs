using Domain.Models.Identification.Authorization.Response;
using Domain.Models.Identification.Users.Response;

namespace Services.Identification.Authorization;

/// <summary>
/// Интерфейс авторизации
/// </summary>
public interface IAuthorization
{
    /// <summary>
    /// Метод авторизации
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<AuthorizationResponse> Login(string? username, string? password);

    /// <summary>
    /// Метод получения информации о пользователе
    /// </summary>
    /// <returns></returns>
    Task<UserInfoResponse> GetUserInfo(string? username);
}
