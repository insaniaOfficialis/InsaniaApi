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
[Comment("Роли")]
public class Role: IdentityRole<long>
{
    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public Role()
    {
        
    }

    /// <summary>
    /// Конструктор с наименованием
    /// </summary>
    /// <param name="name"></param>
    public Role(string name): this()
    {
        Name = name;
    }

    /// <summary>
    /// Конструктор с id и наименованием
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public Role(long id, string name): this(name)
    {
        Id = id;
    }

    /// <summary>
    /// Метод записи наименования
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        Name = name;
    }
}
