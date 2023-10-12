using Domain.Models.Base;
using Domain.Models.Sociology.Names;

namespace Services.Sociology.PersonalNames;

/// <summary>
/// Интерфейс имён
/// </summary>
public interface IPersonalNames
{
    /// <summary>
    /// Метод получения сгенерированного имени
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="generateLastName"></param>
    /// <returns></returns>
    Task<GeneratedName> GetGeneratedName(long? nationId, bool? gender, bool generateLastName);
}
