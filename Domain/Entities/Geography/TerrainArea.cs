using Domain.Entities.Politics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущности связи рельефа с областями
/// </summary>
[Table("un_terrains_areas")]
[Comment("Связь рельефа с областями")]
public class TerrainArea : BaseEntity
{
    /// <summary>
    /// Ссылка на рельеф
    /// </summary>
    [Column("terrain_id")]
    [Comment("Ссылка на рельеф")]
    public long TerrainId { get; private set; }

    /// <summary>
    /// Навигационное свойство рельефа
    /// </summary>
    public Terrain Terrain { get; private set; }

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
    /// Пустой конструктор сущности связи рельефа с областями
    /// </summary>
    public TerrainArea() : base()
    {

    }

    /// <summary>
    /// Конструктор сущности связи рельефа с областями
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="terrain"></param>
    /// <param name="area"></param>
    /// <param name="sizeInPixels"></param>
    public TerrainArea(long id, string user, Terrain terrain, Area area, int sizeInPixels) : base(id, user)
    {
        Terrain = terrain;
        TerrainId = terrain.Id;
        Area = area;
        AreaId = area.Id;
        SizeInPixels = sizeInPixels;
    }

    /// <summary>
    /// Конструктор сущности связи рельефа с областями без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="terrain"></param>
    /// <param name="area"></param>
    /// <param name="sizeInPixels"></param>
    public TerrainArea(string user, Terrain terrain, Area area, int sizeInPixels) : base(user)
    {
        Terrain = terrain;
        TerrainId = terrain.Id;
        Area = area;
        AreaId = area.Id;
        SizeInPixels = sizeInPixels;
    }

    /// <summary>
    /// Метод записи рельефа
    /// </summary>
    /// <param name="terrain"></param>
    public void SetTerrain(Terrain terrain)
    {
        Terrain = terrain;
        TerrainId = terrain.Id;
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
}
