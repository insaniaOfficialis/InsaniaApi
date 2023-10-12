using Domain.Models.Base;
using System.Text.Json.Serialization;

namespace Domain.Models.Sociology.Names;

/// <summary>
/// Модель сгенерированного имени
/// </summary>
public class GeneratedName : BaseResponse
{
    /// <summary>
    /// Пустой конструктор модели сгенерированного имени
    /// </summary>
    public GeneratedName() : base()
    {

    }

    /// <summary>
    /// Конструктор модели сгенерированного имени с ошибкой
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GeneratedName(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор модели сгенерированного имени с личным именем
    /// </summary>
    /// <param name="success"></param>
    /// <param name="personalName"></param>
    public GeneratedName(bool success, string personalName) : base(success) 
    {
        PersonalName = personalName;
    }

    /// <summary>
    /// Конструктор модели сгенерированного имени с личным именем и фамилией
    /// </summary>
    /// <param name="success"></param>
    /// <param name="personalName"></param>
    /// <param name="lastName"></param>
    public GeneratedName(bool success, string personalName, string? lastName) : base(success)
    {
        PersonalName = personalName;
        LastName = lastName;
    }

    /// <summary>
    /// Конструктор модели сгенерированного имени с личным именем, префиксом и фамилией
    /// </summary>
    /// <param name="success"></param>
    /// <param name="personalName"></param>
    /// <param name="prefix"></param>
    /// <param name="lastName"></param>
    public GeneratedName(bool success, string personalName, string? prefix, string? lastName) : base(success)
    {
        PersonalName = personalName;
        Prefix = prefix;
        LastName = lastName;
    }

    /// <summary>
    /// Личное имя
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PersonalName { get; set; }

    /// <summary>
    /// Префикс имён
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Prefix { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; set; }
}
