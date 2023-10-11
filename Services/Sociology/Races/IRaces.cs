using Domain.Models.Base;

namespace Services.Sociology.Races;

/// <summary>
/// Интерфейс рас
/// </summary>
public interface IRaces
{
    /// <summary>
    /// Метод получения списка рас
    /// </summary>
    /// <returns></returns>
    Task<BaseResponseList> GetRacesList();
}
