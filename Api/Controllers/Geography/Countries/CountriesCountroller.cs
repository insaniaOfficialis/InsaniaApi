using Domain.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Geography.Countries;

namespace Api.Controllers.Geography.Countries;

/// <summary>
/// Контроллер стран
/// </summary>
[Authorize]
[Route("api/v1/countries")]
public class CountriesCountroller : Controller
{
    private readonly ILogger<CountriesCountroller> _logger; //интерфейс для записи логов
    private readonly ICountries _countries; //интерфейс стран

    /// <summary>
    /// Конструктор контроллера стран
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="countries"></param>
    public CountriesCountroller(ILogger<CountriesCountroller> logger, ICountries countries)
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
    public async Task<IActionResult> GetCountries()
    {
        try
        {
            var result = await _countries.GetCountries();

            if (result.Success)
            {
                _logger.LogInformation("GetCountries. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("GetCountries. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result);
                    }
                    else
                    {
                        _logger.LogError("GetCountries. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result);
                    }
                }
                else
                {
                    _logger.LogError("GetCountries. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetCountries. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));

        }
    }
}
