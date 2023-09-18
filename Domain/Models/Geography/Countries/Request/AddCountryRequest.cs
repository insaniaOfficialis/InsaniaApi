namespace Domain.Models.Geography.Countries.Request;

/// <summary>
/// Модель запроса на добавление страны
/// </summary>
public class AddCountryRequest
{
    /// <summary>
    /// Наименование
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Номер на карте
    /// </summary>
    public int? Number { get; set; }

    /// <summary>
    /// Цвет на карте
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Язык для названий
    /// </summary>
    public string? LanguageForNames { get; set; }

    /// <summary>
    /// Конструктор модели запроса на добавление стран
    /// </summary>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="languageForNames"></param>
    public AddCountryRequest(string? name, int? number, string? color, string? languageForNames)
    {
        Name = name;
        Number = number;
        Color = color;
        LanguageForNames = languageForNames;
    }
}
