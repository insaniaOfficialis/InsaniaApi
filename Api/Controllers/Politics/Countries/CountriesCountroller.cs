using Api.Controllers.Base;
using Domain.Models.Base;
using Domain.Models.Politics.Countries.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Politics.Countries;

namespace Api.Controllers.Politics.Countries;

/// <summary>
/// Контроллер стран
/// </summary>
[Authorize]
[Route("api/v1/countries")]
public class CountriesCountroller : BaseController
{
    private readonly ILogger<CountriesCountroller> _logger; //интерфейс для записи логов
    private readonly ICountries _countries; //интерфейс стран

    /// <summary>
    /// Конструктор контроллера стран
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="countries"></param>
    public CountriesCountroller(ILogger<CountriesCountroller> logger, ICountries countries)
        : base(logger)
    {
        _logger = logger;
        _countries = countries;
    }

    /// <summary>
    /// Метод получения списка стран
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetCountries() => await GetAnswerAsync(async () =>
    {
        return await _countries.GetCountries();
    });

    /// <summary>
    /// Метод получения списка стран с полной информацией
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("listFull")]
    public async Task<IActionResult> GetCountriesFullInformation([FromQuery] string? search,
        [FromQuery] int? skip, [FromQuery] int? take, [FromQuery] List<BaseSortRequest?>? sort,
        [FromQuery] bool isDeleted) => await GetAnswerAsync(async () =>
        {
            return await _countries.GetCountriesFullInformation(search, skip, take, sort, isDeleted);
        });

    /// <summary>
    /// Метод добавления страны
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddCountry([FromBody] AddCountryRequest? request) => await GetAnswerAsync(async () =>
    {
        string? user = User?.Identity?.Name;
        return await _countries.AddCountry(request, user);
    });

    /// <summary>
    /// Метод добавления страны
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("update/{id}")]
    public async Task<IActionResult> UpdateCountry([FromBody] AddCountryRequest? request, [FromRoute] long id) => 
        await GetAnswerAsync(async () =>
    {
        string? user = User?.Identity?.Name;
        return await _countries.UpdateCountry(request, user, id);
    });

    /// <summary>
    /// Метод удаления страны
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteCountry([FromRoute] long id) => await GetAnswerAsync(async () =>
    {
        string? user = User?.Identity?.Name;
        return await _countries.UpdateCountry(true, id, user);
    });

    /// <summary>
    /// Метод восстановления страны
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("restore/{id}")]
    public async Task<IActionResult> RestoreCountry([FromRoute] long id) => await GetAnswerAsync(async () =>
    {
        string? user = User?.Identity?.Name;
        return await _countries.UpdateCountry(false, id, user);
    });
}