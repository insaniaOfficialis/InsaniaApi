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

    /// <summary>
    /// Метод получения списка стран с полной информацией
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("listFull")]
    public async Task<IActionResult> GetCountriesFullInformation([FromQuery] string? search, [FromQuery] int? skip, [FromQuery] int? take,
        [FromQuery] List<BaseSortRequest?>? sort, [FromQuery] bool isDeleted)
    {
        try
        {
            var result = await _countries.GetCountriesFullInformation(search, skip, take, sort, isDeleted);

            if (result.Success)
            {
                _logger.LogInformation("GetCountriesFullInformation. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("GetCountriesFullInformation. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result);
                    }
                    else
                    {
                        _logger.LogError("GetCountriesFullInformation. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result);
                    }
                }
                else
                {
                    _logger.LogError("GetCountriesFullInformation. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetCountriesFullInformation. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));

        }
    }

    /// <summary>
    /// Метод добавления страны
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddCountry([FromBody] AddCountryRequest? request)
    {
        try
        {
            string? user = User?.Identity?.Name;

            var result = await _countries.AddCountry(request, user);

            if (result.Success)
            {
                _logger.LogInformation("AddCountry. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("AddCountry. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result);
                    }
                    else
                    {
                        _logger.LogError("AddCountry. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result);
                    }
                }
                else
                {
                    _logger.LogError("AddCountry. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("AddCountry. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));

        }
    }

    /// <summary>
    /// Метод добавления страны
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("update/{id}")]
    public async Task<IActionResult> UpdateCountry([FromBody] AddCountryRequest? request, [FromRoute] long id)
    {
        try
        {
            string? user = User?.Identity?.Name;

            var result = await _countries.UpdateCountry(request, user, id);

            if (result.Success)
            {
                _logger.LogInformation("UpdateCountry. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("UpdateCountry. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result);
                    }
                    else
                    {
                        _logger.LogError("UpdateCountry. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result);
                    }
                }
                else
                {
                    _logger.LogError("UpdateCountry. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("UpdateCountry. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));

        }
    }

    /// <summary>
    /// Метод удаления страны
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteCountry([FromRoute] long id)
    {
        try
        {
            string? user = User?.Identity?.Name;

            var result = await _countries.UpdateCountry(true, id, user);

            if (result.Success)
            {
                _logger.LogInformation("DeleteCountry. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("DeleteCountry. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result);
                    }
                    else
                    {
                        _logger.LogError("DeleteCountry. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result);
                    }
                }
                else
                {
                    _logger.LogError("DeleteCountry. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("DeleteCountry. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));

        }
    }

    /// <summary>
    /// Метод восстановления страны
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("restore/{id}")]
    public async Task<IActionResult> RestoreCountry([FromRoute] long id)
    {
        try
        {
            string? user = User?.Identity?.Name;

            var result = await _countries.UpdateCountry(false, id, user);

            if (result.Success)
            {
                _logger.LogInformation("RestoreCountry. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("RestoreCountry. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result);
                    }
                    else
                    {
                        _logger.LogError("RestoreCountry. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result);
                    }
                }
                else
                {
                    _logger.LogError("RestoreCountry. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("RestoreCountry. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));

        }
    }
}
