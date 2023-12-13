using Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Politics.Regions.CheckingRegionsColors;

namespace Api.Controllers.Politics.Regions;

/// <summary>
/// Контроллер регионов
/// </summary>
[Authorize]
[Route("api/v1/regions")]
public class RegionsController : BaseController
{
    private readonly ILogger<RegionsController> _logger; //интерфейс для записи логов
    private readonly ICheckingRegionsColors _checkingRegionsColors; //проверка цвета регионов

    /// <summary>
    /// Конструктор контроллера регионов
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="checkingRegionsColors"></param>
    public RegionsController(ILogger<RegionsController> logger, ICheckingRegionsColors checkingRegionsColors)
        : base(logger)
    {
        _logger = logger;
        _checkingRegionsColors = checkingRegionsColors;
    }

    /// <summary>
    /// Метод проверки цвета регионов
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("check/colors")]
    public async Task<IActionResult> CheckingRegionsColors([FromQuery] string? value) => await GetAnswerAsync(async () =>
    {
        return await _checkingRegionsColors.Handler(value);
    });
}