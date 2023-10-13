using Api.Controllers.Base;
using Domain.Models.Base;
using Domain.Models.Files.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Files;

namespace Api.Controllers.General.Files;

/// <summary>
/// Контроллер файлов
/// </summary>
[Authorize]
[Route("api/v1/files")]
public class FilesController : BaseController
{
    private readonly ILogger<FilesController> _logger; //логгер для записи логов
    private readonly IFiles _files; //сервис ролей

    /// <summary>
    /// Конструктор контроллера файлов
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="files"></param>
    public FilesController(ILogger<FilesController> logger, IFiles files) : base(logger)
    {
        _logger = logger;
        _files = files;
    }

    /// <summary>
    /// Метод добавления файла
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add/{type}/{id}")]
    public async Task<IActionResult> AddFile([FromRoute] string type, [FromRoute] long id, [FromForm] IFormFile file) => await GetAnswerAsync(async () =>
    {
        AddFileRequest request = new(id, file.FileName, type, file.OpenReadStream());
        return await _files.AddFile(request);
    });
}