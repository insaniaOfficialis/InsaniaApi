using Domain.Models.Base;

namespace Services.Geography.Countries;

/// <summary>
/// Интерфейс стран
/// </summary>
public interface ICountries
{
    /// <summary>
    /// Метод получения списка стран
    /// </summary>
    /// <returns></returns>
    Task<BaseResponseList> GetCountries();
}
