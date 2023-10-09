using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущность рельефа
/// </summary>
[Table("dir_terrain")]
[Comment("Рельеф")]
public class Terrain : Directory
{
    /// <summary>
    /// Пустой конструктор сущности рельефа
    /// </summary>
    public Terrain() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности рельефа
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Terrain(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности рельефа без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Terrain(string user, string name) : base(user, name)
    {
    }
}
