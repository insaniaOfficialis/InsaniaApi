using Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Sociology.PersonalNames;

namespace Api.Controllers.Sociology.Names;

/// <summary>
/// Контроллер имён
/// </summary>
[Authorize]
[Route("api/v1/personalNames")]
public class PersonalNamesController : BaseController
{
    private readonly ILogger<PersonalNamesController> _logger; //интерфейс для записи логов
    private readonly IPersonalNames _personalNames; //интерфейс личных имён

    /// <summary>
    /// Конструктор контроллера имён
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="personalNames"></param>
    public PersonalNamesController(ILogger<PersonalNamesController> logger, IPersonalNames personalNames) : base(logger)
    {
        _logger = logger;
        _personalNames = personalNames;
    }

    /// <summary>
    /// Метод получения сгенерированного имени
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="generateLastName"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("generate")]
    public async Task<IActionResult> GetGeneratedName([FromQuery] long? nationId, [FromQuery] bool? gender, [FromQuery] bool generateLastName)
        => await GetAnswerAsync(async () =>
        {
            return await _personalNames.GetGeneratedName(nationId, gender, generateLastName);
        });

    /// <summary>
    /// Метод получения начал имён
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("beginningsNames")]
    public async Task<IActionResult> GetListBeginningsNames([FromQuery] long? nationId, [FromQuery] bool? gender)
        => await GetAnswerAsync(async () =>
        {
            return await _personalNames.GetListBeginningsNames(nationId, gender);
        });
}