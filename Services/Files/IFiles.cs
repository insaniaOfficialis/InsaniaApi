using Domain.Models.Base;
using Domain.Models.Files.Request;

namespace Services.Files;

/// <summary>
/// Интерфейс файлов
/// </summary>
public interface IFiles
{
    /// <summary>
    /// Метод добавления файлов
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BaseResponse> AddFile(AddFileRequest? request);
}
