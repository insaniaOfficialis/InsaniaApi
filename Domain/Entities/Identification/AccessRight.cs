using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Identification;

/// <summary>
/// Сущность прав доступа
/// </summary>
[Table("dir_access_rights")]
[Comment("Права доступа")]
public class AccessRight : Directory
{
    /// <summary>
    /// Пустой конструктор сущности прав доступа
    /// </summary>
    public AccessRight() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности прав доступа
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public AccessRight(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности прав доступа без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public AccessRight(string user, string name) : base(user, name)
    {
    }
}