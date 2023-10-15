using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Identification.Users.Response;

/// <summary>
/// Модель ответа информации о пользователе
/// </summary>
public class UserInfoResponse : BaseResponse
{
    /// <summary>
    /// Логин
    /// </summary>
    public string? UserName { get; private set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string? FirstName { get; private set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string? LastName { get; private set; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; private set; }

    /// <summary>
    /// Полное имя
    /// </summary>
    public string? FullName { get; private set; }

    /// <summary>
    /// Инициалы
    /// </summary>
    public string? Initials { get; private set; }

    /// <summary>
    /// Пол (истина - мужской/ложь - женский)
    /// </summary>
    public bool Gender { get; private set; }

    /// <summary>
    /// Почта
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Телефон
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// Признак заблокированного пользователя
    /// </summary>
    public bool? IsBlocked { get; private set; }

    /// <summary>
    /// Роли
    /// </summary>
    public List<string>? Roles { get; private set; }

    /// <summary>
    /// Список прав доступа
    /// </summary>
    public List<string>? AccessRights { get; private set; }

    /// <summary>
    /// Конструктор ответа модели информации о пользователе с ошибкой
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public UserInfoResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор модели ответа информации о пользователе
    /// </summary>
    /// <param name="success"></param>
    /// <param name="id"></param>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="patronimyc"></param>
    /// <param name="fullName"></param>
    /// <param name="initials"></param>
    /// <param name="gender"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="isBlockded"></param>
    /// <param name="roles"></param>
    /// <param name="accessRights"></param>
    public UserInfoResponse(bool success, long id, string? userName, string? firstName, string? lastName,
        string? patronimyc, string? fullName, string? initials, bool gender, string? email, string? phoneNumber,
        bool? isBlockded, List<string>? roles, List<string>? accessRights) : base(success, id)
    {
        Id = id;
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronimyc;
        FullName = fullName;
        Initials = initials;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        IsBlocked = isBlockded;
        Roles = roles;
        AccessRights = accessRights;
    }
}