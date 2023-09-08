using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Base;

/// <summary>
/// Модель сущности реестра
/// </summary>
public abstract class Reestr : Base
{
    /// <summary>
    ///	Признак системной записи
    /// </summary>
    [Column("is_system")]
    [Comment("Признак системной записи")]
    public bool IsSystem { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public Reestr(): base()
    {

    }

    /// <summary>
    /// Конструктор без id ссущности реестра
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    public Reestr(string? user, bool isSystem): base(user)
    {
        IsSystem = isSystem;
    }

    /// <summary>
    /// Конструктор модели сущности реестра
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    public Reestr(long id, string? user, bool isSystem): 
        base(id, user)
    {
        IsSystem = isSystem;
    }

    /// <summary>
    /// Метод записи признака системности записи
    /// </summary>
    /// <param name="isSystem"></param>
    public void SetIsSystem(bool isSystem)
    { 
        IsSystem = isSystem; 
    }
}
