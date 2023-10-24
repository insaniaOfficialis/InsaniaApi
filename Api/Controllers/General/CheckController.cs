using Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.General;

/// <summary>
/// Контроллер проверки соединения
/// </summary>
[Authorize]
[Route("api/v1/check")]
public class CheckController : BaseController
{
    private readonly ILogger<CheckController> _logger; //сервис для записи логов

    /// <summary>
    /// Конструктор контроллера проверки соединения
    /// </summary>
    /// <param name="logger"></param>
    public CheckController(ILogger<CheckController> logger) : base(logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Проверка соединения авторизованного пользователя
    /// </summary>
    /// <returns></returns>
    [HttpHead]
    public IActionResult CheckAuthorize()
    {
        return Ok();
    }

    /// <summary>
    /// Проверка соединения неавторизованного пользователя
    /// </summary>
    /// <returns></returns>
    [HttpHead]
    [AllowAnonymous]
    [Route("anonymous")]
    public IActionResult CheckNotAuthorize()
    {
        return Ok();
    }
}
