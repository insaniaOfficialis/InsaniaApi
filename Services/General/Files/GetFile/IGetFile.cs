using Domain.Models.General.Files.Response;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.GetFile;

/// <summary>
/// Интерфейс получения файла
/// </summary>
public interface IGetFile
{
    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    Task<bool> Validator(long? id, long? entityId);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    Task<GetFileReponse> Handler(long? id, long? entityId);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="id"></param>
    Task<FileEntity> Query(long? id);
}
