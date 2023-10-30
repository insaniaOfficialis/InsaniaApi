using Domain.Models.Base;
using Domain.Models.Informations.News.Request;

namespace Services.Informations.News.AddNews;

/// <summary>
/// Интерфейс добавления новости
/// </summary>
public interface IAddNews
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, AddNewsRequest? request);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, AddNewsRequest? request);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, AddNewsRequest? request);
}