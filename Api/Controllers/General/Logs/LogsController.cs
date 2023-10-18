using Api.Controllers.Base;
using Domain.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.General.Logs;

namespace Api.Controllers.General.Logs;

/// <summary>
/// Контроллер логов
/// </summary>
[Authorize]
[Route("api/v1/logs")]
public class LogsController : BaseController
{
    private readonly ILogger<LogsController> _logger; //интерфейс для записи логов
    private readonly IGetLogs _getLogs; //интерфейс сервиса получения списка логов

    /// <summary>
    /// Конструктор контроллера логов
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="getLogs"></param>
    public LogsController(ILogger<LogsController> logger, IGetLogs getLogs)
        : base(logger)
    {
        _logger = logger;
        _getLogs = getLogs;
    }

    /// <summary>
    /// Метод получения списка логов
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetLogs(string? search, int? skip, int? take,
        List<BaseSortRequest?>? sort, DateTime? from, DateTime? to, bool? success) 
        => await GetAnswerAsync(async () =>
    {
        return await _getLogs.Handler(search, skip, take, sort, from, to, success);
    });
}