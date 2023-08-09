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
[Table("r_roles")]
[Comment("Роли")]
public class Role: IdentityRole<int>
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
}
