using Api.Controllers.Base;
using Domain.Models.General.Files.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.General.Files;
using Services.General.Files.EditOrdinalNumberFile;
using Services.General.Files.GetFile;
using Services.General.Files.ManagingFileDeletion;

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
    private readonly IManagingFileDeletion _managingFileDeletion; //управление удалением файла
    private readonly IEditOrdinalNumberFile _editOrdinalNumberFile; //изменения порядкового номера файла

    /// <summary>
    /// Конструктор контроллера файлов
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="files"></param>
    /// <param name="getFile"></param>
    /// <param name="managingFileDeletion"></param>
    /// <param name="editOrdinalNumberFile"></param>
    public FilesController(ILogger<FilesController> logger, IFiles files, IGetFile getFile, IManagingFileDeletion managingFileDeletion,
        IEditOrdinalNumberFile editOrdinalNumberFile) 
        : base(logger)
    {
        _logger = logger;
        _files = files;
        _getFile = getFile;
        _managingFileDeletion = managingFileDeletion;
        _editOrdinalNumberFile = editOrdinalNumberFile;
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
        AddFileRequest request = new(id, file.FileName, type, file.OpenReadStream(), 0);
        return await _files.AddFile(request);
    });

    /// <summary>
    /// Метод добавления файла с порядковым номером
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <param name="file"></param>
    /// <param name="ordinalNumber"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add/{type}/{id}/{ordinalNumber}")]
    public async Task<IActionResult> AddFile([FromRoute] string type, [FromRoute] long id,
        [FromForm] IFormFile file, [FromRoute] long ordinalNumber) => await GetAnswerAsync(async () =>
        {
            AddFileRequest request = new(id, file.FileName, type, file.OpenReadStream(), ordinalNumber);
            return await _files.AddFile(request);
        });

    /// <summary>
    /// Метод получения файла
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("{entityId}/{id}")]
    public async Task<FileContentResult?> GetFile([FromRoute] long entityId, [FromRoute] long id)
    {
        try
        {
            var result = await _getFile.Handler(id, entityId);

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

    /// <summary>
    /// Управление удалением файла
    /// </summary>
    /// <param name="isDeleted"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> ManagingFileDeletion([FromQuery] bool? isDeleted, [FromRoute] long id)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _managingFileDeletion.Handler(user, id, isDeleted);
        });

    /// <summary>
    /// Изменения порядкового номера файла
    /// </summary>
    /// <param name="ordinalNumber"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("ordinalNumber/{id}")]
    public async Task<IActionResult> EditOrdinalNumberFile([FromQuery] long? ordinalNumber, [FromRoute] long? id)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _editOrdinalNumberFile.Handler(user, ordinalNumber, id);
        });
}