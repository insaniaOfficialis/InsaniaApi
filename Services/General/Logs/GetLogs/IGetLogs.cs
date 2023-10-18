using Domain.Entities.General.Log;
using Domain.Models.Base;
using Domain.Models.General.Logs.Response;

namespace Services.General.Logs;

/// <summary>
/// Интерфейс получения списка стран
/// </summary>
public interface IGetLogs
{
    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="success"></param>
    /// <returns></returns>
    Task<GetLogsResponse> Handler(string? search, int? skip, int? take, List<BaseSortRequest?>? sort,
        DateTime? from, DateTime? to, bool? success);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="success"></param>
    /// <returns></returns>
    Task<List<Log>> Query(string? search, int? skip, int? take, List<BaseSortRequest?>? sort,
        DateTime? from, DateTime? to, bool? success);
}