using Domain.Models.Base;

namespace Services.Politics.Regions.GenerationRegionColor;

/// <summary>
/// Генерация цвета региона
/// </summary>
public interface IGenerationRegionColor
{
    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <returns></returns>
    Task<BaseResponseValue> Handler();

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <returns></returns>
    Task<string> Query();
}