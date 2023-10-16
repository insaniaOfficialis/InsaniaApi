using Domain.Entities.Identification;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.General.File;

/// <summary>
/// Сущности связи файлов с пользователями
/// </summary>
[Table("un_files_users")]
[Comment("Связь файлов с пользователями")]
public class FileUser : BaseEntity
{
    /// <summary>
    /// Файл
    /// </summary>
    [Column("file_id")]
    [Comment("Файл")]
    public long FileId { get; private set; }

    /// <summary>
    /// Навигационное свойство файла
    /// </summary>
    [ForeignKey("FileId")]
    public File? File { get; private set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    [Column("user_id")]
    [Comment("Пользователь")]
    public long UserId { get; private set; }

    /// <summary>
    /// Навигационное свойство пользователя
    /// </summary>
    [ForeignKey("UserId")]
    public User? User { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public FileUser(): base()
    {

    }

    /// <summary>
    /// Конструктор без id сущности связт файла с пользователем
    /// </summary>
    /// <param name="user"></param>
    /// <param name="file"></param>
    /// <param name="userEntity"></param>
    public FileUser(string? user, File file, User userEntity): base(user)
    {
        File = file;
        FileId = file.Id;
        User = userEntity;
        UserId = userEntity.Id;
    }

    /// <summary>
    /// Конструктор сущности связи файла с пользователем
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="file"></param>
    /// <param name="userEntity"></param>
    public FileUser(long id, string user, File file, User userEntity): base(id, user)
    {
        File = file;
        FileId = file.Id;
        User = userEntity;
        UserId = userEntity.Id;
    }

    /// <summary>
    /// Метод записи файла
    /// </summary>
    /// <param name="file"></param>
    public void SetFile(File file)
    {
        FileId = file.Id;
        File = file;
    }

    /// <summary>
    /// Метод записи пользователя
    /// </summary>
    /// <param name="user"></param>
    public void SetUser(User user)
    {
        UserId = user.Id;
        User = user;
    }
}
