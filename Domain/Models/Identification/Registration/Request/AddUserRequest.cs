﻿namespace Domain.Models.Identification.Registration.Request;

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
    public string? LastName { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; set; }
    
    /// <summary>
    /// Признак записи блокировки
    /// </summary>
    public bool? IsBlocked { get; set; }

    /// <summary>
    /// Пол (истина - мужской/ложь - женский)
    /// </summary>
    public bool? Gender { get; set; }

    /// <summary>
    /// Массив ролей
    /// </summary>
    public List<string>? Roles { get; set; }
}
