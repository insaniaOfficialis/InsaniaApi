using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущность географических объектов
/// </summary>
[Table("dir_geographical_objects")]
[Comment("Географические объекты")]
public class GeographicalObject : Directory
{
    /// <summary>
    /// Ссылка на тип географического объекта
    /// </summary>
    [Column("type_id")]
    [Comment("Ссылка на тип географического объекта")]
    public long TypeId { get; private set; }

    /// <summary>
    /// Навигационное свойство типа географического объекта
    /// </summary>
    public TypeGeographicalObject Type { get; private set; }

    /// <summary>
    /// Ссылка на родительский географический объект
    /// </summary>
    [Column("parent_id")]
    [Comment("Ссылка на родительский географический объект")]
    public long? ParentId { get; private set; }

    /// <summary>
    /// Навигационное свойство родительского географического объекта
    /// </summary>
    public GeographicalObject? Parent { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности географических объектов
    /// </summary>
    public GeographicalObject() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности географических объектов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    public GeographicalObject(long id, string user, string name, TypeGeographicalObject type, GeographicalObject? parent) : base(id, user, name)
    {
        Type = type;
        TypeId = type.Id;
        Parent = parent;
        ParentId = parent?.Id;
    }

    /// <summary>
    /// Конструктор сущности географических объектов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    public GeographicalObject(string user, string name, TypeGeographicalObject type, GeographicalObject? parent) : base(user, name)
    {
        Type = type;
        TypeId = type.Id;
        Parent = parent;
        ParentId = parent?.Id;
    }
}
