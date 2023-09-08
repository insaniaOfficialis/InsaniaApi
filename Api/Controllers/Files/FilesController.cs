using Domain.Models.Base;
using Domain.Models.Files.Request;
using Microsoft.AspNetCore.Mvc;
using Services.Files;

namespace Api.Controllers.Files;

/// <summary>
/// Контроллер файлов
/// </summary>
[Route("api/v1/files")]
public class FilesController : Controller
{
    private readonly ILogger<FilesController> _logger; //логгер для записи логов
    private readonly IFiles _files; //сервис ролей

    /// <summary>
    /// Конструктор контроллера файлов
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="files"></param>
    public FilesController(ILogger<FilesController> logger, IFiles files)
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
    public async Task<IActionResult> AddFile([FromRoute] string type, [FromRoute] long id, [FromForm] IFormFile file)
    {
        try
        {
            AddFileRequest request = new(id, file.FileName, type, file.OpenReadStream());

            var result = await _files.AddFile(request);

            if (result.Success)
            {
                _logger.LogInformation("AddFile. Успешно");
                return Ok(result);
            }
            else
            {
                if (result != null && result.Error != null)
                {
                    if (result.Error.Code != 500)
                    {
                        _logger.LogError("AddFile. Обработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 400, result.Error);
                    }
                    else
                    {
                        _logger.LogError("AddFile. Необработанная ошибка: " + result.Error);
                        return StatusCode(result.Error.Code ?? 500, result.Error);
                    }
                }
                else
                {
                    _logger.LogError("AddFile. Непредвиденная ошибка");
                    BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                    return StatusCode(500, response);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("AddFile. Необработанная ошибка: " + ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}