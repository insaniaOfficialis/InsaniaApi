using Domain.Models.Base;
using Domain.Models.Identification.Registration.Request;
using Microsoft.AspNetCore.Mvc;
using Services.Identification.Registration;

namespace Api.Controllers.Identification.Registration;

/// <summary>
/// Контроллер регистрации
/// </summary>
[Route("api/v1/registration")]
public class RegistrationController: Controller
{
    private readonly ILogger<RegistrationController> _logger; //логгер для записи логов
    private readonly IRegistration _registration; //сервис регистрации

    /// <summary>
    /// Конструктор контроллера регистрации
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="registration"></param>
    public RegistrationController(ILogger<RegistrationController> logger, IRegistration registration)
    {
        _logger = logger;
        _registration = registration;
    }

    /// <summary>
    /// Метод добавления пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddUser([FromBody] AddUserRequest? request)
    {
        try
        {
            var result = await _registration.AddUser(request);

            if (result.Success)
            {
                _logger.LogInformation("AddUser. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("AddUser. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result.Error);
                    }
                    else
                    {
                        _logger.LogError("AddUser. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result.Error);
                    }
                }
                else
                {
                    _logger.LogError("AddUser. Непредвиденная ошибка");
                    BaseResponse response = new(false, new(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("AddUser. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
