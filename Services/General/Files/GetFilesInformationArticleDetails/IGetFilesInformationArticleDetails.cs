using Domain.Models.Base;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.GetFilesInformationArticleDetails;

/// <summary>
/// Интерфейс получения списка файлов детальной части информационной статьи
/// </summary>
public interface IGetFilesInformationArticleDetails
{
    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="informationArticleDetailId"></param>
    /// <returns></returns>
    Task<bool> Validator(long? informationArticleDetailId);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="informationArticleDetailId"></param>
    /// <returns></returns>
    Task<BaseResponseList> Handler(long? informationArticleDetailId);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="informationArticleDetailId"></param>
    /// <returns></returns>
    Task<List<FileEntity?>> Query(long? informationArticleDetailId);
}