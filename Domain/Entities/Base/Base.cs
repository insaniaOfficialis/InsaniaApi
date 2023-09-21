using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Base;

/// <summary>
/// Базовая модель сущностей
/// </summary>
public abstract class Base
{
    /// <summary>
    /// Первичный ключ таблицы
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    [Comment("Первичный ключ таблицы")]
    public long Id { get; private set; }

    /// <summary>
    ///	Дата создания
    /// </summary>
    [Column("date_create")]
    [Comment("Дата создания")]
    public DateTime DateCreate { get; private set; }

    /// <summary>
    ///	Пользователь, создавший
    /// </summary>
    [Column("user_create")]
    [Comment("Пользователь, создавший")]
    public string UserCreate { get; private set; }
    
    /// <summary>
    ///	Дата обновления
    /// </summary>
    [Column("date_update")]
    [Comment("Дата обновления")]
    public DateTime DateUpdate { get; private set; }
    
    /// <summary>
    ///	Пользователь, обновивший
    /// </summary>
    [Column("user_update")]
    [Comment("Пользователь, обновивший")]
    public string UserUpdate { get; private set; }

    /// <summary>
    ///	Дата удаления
    /// </summary>
    [Column("date_deleted")]
    [Comment("Дата удаления")]
    public DateTime? DateDeleted { get; private set; }

    /// <summary>
    ///	Признак удалённой записи
    /// </summary>
    [NotMapped]
    [Column("is_deleted")]
    [Comment("Признак удалённой записи")]
    public bool IsDeleted { get => DateDeleted != null; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public Base()
    {
        
    }

    /// <summary>
    /// Конструктор без id базовой модели сущности
    /// </summary>
    /// <param name="user"></param>
    public Base(string? user): this()
    {
        DateCreate = DateTime.UtcNow;
        UserCreate = user ?? "system";
        DateUpdate = DateTime.UtcNow;
        UserUpdate = user ?? "system";
    }

    /// <summary>
    /// Конструктор базовой модели сущности
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    public Base(long id, string? user): this(user)
    {
        Id = id;
    }

    /// <summary>
    /// Метод записи изменения
    /// </summary>
    /// <param name="dateUpdate"></param>
    /// <param name="userUpdate"></param>
    public void SetUpdate(string? user)
    {
        DateUpdate = DateTime.UtcNow;
        UserUpdate = user ?? "system";
    }

    /// <summary>
    /// Метод удаления
    /// </summary>
    public void SetDeleted()
    {
        DateDeleted = DateTime.UtcNow;
    }

    /// <summary>
    /// Метод восстновления
    /// </summary>
    public void SetRestored()
    {
        DateDeleted = null;
    }
}
