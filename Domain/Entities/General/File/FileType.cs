using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.General.File;

/// <summary>
/// Сущность типов файлов
/// </summary>
[Table("dir_file_types")]
[Comment("Типы файлов")]
public class FileType : Directory
{
    /// <summary>
    /// Путь к директории
    /// </summary>
    [Column("path")]
    [Comment("Путь к диретории")]
    public string Path { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public FileType()
    {
        
    }

    /// <summary>
    /// Конструктор сущности типов файлов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public FileType(string user, string name, string path) : base(user, name)
    {
        Path = path;
    }
    
    /// <summary>
    /// Конструктор сущности типов файлов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public FileType(long id, string user, string name, string path) : base(id, user, name)
    {
        Path = path;
    }

    /// <summary>
    /// Метод записи пути к диретории
    /// </summary>
    /// <param name="path"></param>
    public void SetPath(string path)
    {
        Path = path;
    }
}
