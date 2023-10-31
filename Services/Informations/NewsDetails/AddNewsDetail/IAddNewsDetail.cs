using Domain.Models.Base;
using Domain.Models.Informations.NewsDetails.Request;

namespace Services.Informations.NewsDetails.AddNewsDetail;

/// <summary>
/// Интерфейс добавления детальной части новости
/// </summary>
public interface IAddNewsDetail
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, AddNewsDetailRequest? request);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, AddNewsDetailRequest? request);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, AddNewsDetailRequest? request);
}