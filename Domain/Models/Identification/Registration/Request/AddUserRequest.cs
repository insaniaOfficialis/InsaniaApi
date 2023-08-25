namespace Domain.Models.Identification.Registration.Request;

/// <summary>
/// Модель запроса добавления пользователя
/// </summary>
public class AddUserRequest
{
    /// <summary>
    /// Логин
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Почта
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Номер телефона
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string? LastName { get; private set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string? FirstName { get; private set; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; private set; }
}
