using Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Politics.Areas.CheckingAreasColors;

namespace Api.Controllers.Politics.Areas;

/// <summary>
/// Контроллер областей
/// </summary>
[Authorize]
[Route("api/v1/areas")]
public class AreasController : BaseController
{
    private readonly ILogger<AreasController> _logger; //интерфейс для записи логов
    private readonly ICheckingAreasColors _checkingAreasColors; //проверка цвета области

    /// <summary>
    /// Конструктор контроллера областей
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="checkingAreasColors"></param>
    public AreasController(ILogger<AreasController> logger, ICheckingAreasColors checkingAreasColors)
        : base(logger)
    {
        _logger = logger;
        _checkingAreasColors = checkingAreasColors;
    }

    /// <summary>
    /// Метод проверки цвета области
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("check/colors")]
    public async Task<IActionResult> CheckingAreasColors([FromQuery] string? value) => await GetAnswerAsync(async () =>
    {
        return await _checkingAreasColors.Handler(value);
    });
}