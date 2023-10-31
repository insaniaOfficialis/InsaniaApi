using Api.Controllers.Base;
using Domain.Models.General.Files.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.General.Files;
using Services.General.Files.GetFile;

namespace Api.Controllers.General.Files;

/// <summary>
/// Контроллер файлов
/// </summary>
[Authorize]
[Route("api/v1/files")]
public class FilesController : BaseController
{
    private readonly ILogger<FilesController> _logger; //логгер для записи логов
    private readonly IFiles _files; //сервис файлов
    private readonly IGetFile _getFile; //сервис получения файла

    /// <summary>
    /// Конструктор контроллера файлов
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="files"></param>
    /// <param name="getFile"></param>
    public FilesController(ILogger<FilesController> logger, IFiles files, IGetFile getFile) : base(logger)
    {
        _logger = logger;
        _files = files;
        _getFile = getFile;
    }

    /// <summary>
    /// Метод добавления файла
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add/{type}/{id}")]
    public async Task<IActionResult> AddFile([FromRoute] string type, [FromRoute] long id,
        [FromForm] IFormFile file) => await GetAnswerAsync(async () =>
    {
        AddFileRequest request = new(id, file.FileName, type, file.OpenReadStream());
        return await _files.AddFile(request);
    });

    /// <summary>
    /// Метод получения файла
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("{id}")]
    public async Task<FileContentResult?> GetFile([FromRoute] long id)
    {
        try
        {
            var result = await _getFile.Handler(id);

            if (result.Success && !string.IsNullOrEmpty(result.Path) && !string.IsNullOrEmpty(result.ContentType))
            {
                _logger.LogInformation("GetFile. Успешно");

                byte[] fileBytes = System.IO.File.ReadAllBytes(result.Path);

                return File(fileBytes, result.ContentType);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("GetFile. Обработанная ошибка: {0}", result.Error);
                        return null;
                    }
                    else
                    {
                        _logger.LogError("GetFile. Необработанная ошибка:  {0}", result.Error);
                        return null;
                    }
                }
                else
                {
                    _logger.LogError("GetFile. Непредвиденная ошибка");
                        return null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetFile. Необработанная ошибка:  {0}", ex);
            return null;
        }
    }
}