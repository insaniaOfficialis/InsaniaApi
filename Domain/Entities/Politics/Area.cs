using Domain.Entities.Geography;
using Domain.Entities.Sociology;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Politics;

/// <summary>
/// Сущность областей
/// </summary>
[Table("dir_areas")]
[Comment("Области")]
public class Area : Directory
{
    /// <summary>
    /// Размер в пикселях
    /// </summary>
    [Column("size_in_pixels")]
    [Comment("Размер в пикселях")]
    public int SizeInPixels { get; private set; }

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
    /// Ссылка на географический объект
    /// </summary>
    [Column("geographical_object_id")]
    [Comment("Ссылка на географический объект")]
    public long GeographicalObjectId { get; private set; }
    
    /// <summary>
    /// Навигационное свойство географического объекта
    /// </summary>
    public GeographicalObject GeographicalObject { get; private set; }

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
    /// Ссылка на регион
    /// </summary>
    [Column("region_id")]
    [Comment("Ссылка на регион")]
    public long RegionId { get; private set; }

    /// <summary>
    /// Навигационное свойство региона
    /// </summary>
    public Region Region { get; private set; }

    /// <summary>
    /// Ссылка на фракцию
    /// </summary>
    [Column("fraction_id")]
    [Comment("Ссылка на фракцию")]
    public long FractionId { get; private set; }

    /// <summary>
    /// Навигационное свойство фракции
    /// </summary>
    public Fraction Fraction { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности областей
    /// </summary>
    public Area() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности областей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="sizeInPixels"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="geographicalObject"></param>
    /// <param name="country"></param>
    /// <param name="region"></param>
    /// <param name="fraction"></param>
    public Area(long id, string user, string name, int sizeInPixels, int number, string color, GeographicalObject geographicalObject, Country country,
        Region region, Fraction fraction) : base(id, user, name)
    {
        SizeInPixels = sizeInPixels;
        Number = number;
        Color = color;
        GeographicalObject = geographicalObject;
        GeographicalObjectId = geographicalObject.Id;
        Country = country;
        CountryId = country.Id;
        Region = region;
        RegionId = region.Id;
        Fraction = fraction;
        FractionId = fraction.Id;
    }

    /// <summary>
    /// Конструктор сущности областей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="sizeInPixels"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="geographicalObject"></param>
    /// <param name="country"></param>
    /// <param name="region"></param>
    /// <param name="fraction"></param>
    public Area(string user, string name, int sizeInPixels, int number, string color, GeographicalObject geographicalObject, Country country,
        Region region, Fraction fraction) : base(user, name)
    {
        SizeInPixels = sizeInPixels;
        Number = number;
        Color = color;
        GeographicalObject = geographicalObject;
        GeographicalObjectId = geographicalObject.Id;
        Country = country;
        CountryId = country.Id;
        Region = region;
        RegionId = region.Id;
        Fraction = fraction;
        FractionId = fraction.Id;
    }

    /// <summary>
    /// Метод записи размера в пикселях
    /// </summary>
    /// <param name="sizeInPixels"></param>
    public void SetSizeInPixels(int sizeInPixels)
    {
        SizeInPixels = sizeInPixels;
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
    /// Метод записи географического объекта
    /// </summary>
    /// <param name="geographicalObject"></param>
    public void SetGeographicalObject(GeographicalObject geographicalObject)
    {
        GeographicalObject = geographicalObject;
        GeographicalObjectId = geographicalObject.Id;
    }

    /// <summary>
    /// Метод записи страны
    /// </summary>
    /// <param name="country"></param>
    public void SetCountry(Country country)
    {
        Country = country;
        CountryId = country.Id;
    }

    /// <summary>
    /// Метод записи региона
    /// </summary>
    /// <param name="region"></param>
    public void SetRegion(Region region)
    {
        Region = region;
        RegionId = region.Id;
    }

    /// <summary>
    /// Метод записи фракции
    /// </summary>
    /// <param name="fraction"></param>
    public void SetFraction(Fraction fraction)
    {
        Fraction = fraction;
        FractionId = fraction.Id;
    }
}
