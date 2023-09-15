using Domain.Models.Base;

namespace Domain.Models.Identification.Authorization.Response;

/// <summary>
/// Модель ответа авторизации
/// </summary>
public class AuthorizationResponse: BaseResponse
{
    /// <summary>
    /// Токен доступа
    /// </summary>
    public string? Token { get; set; }
}
