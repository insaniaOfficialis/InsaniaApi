using Domain.Models.Identification.Authorization.Response;

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
}
