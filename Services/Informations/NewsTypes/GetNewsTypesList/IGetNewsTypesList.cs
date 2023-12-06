using Domain.Entities.Informations;
using Domain.Models.Base;

namespace Services.Informations.NewsTypes.GetNewsTypesList;

/// <summary>
/// Получение списка типов новостей
/// </summary>
public interface IGetNewsTypesList
{
    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <returns></returns>
    Task<BaseResponseList> Handler();

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <returns></returns>
    Task<List<NewsType>> Query();
}