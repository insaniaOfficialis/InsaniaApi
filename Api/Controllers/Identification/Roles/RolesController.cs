using Api.Controllers.Identification.Registration;
using Domain.Models.Base;
using Domain.Models.Identification.Registration.Request;
using Domain.Models.Identification.Roles.Request;
using Microsoft.AspNetCore.Mvc;
using Services.Identification.Registration;
using Services.Identification.Roles;

namespace Api.Controllers.Identification.Roles;

/// <summary>
/// Контроллер ролей
/// </summary>
[Route("api/v1/roles")]
public class RolesController : Controller
{
    private readonly ILogger<RolesController> _logger; //логгер для записи логов
    private readonly IRoles _roles; //сервис ролей

    /// <summary>
    /// Конструктор контроллера ролей
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="roles"></param>
    public RolesController(ILogger<RolesController> logger, IRoles roles)
    {
        _logger = logger;
        _roles = roles;
    }

    /// <summary>
    /// Метод добавления роли
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddRole([FromBody] AddRoleRequest? request)
    {
        try
        {
            var result = await _roles.AddRole(request);

            if (result.Success)
            {
                _logger.LogInformation("AddRole. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("AddRole. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result.Error);
                    }
                    else
                    {
                        _logger.LogError("AddRole. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result.Error);
                    }
                }
                else
                {
                    _logger.LogError("AddRole. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("AddRole. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Метод получения списка ролей
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var result = await _roles.GetRoles();

            if (result.Success)
            {
                _logger.LogInformation("GetRoles. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("GetRoles. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result.Error);
                    }
                    else
                    {
                        _logger.LogError("GetRoles. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result.Error);
                    }
                }
                else
                {
                    _logger.LogError("GetRoles. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetRoles. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}