using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущность типов географических объектов
/// </summary>
[Table("dir_types_geographical_objects")]
[Comment("Типы географических объектов")]
public class TypeGeographicalObject : Directory
{
    /// <summary>
    /// Пустой конструктор сущности типов географических объектов
    /// </summary>
    public TypeGeographicalObject() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности типов географических объектов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public TypeGeographicalObject(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности типов географических объектов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public TypeGeographicalObject(string user, string name) : base(user, name)
    {
    }
}
