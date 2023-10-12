using Api.Controllers.Base;
using Domain.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Sociology.Nations;

namespace Api.Controllers.Sociology.Nations;

/// <summary>
/// Контроллер наций
/// </summary>
[Authorize]
[Route("api/v1/nations")]
public class NationsController : BaseController
{
    private readonly ILogger<NationsController> _logger; //интерфейс для записи логов
    private readonly INations _nations; //интерфейс наций

    /// <summary>
    /// Конструктор контроллера наций
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="nations"></param>
    public NationsController(ILogger<NationsController> logger, INations nations) : base(logger)
    {
        _logger = logger;
        _nations = nations;
    }

    /// <summary>
    /// Метод получения списка наций
    /// </summary>
    /// <param name="raceId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetNationsList([FromQuery] long? raceId)
        => await GetAnswerAsync(async () =>
        {
            return await _nations.GetNationsList(raceId);
        });
}