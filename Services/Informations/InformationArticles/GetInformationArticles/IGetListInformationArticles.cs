using Domain.Entities.Informations;
using Domain.Models.Base;

namespace Services.Informations.InformationArticles.GetInformationArticles;

/// <summary>
/// Интерфейс получения списка информационных статей
/// </summary>
public interface IGetListInformationArticles
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
    Task<List<InformationArticle>> Query(string? search);
}
