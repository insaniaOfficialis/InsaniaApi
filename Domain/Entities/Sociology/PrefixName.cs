using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность префиксов имён
/// </summary>
[Table("dir_prefixes_name")]
[Comment("Префиксы имён")]
public class PrefixName : Directory
{
    /// <summary>
    /// Пустой конструктор сущности префиксов имён
    /// </summary>
    public PrefixName() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности префиксов имён
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public PrefixName(long id, string user, string name, bool gender) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности префиксов имён без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public PrefixName(string user, string name) : base(user, name)
    {
    }
}
