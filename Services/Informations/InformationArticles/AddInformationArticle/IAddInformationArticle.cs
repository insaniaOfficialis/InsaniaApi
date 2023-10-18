using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Informations.InformationArticles.Request;

namespace Services.Informations.InformationArticles.AddInformationArticle;

/// <summary>
/// Интерфейс добавления информационной статьи
/// </summary>
public interface IAddInformationArticle
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, AddInformationArticleRequest? request);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, AddInformationArticleRequest? request);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, AddInformationArticleRequest? request);
}
