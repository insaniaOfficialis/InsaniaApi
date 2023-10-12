using Domain.Models.Base;

namespace Services.Sociology.Nations;

/// <summary>
/// Интерфейс наций
/// </summary>
public interface INations
{
    /// <summary>
    /// Метод получения списка наций
    /// </summary>
    /// <param name="raceId"></param>
    /// <returns></returns>
    Task<BaseResponseList> GetNationsList(long? raceId);
}
