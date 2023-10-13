using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.General.Log;

/// <summary>
/// Сущность логов
/// </summary>
[Table("re_logs")]
[Comment("Логи")]
public class Log : Reestr
{
    /// <summary>
    /// Наименование вызываемого метода
    /// </summary>
    [Column("method")]
    [Comment("Наименование вызываемого метода")]
    public string Method { get; private set; }

    /// <summary>
    /// Тип вызываемого метода
    /// </summary>
    [Column("type")]
    [Comment("Тип вызываемого метода")]
    public string Type { get; private set; }

    /// <summary>
    /// Признак успешного выполнения
    /// </summary>
    [Column("success")]
    [Comment("Признак успешного выполнения")]
    public bool Success { get; private set; }

    /// <summary>
    /// Дата начала
    /// </summary>
    [Column("date_start")]
    [Comment("Дата начала")]
    public DateTime DateStart { get; private set; }

    /// <summary>
    /// Дата окончания
    /// </summary>
    [Column("date_end")]
    [Comment("Дата окончания")]
    public DateTime? DateEnd { get; private set; }

    /// <summary>
    /// Данные на вход
    /// </summary>
    [Column("data_in")]
    [Comment("Данные на вход")]
    public string? DataIn { get; private set; }

    /// <summary>
    /// Данные на выход
    /// </summary>
    [Column("data_out")]
    [Comment("Данные на выход")]
    public string? DataOut { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public Log() : base()
    {

    }

    /// <summary>
    /// Конструктор без id ссущности реестра
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="method"></param>
    /// <param name="type"></param>
    /// <param name="dataIn"></param>
    public Log(string? user, bool isSystem, string method, string type, string dataIn)
        : base(user, isSystem)
    {
        Method = method;
        DataIn = dataIn;
        Type = type;
        DateStart = DateTime.UtcNow;
    }

    /// <summary>
    /// Конструктор модели сущности реестра
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    public Log(long id, string? user, bool isSystem, string method, string type,
        string dataIn) : base(id, user, isSystem)
    {
        Method = method;
        DataIn = dataIn;
        Type = type;
        DateStart = DateTime.UtcNow;
    }

    /// <summary>
    /// Метод записи завершения выполнения
    /// </summary>
    /// <param name="success"></param>
    /// <param name="dataOut"></param>
    public void SetEnd(bool success, string? dataOut)
    {
        Success = success;
        DataOut = dataOut;
        DateEnd = DateTime.UtcNow;
    }
}
