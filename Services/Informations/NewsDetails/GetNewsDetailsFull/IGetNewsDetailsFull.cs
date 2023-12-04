using Domain.Entities.Informations;
using Domain.Models.Informations.NewsDetails.Response;

namespace Services.Informations.NewsDetails.GetNewsDetailsFull;

/// <summary>
/// Получение всех детальных частей новости
/// </summary>
public interface IGetNewsDetailsFull
{
    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="newsId"></param>
    /// <returns></returns>
    Task<bool> Validator(long? newsId);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="newsId"></param>
    /// <returns></returns>
    Task<GetNewsDetailsFullResponse> Handler(long? newsId);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="newsId"></param>
    /// <returns></returns>
    Task<List<NewsDetail>> Query(long? newsId);

    /// <summary>
    /// Метод формирования ответа
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<GetNewsDetailsFullResponse> RequestBuilder(List<NewsDetail> request);
}