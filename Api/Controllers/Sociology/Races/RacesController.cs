﻿using Api.Controllers.Base;
using Domain.Entities.Sociology;
using Domain.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Sociology.Races;

namespace Api.Controllers.Sociology.Races;

/// <summary>
/// Контроллер рас
/// </summary>
[Authorize]
[Route("api/v1/races")]
public class RacesController : BaseController
{
    private readonly ILogger<RacesController> _logger; //интерфейс для записи логов
    private readonly IRaces _races; //интерфейс рас

    /// <summary>
    /// Конструктор контроллера рас
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="races"></param>
    public RacesController(ILogger<RacesController> logger, IRaces races) : base(logger)
    {
        _logger = logger;
        _races = races;
    }

    /// <summary>
    /// Метод получения списка рас
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetRacesList()
        => await GetAnswerAsync(async () =>
        {
            return await _races.GetRacesList();
        });
}
