using Domain.Entities.Politics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущности связи климата с областями
/// </summary>
[Table("un_climates_areas")]
[Comment("Связь климата с областями")]
public class ClimateArea : BaseEntity
{
    /// <summary>
    /// Ссылка на климат
    /// </summary>
    [Column("climate_id")]
    [Comment("Ссылка на климат")]
    public long ClimateId { get; private set; }

    /// <summary>
    /// Навигационное свойство климата
    /// </summary>
    public Climate Climate { get; private set; }

    /// <summary>
    /// Ссылка на область
    /// </summary>
    [Column("area_id")]
    [Comment("Ссылка на область")]
    public long AreaId { get; private set; }

    /// <summary>
    /// Навигационное свойство области
    /// </summary>
    public Area Area { get; private set; }

    /// <summary>
    /// Размер в пикселях
    /// </summary>
    [Column("size_in_pixels")]
    [Comment("Размер в пикселях")]
    public int SizeInPixels { get; private set; }

    /// <summary>
    /// Признак морского климата
    /// </summary>
    [Column("is_marine")]
    [Comment("Признак морского климата")]
    public bool IsMarine { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности связи климата с областями
    /// </summary>
    public ClimateArea() : base()
    {
        
    }

    /// <summary>
    /// Конструктор сущности связи климата с областями
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="climate"></param>
    /// <param name="area"></param>
    /// <param name="sizeInPixels"></param>
    /// <param name="isMarine"></param>
    public ClimateArea(long id, string user, Climate climate, Area area, int sizeInPixels, bool isMarine) : base(id, user)
    {
        Climate = climate;
        ClimateId = climate.Id;
        Area = area;
        AreaId = area.Id;
        SizeInPixels = sizeInPixels;
        IsMarine = isMarine;
    }

    /// <summary>
    /// Конструктор сущности связи климата с областями без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="climate"></param>
    /// <param name="area"></param>
    /// <param name="sizeInPixels"></param>
    /// <param name="isMarine"></param>
    public ClimateArea(string user, Climate climate, Area area, int sizeInPixels, bool isMarine) : base(user)
    {
        Climate = climate;
        ClimateId = climate.Id;
        Area = area;
        AreaId = area.Id;
        SizeInPixels = sizeInPixels;
        IsMarine = isMarine;
    }

    /// <summary>
    /// Метод записи климата
    /// </summary>
    /// <param name="climate"></param>
    public void SetClimate(Climate climate)
    {
        Climate = climate;
        ClimateId = climate.Id;
    }

    /// <summary>
    /// Метод записи области
    /// </summary>
    /// <param name="area"></param>
    public void SetArea(Area area)
    {
        Area = area;
        AreaId = area.Id;
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
    /// Метод записи признака морского климата
    /// </summary>
    /// <param name="isMarine"></param>
    public void SetIsMarine(bool isMarine)
    {
        IsMarine = isMarine;
    }
}
