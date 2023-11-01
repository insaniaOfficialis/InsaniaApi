using Domain.Models.Base;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.GetFilesUser;

/// <summary>
/// Интерфейс получения списка файлов пользователя
/// </summary>
public interface IGetFilesUser
{
    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Validator(long? id);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResponseList> Handler(long? id);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<FileEntity?>> Query(long? id);
}