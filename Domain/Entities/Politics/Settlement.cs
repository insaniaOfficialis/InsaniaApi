using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Politics;

/// <summary>
/// Сущность населённых пунктов
/// </summary>
[Table("dir_settlements")]
[Comment("Населённые пункты")]
public class Settlement : Directory
{
    /// <summary>
    /// Размер в пикселях
    /// </summary>
    [Column("size_in_pixels")]
    [Comment("Размер в пикселях")]
    public int SizeInPixels { get; private set; }

    /// <summary>
    /// Ссылка на тип населённого пункта
    /// </summary>
    [Column("type_id")]
    [Comment("Ссылка на тип населённого пункта")]
    public long TypeId { get; private set; }

    /// <summary>
    /// Навигационное свойство типа населённого пункта
    /// </summary>
    [ForeignKey("TypeId")]
    public TypeSettlement TypeSettlement { get; private set; }

    /// <summary>
    /// Ссылка на область
    /// </summary>
    [Column("area_id")]
    [Comment("Ссылка на географический объект")]
    public long AreaId { get; private set; }

    /// <summary>
    /// Навигационное свойство области
    /// </summary>
    public Area Area { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности населённых пунктов
    /// </summary>
    public Settlement() : base()
    {
        
    }

    /// <summary>
    /// Конструктор сущности населённых пунктов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="sizeInPixels"></param>
    /// <param name="typeSettlement"></param>
    /// <param name="area"></param>
    public Settlement(long id, string user, string name, int sizeInPixels, TypeSettlement typeSettlement, Area area) : base(id, user, name)
    {
        SizeInPixels = sizeInPixels;
        TypeSettlement = typeSettlement;
        TypeId = typeSettlement.Id;
        Area = area;
        AreaId = area.Id;
    }

    /// <summary>
    /// Конструктор сущности населённых пунктов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="sizeInPixels"></param>
    /// <param name="typeSettlement"></param>
    /// <param name="area"></param>
    public Settlement(string user, string name, int sizeInPixels, TypeSettlement typeSettlement, Area area) : base(user, name)
    {
        SizeInPixels = sizeInPixels;
        TypeSettlement = typeSettlement;
        TypeId = typeSettlement.Id;
        Area = area;
        AreaId = area.Id;
    }

    /// <summary>
    /// Метод записи размера в пикселях
    /// </summary>
    /// <param name="sizeInPixels"></param>
    public void SetSizeInPixels (int sizeInPixels)
    {
        SizeInPixels = sizeInPixels;
    }

    /// <summary>
    /// Метод записи типа населённого пункта
    /// </summary>
    /// <param name="typeSettlement"></param>
    public void SetType(TypeSettlement typeSettlement)
    {
        TypeSettlement = typeSettlement;
        TypeId = typeSettlement.Id;
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
}
