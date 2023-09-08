using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.General.File;

/// <summary>
/// Сущность файлов
/// </summary>
[Table("re_files")]
[Comment("Файлы")]
public class File : Reestr
{
    /// <summary>
    /// Наименование файла 
    /// </summary>
    [Column("name")]
    [Comment("Наименование файла")]
    public string Name { get; private set; }

    /// <summary>
    /// Расширение файла
    /// </summary>
    [Column("extention")]
    [Comment("Расширение файла")]
    public string? Extention { get; private set; }

    /// <summary>
    /// Тип файла
    /// </summary>
    [Column("type_id")]
    [Comment("Тип файла")]
    public long TypeId { get; private set; }

    /// <summary>
    /// Навигационное свойство типа файла
    /// </summary>
    [ForeignKey("TypeId")]
    public FileType? Type { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public File(): base()
    {

    }

    /// <summary>
    /// Конструктор без id сущности файлов
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="name"></param>
    /// <param name="typeId"></param>
    public File(string? user, bool isSystem, string name, long typeId) : base(user, isSystem)
    {
        Name = name;
        TypeId = typeId;
        Extention = name[(name.LastIndexOf('.') + 1)..];
    }

    /// <summary>
    /// Конструктор сущности файлов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="name"></param>
    /// <param name="typeId"></param>
    public File(long id, string user, bool isSystem, string name, long typeId):
        base(id, user, isSystem)
    {
        Name = name;
        TypeId = typeId;
        Extention = name[(name.LastIndexOf('.') + 1)..];
    }

    /// <summary>
    /// Метод записи наименования
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        Name = name;
        Extention = name[(name.LastIndexOf('.') + 1)..];
    }

    /// <summary>
    /// Метод записи типа
    /// </summary>
    /// <param name="type"></param>
    public void SetType(FileType type)
    {
        TypeId = type.Id;
        Type = type;
    }
}
