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
    public async Task<IActionResult> GetGeneratedName([FromQuery] long? nationId, [FromQuery] bool? gender,
        [FromQuery] bool generateLastName)
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
    [Route("beginnings")]
    public async Task<IActionResult> GetListBeginningsNames([FromQuery] long? nationId, [FromQuery] bool? gender)
        => await GetAnswerAsync(async () =>
        {
            return await _personalNames.GetListBeginningsNames(nationId, gender);
        });

    /// <summary>
    /// Метод получения окончаний имён
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("endings")]
    public async Task<IActionResult> GetListEndingsNames([FromQuery] long? nationId, [FromQuery] bool? gender)
        => await GetAnswerAsync(async () =>
        {
            return await _personalNames.GetListEndingsNames(nationId, gender);
        });

    /// <summary>
    /// Метод генерации нового имени
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="firstSyllable"></param>
    /// <param name="lastSyllable"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("generateNew")]
    public async Task<IActionResult> GetGeneratingNewName([FromQuery] long? nationId, [FromQuery] bool? gender,
        [FromQuery] string? firstSyllable, [FromQuery] string? lastSyllable)
        => await GetAnswerAsync(async () =>
        {
            return await _personalNames.GetGeneratingNewName(nationId, gender, firstSyllable, lastSyllable);
        });

    /// <summary>
    /// Метод добавления имени
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="name"></param>
    /// <param name="probability"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddName([FromQuery] long? nationId, [FromQuery] bool? gender,
        [FromQuery] string? name, [FromQuery] double? probability)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;

            return await _personalNames.AddName(user, nationId, gender, name, probability);
        });
}