using Domain.Models.Base;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.GetNewsFullList;

/// <summary>
/// Интерфейс получения полного списка новостей
/// </summary>
public interface IGetNewsFullList
{
    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<BaseResponseList> Handler(string? search);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<List<NewsEntity>> Query(string? search);
}