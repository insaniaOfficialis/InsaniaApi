using Domain.Models.Base;
using Domain.Models.General.Files.Request;

namespace Services.General.Files;

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
