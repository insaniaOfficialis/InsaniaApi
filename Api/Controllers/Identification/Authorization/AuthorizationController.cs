using Domain.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Services.Identification.Authorization;

namespace Api.Controllers.Identification.Authorization;

/// <summary>
/// Контроллер авторизации
/// </summary>
[Route("api/v1/authorization")]
public class AuthorizationController : Controller
{
    private readonly ILogger<AuthorizationController> _logger; //логгер для записи логов
    private readonly IAuthorization _authorization; //сервис авторизации

    /// <summary>
    /// Конструктор контроллера авторизации
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="authorization"></param>
    public AuthorizationController(ILogger<AuthorizationController> logger, IAuthorization authorization)
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
    public async Task<IActionResult> Login([FromQuery] string? username, [FromQuery] string? password)
    {
        try
        {
            var result = await _authorization.Login(username, password);

            if (result.Success)
            {
                _logger.LogInformation("Login. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("Login. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result);
                    }
                    else
                    {
                        _logger.LogError("Login. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result);
                    }
                }
                else
                {
                    _logger.LogError("Login. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Login. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));
        }
    }
}