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
    /// Ссылка на родительский элемент
    /// </summary>
    [Column("parent_id")]
    [Comment("Ссылка на родительский элемент")]
    public long? ParentId { get; private set; }

    [ForeignKey("ParentId")]
    public AccessRight? Parent { get; private set; }

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
    /// <param name="parent"></param>
    public AccessRight(long id, string user, string name, AccessRight? parent) : base(id, user, name)
    {
        Parent = parent;
        ParentId = parent?.Id;
    }

    /// <summary>
    /// Конструктор сущности прав доступа без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    public AccessRight(string user, string name, AccessRight? parent) : base(user, name)
    {
        Parent = parent;
        ParentId = parent?.Id;
    }

    /// <summary>
    /// Метод записи родителя
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent (AccessRight parent)
    {
        Parent = parent;
        ParentId = parent.Id;
    }
}