using Api.Controllers.Base;
using Domain.Models.Identification.Roles.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Identification.Roles;

namespace Api.Controllers.Identification.Roles;

/// <summary>
/// Контроллер ролей
/// </summary>
[Authorize]
[Route("api/v1/roles")]
public class RolesController : BaseController
{
    private readonly ILogger<RolesController> _logger; //логгер для записи логов
    private readonly IRoles _roles; //сервис ролей

    /// <summary>
    /// Конструктор контроллера ролей
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="roles"></param>
    public RolesController(ILogger<RolesController> logger, IRoles roles) : base(logger)
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
    public async Task<IActionResult> AddRole([FromBody] AddRoleRequest? request) => await GetAnswerAsync(async () =>
    {
        return await _roles.AddRole(request);
    });

    /// <summary>
    /// Метод получения списка ролей
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetRoles() => await GetAnswerAsync(async () =>
    {
        return await _roles.GetRoles();
    });
}