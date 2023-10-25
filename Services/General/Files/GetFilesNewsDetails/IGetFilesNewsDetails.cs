using Domain.Models.Base;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.GetFilesNewsDetails;

/// <summary>
/// Интерфейс получения списка файлов детальной части новости
/// </summary>
public interface IGetFilesNewsDetails
{
    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="newsDetailId"></param>
    /// <returns></returns>
    Task<bool> Validator(long? newsDetailId);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="newsDetailId"></param>
    /// <returns></returns>
    Task<BaseResponseList> Handler(long? newsDetailId);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="newsDetailId"></param>
    /// <returns></returns>
    Task<List<FileEntity?>> Query(long? newsDetailId);
}