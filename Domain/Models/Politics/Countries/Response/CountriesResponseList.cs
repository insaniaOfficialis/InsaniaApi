using Domain.Models.Base;

namespace Domain.Models.Politics.Countries.Response;

/// <summary>
/// Модель ответа для списка стран
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

    /// <summary>
    /// Конструктор модели ответа для списка стран со списком
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
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