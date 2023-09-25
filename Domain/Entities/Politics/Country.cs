using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Politics;

/// <summary>
/// Сущность стран
/// </summary>
[Table("dir_countries")]
[Comment("Страны")]
public class Country: Directory
{
    /// <summary>
    /// Номер на карте
    /// </summary>
    [Column("number")]
    [Comment("Номер на карте")]
    public int Number { get; private set; }

    /// <summary>
    /// Цвет на карте
    /// </summary>
    [Column("color")]
    [Comment("Цвет на карте")]
    public string Color { get; private set; }

    /// <summary>
    /// Язык для названий
    /// </summary>
    [Column("language_for_names")]
    [Comment("Язык для названий")]
    public string? LanguageForNames { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности стран
    /// </summary>
    public Country(): base()
    {
        
    }

    /// <summary>
    /// Конструктор сущности стран
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="languageForNames"></param>
    public Country(long id, string user, string name, int number, string color, string? languageForNames) : base(id, user, name)
    {
        Number = number;
        Color = color;
        LanguageForNames = languageForNames;
    }

    /// <summary>
    /// Конструктор сущности стран без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="languageForNames"></param>
    public Country(string user, string name, int number, string color, string? languageForNames) : base(user, name)
    {
        Number = number;
        Color = color;
        LanguageForNames = languageForNames;
    }

    /// <summary>
    /// Метод записи номера на карте
    /// </summary>
    /// <param name="number"></param>
    public void SetNumber(int number)
    {
        Number = number;
    }

    /// <summary>
    /// Метод записи цвета на карте
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(string color)
    {
        Color = color;
    }

    /// <summary>
    /// Метод записи языка для названий
    /// </summary>
    /// <param name="languageForNames"></param>
    public void SetLanguageForNames(string languageForNames)
    {
        LanguageForNames = languageForNames;
    }
}
