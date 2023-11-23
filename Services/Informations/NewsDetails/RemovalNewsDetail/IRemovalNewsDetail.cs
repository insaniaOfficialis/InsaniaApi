using Domain.Models.Base;

namespace Services.Informations.NewsDetails.RemovalNewsDetail;

/// <summary>
/// Интерфейс удаления/восстановления детальной части новости
/// </summary>
public interface IRemovalNewsDetail
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, long? id);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, long? id, bool? isDeleted);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, long? id, bool? isDeleted);
}