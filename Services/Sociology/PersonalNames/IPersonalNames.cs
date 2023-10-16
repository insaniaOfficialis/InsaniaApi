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

    /// <summary>
    /// Метод получения начал имён
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    Task<BaseResponseList> GetListBeginningsNames(long? nationId, bool? gender);

    /// <summary>
    /// Метод получения окончания имён
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    Task<BaseResponseList> GetListEndingsNames(long? nationId, bool? gender);

    /// <summary>
    /// Метод генерации нового имени
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="firstSyllable"></param>
    /// <param name="lastSyllable"></param>
    /// <returns></returns>
    Task<GeneratedName> GetGeneratingNewName(long? nationId, bool? gender, string? firstSyllable,
        string? lastSyllable);

    /// <summary>
    /// Метод добавления имени
    /// </summary>
    /// <param name="user"></param>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="name"></param>
    /// <param name="probability"></param>
    /// <returns></returns>
    Task<BaseResponse> AddName(string? user, long? nationId, bool? gender, string? name,
        double? probability);
}
