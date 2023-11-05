using Domain.Models.Informations.News.Response;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.GetNewsList;

/// <summary>
/// Интерфейс получения списка новостей
/// </summary>
public interface IGetNewsList
{
    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<GetNewsListResponse> Handler(string? search);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<List<NewsEntity>> Query(string? search);

    /// <summary>
    /// Метод формирования ответа
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    GetNewsListResponse ResponseBuilder(List<NewsEntity> entities);
}