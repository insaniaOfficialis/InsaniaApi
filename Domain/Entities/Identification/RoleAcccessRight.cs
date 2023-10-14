using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.Identification;

/// <summary>
/// Сущность связи ролей с правами доступа
/// </summary>
[Table("un_roles_access_rights")]
[Comment("Связь ролей с правами доступа")]
public class RoleAcccessRight : BaseEntity
{
    /// <summary>
    /// Ссылка на роль
    /// </summary>
    [Column("role_id")]
    [Comment("Ссылка на роль")]
    public long RoleId { get; private  set; }

    /// <summary>
    /// Навигационное свойство роли
    /// </summary>
    public Role Role { get; private set; }

    /// <summary>
    /// Ссылка на право доступа
    /// </summary>
    [Column("access_right_id")]
    [Comment("Ссылка на право доступа")]
    public long AccessRightId { get; private set; }

    /// <summary>
    /// Навигационное свойство роли
    /// </summary>
    public AccessRight AccessRight { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности связи ролей с правами доступа
    /// </summary>
    public RoleAcccessRight() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности связи ролей с правами доступа
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <param name="accessRight"></param>
    public RoleAcccessRight(long id, string user, Role role, AccessRight accessRight) : base(id, user)
    {
        Role = role;
        RoleId = role.Id;
        AccessRight = accessRight;
        AccessRightId = accessRight.Id;
    }

    /// <summary>
    /// Конструктор сущности связи ролей с правами доступа без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <param name="accessRight"></param>
    public RoleAcccessRight(string user, Role role, AccessRight accessRight) : base(user)
    {
        Role = role;
        RoleId = role.Id;
        AccessRight = accessRight;
        AccessRightId = accessRight.Id;
    }

    /// <summary>
    /// Метод записи ролей
    /// </summary>
    /// <param name="role"></param>
    public void SetRole(Role role)
    {
        Role = role;
        RoleId = role.Id;
    }

    /// <summary>
    /// Метод записи права доступа
    /// </summary>
    /// <param name="accessRight"></param>
    public void SetAccessRight(AccessRight accessRight)
    {
        AccessRight = accessRight;
        AccessRightId = accessRight.Id;
    }
}
