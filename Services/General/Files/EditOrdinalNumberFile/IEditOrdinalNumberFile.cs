using Domain.Models.Base;

namespace Services.General.Files.EditOrdinalNumberFile;

/// <summary>
/// Изменение порядкового номера файла
/// </summary>
public interface IEditOrdinalNumberFile
{
    /// <summary>
    /// Проверка входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Validator(string? user, long? ordinalNumber, long? id);

    /// <summary>
    /// Обработка
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResponse> Handler(string? user, long? ordinalNumber, long? id);

    /// <summary>
    /// Запрос к базе
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<long?> Query(string? user, long? ordinalNumber, long? id);
}