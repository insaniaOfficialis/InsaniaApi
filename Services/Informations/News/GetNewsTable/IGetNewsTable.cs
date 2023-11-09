using Domain.Models.Base;
using Domain.Models.Informations.News.Response;
namespace Services.Informations.News.GetNewsTable;
using NewsEntity = Domain.Entities.Informations.News;

/// <summary>
/// Интерфейс получения списка новостей для таблицы
/// </summary>
public interface IGetNewsTable
{
    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    Task<GetNewsTableResponse> Handler(string? search, int? skip, int? take, List<BaseSortRequest?>? sort, bool? isDeleted);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    Task<List<NewsEntity>> Query(string? search, int? skip, int? take, List<BaseSortRequest?>? sort, bool? isDeleted);

    /// <summary>
    /// Метод формирования ответа
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    GetNewsTableResponse ResponseBuilder(List<NewsEntity> entities);
}