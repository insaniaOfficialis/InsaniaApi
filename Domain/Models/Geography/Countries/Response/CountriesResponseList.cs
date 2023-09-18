using Domain.Models.Base;

namespace Domain.Models.Geography.Countries.Response;

/// <summary>
/// Модель стандартного ответа для списка
/// </summary>
public class CountriesResponseList: BaseResponseList
{
    /// <summary>
    /// Простой конструктор ответа модели ответа списка
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public CountriesResponseList(bool success, BaseError? error): base(success, error)
    {
        
    }

    public CountriesResponseList(bool success, BaseError? error, List<CountriesResponseListItem?>? items): base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public new List<CountriesResponseListItem?>? Items { get; set; }
}

/// <summary>
/// Модель элемента списка
/// </summary>
public class CountriesResponseListItem: BaseResponseListItem
{
    /// <summary>
    /// Номер на карте
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Цвет на карте
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Язык для названий
    /// </summary>
    public string? LanguageForNames { get; set; }
}