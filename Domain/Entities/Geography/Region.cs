using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущность регионов
/// </summary>
[Table("dir_regions")]
[Comment("Регионы")]
public class Region : Directory
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
    /// Уникальный номер
    /// </summary>
    [Column("unique_number")]
    [Comment("Уникальный номер")]
    public string UniqueNumber { get; private set; }

    /// <summary>
    /// Ссылка на страну
    /// </summary>
    [Column("country_id")]
    [Comment("Ссылка на страну")]
    public long CountryId { get; private set; }

    /// <summary>
    /// Навигационное свойство страны
    /// </summary>
    public Country Country { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности регионов
    /// </summary>
    public Region() : base()
    {

    }

    /// <summary>
    /// Конструктор сущности регионов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="uniqueNumber"></param>
    /// <param name="country"></param>
    public Region(long id, string user, string name, int number, string color, string uniqueNumber, Country country) : base(id, user, name)
    {
        Number = number;
        Color = color;
        UniqueNumber = uniqueNumber;
        CountryId = country.Id;
        Country = country;
    }

    /// <summary>
    /// Конструктор сущности регионов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="uniqueNumber"></param>
    /// <param name="country"></param>
    public Region(string user, string name, int number, string color, string uniqueNumber, Country country) : base(user, name)
    {
        Number = number;
        Color = color;
        UniqueNumber = uniqueNumber;
        CountryId = country.Id;
        Country = country;
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
    /// Метод записи уникального номера
    /// </summary>
    /// <param name="uniqueNumber"></param>
    public void SetUniqueNumber(string uniqueNumber)
    {
        UniqueNumber = uniqueNumber;
    }

    /// <summary>
    /// Метод установки страны
    /// </summary>
    /// <param name="country"></param>
    public void SetCountry(Country country)
    {
        CountryId = country.Id;
        Country = country;
    }
}