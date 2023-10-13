using Api.Controllers.Base;
using Domain.Models.Identification.Registration.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Identification.Registration;

namespace Api.Controllers.Identification.Registration;

/// <summary>
/// Контроллер регистрации
/// </summary>
[Authorize]
[Route("api/v1/registration")]
public class RegistrationController: BaseController
{
    private readonly ILogger<RegistrationController> _logger; //логгер для записи логов
    private readonly IRegistration _registration; //сервис регистрации

    /// <summary>
    /// Конструктор контроллера регистрации
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="registration"></param>
    public RegistrationController(ILogger<RegistrationController> logger, IRegistration registration) : base(logger)
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
    public async Task<IActionResult> AddUser([FromBody] AddUserRequest? request) => await GetAnswerAsync(async () =>
        {
            return await _registration.AddUser(request);
        });
}
