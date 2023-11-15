using Domain.Models.Base;
using Domain.Models.Informations.NewsDetails.Request;

namespace Services.Informations.NewsDetails.EditNewsDetail;

/// <summary>
/// Интерфейс редактирования детальной части новости
/// </summary>
public interface IEditNewsDetail
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, AddNewsDetailRequest? request, long? id);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, AddNewsDetailRequest? request, long? id);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, AddNewsDetailRequest? request, long? id);
}