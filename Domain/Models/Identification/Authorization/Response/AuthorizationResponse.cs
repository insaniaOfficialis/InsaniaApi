using Domain.Models.Base;

namespace Domain.Models.Identification.Authorization.Response;

/// <summary>
/// Модель ответа авторизации
/// </summary>
public class AuthorizationResponse: BaseResponse
{
    /// <summary>
    /// Простой конструктор ответа модели авторизации
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public AuthorizationResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор модели ответа для списка стран со списком
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="token"></param>
    public AuthorizationResponse(bool success, BaseError? error, string? token) : base(success, error)
    {
        Token = token;
    }

    /// <summary>
    /// Токен доступа
    /// </summary>
    public string? Token { get; set; }
}
