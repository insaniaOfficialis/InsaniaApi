using Domain.Models.Base;
using Domain.Models.Informations.News.Request;

namespace Services.Informations.News.EditNews;

/// <summary>
/// Интерфейс изменения новости
/// </summary>
public interface IEditNews
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, AddNewsRequest? request, long? id);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, AddNewsRequest? request, long? id);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, AddNewsRequest? request, long? id);
}