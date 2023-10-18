using Domain.Models.Base;
using Domain.Models.Informations.InformationArticlesDetails.Request;

namespace Services.Informations.InformationArticlesDetails.AddInformationArticleDetail;

/// <summary>
/// Интерфейс добавления детальной части информационной статьи
/// </summary>
public interface IAddInformationArticleDetail
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, AddInformationArticleDetailRequest? request);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, AddInformationArticleDetailRequest? request);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, AddInformationArticleDetailRequest? request);
}