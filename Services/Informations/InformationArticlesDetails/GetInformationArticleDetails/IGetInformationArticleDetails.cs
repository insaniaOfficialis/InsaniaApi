using Domain.Entities.Informations;
using Domain.Models.Informations.InformationArticlesDetails.Response;

namespace Services.Informations.InformationArticlesDetails.GetInformationArticleDetails;

/// <summary>
/// Интерфейс получения списка детальных частей информационной статьи
/// </summary>
public interface IGetInformationArticleDetails
{
    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="informationArticleId"></param>
    /// <returns></returns>
    Task<bool> Validator(long? informationArticleId);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="informationArticleId"></param>
    /// <returns></returns>
    Task<GetInformationArticleDetailsResponse> Handler(long? informationArticleId);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="informationArticleId"></param>
    /// <returns></returns>
    Task<List<InformationArticleDetail>> Query(long? informationArticleId);

    /// <summary>
    /// Метод формирования ответа
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<GetInformationArticleDetailsResponse> RequestBuilder(List<InformationArticleDetail> request);
}