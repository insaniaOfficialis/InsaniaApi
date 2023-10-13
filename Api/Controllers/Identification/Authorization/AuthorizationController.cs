using Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Services.Identification.Authorization;

namespace Api.Controllers.Identification.Authorization;

/// <summary>
/// Контроллер авторизации
/// </summary>
[Route("api/v1/authorization")]
public class AuthorizationController : BaseController
{
    private readonly ILogger<AuthorizationController> _logger; //логгер для записи логов
    private readonly IAuthorization _authorization; //сервис авторизации

    /// <summary>
    /// Конструктор контроллера авторизации
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="authorization"></param>
    public AuthorizationController(ILogger<AuthorizationController> logger, IAuthorization authorization) : base(logger)
    {
        _logger = logger;
        _authorization = authorization;
    }

    /// <summary>
    /// Метод входа
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login([FromQuery] string? username, [FromQuery] string? password) => 
        await GetAnswerAsync(async () =>
    {
        return await _authorization.Login(username, password);
    });
}