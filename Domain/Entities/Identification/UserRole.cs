using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identification;

/// <summary>
/// Сущность ролей
/// </summary>
[Table("un_users_roles")]
[Comment("Связь пользователей с ролями")]
public class UserRole: IdentityUserRole<int>
{
    /// <summary>
    /// Первчиный ключ таблицы
    /// </summary>
    [Comment("Первичный ключ таблицы")]
    public int Id { get; set; }

    /// <summary>
    /// Навигационное свойство пользователя
    /// </summary>
    public User User { get; set; } = new();
    
    /// <summary>
    /// Навигационное свойство роли
    /// </summary>
    public Role Role { get; set; } = new();

    /// <summary>
    /// Пустой контсруктор
    /// </summary>
    public UserRole()
    {

    }

    /// <summary>
    /// Конструктор с id пользователя и роли
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    public UserRole(int userId, int roleId) : this()
    {
        UserId = userId;
        RoleId = roleId;
    }

    /// <summary>
    /// Конструктор с первичным ключом, id пользователя и роли
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    public UserRole(int id, int userId, int roleId): this(userId, roleId)
    {
        Id = id;
    }
}
