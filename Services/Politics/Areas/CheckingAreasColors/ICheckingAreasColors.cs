using Domain.Models.Base;

namespace Services.Politics.Areas.CheckingAreasColors;

/// <summary>
/// Проверка цветов областей
/// </summary>
public interface ICheckingAreasColors
{
    /// <summary>
    /// Метод проверки входных данных
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Validator(string? value);

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? value);

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<bool> Query(string? value);
}