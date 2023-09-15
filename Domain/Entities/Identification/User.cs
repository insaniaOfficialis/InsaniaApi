using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Identification;

/// <summary>
/// Сущность пользователей
/// </summary>
[Comment("Пользователи")]
public class User : IdentityUser<long>
{
    /// <summary>
    /// Фамилия
    /// </summary>
    [Comment("Фамилия")]
    public string? LastName { get; private set; }

    /// <summary>
    /// Имя
    /// </summary>
    [Comment("Имя")]
    public string? FirstName { get; private set; }

    /// <summary>
    /// Отчество
    /// </summary>
    [Comment("Отчество")]
    public string? Patronymic { get; private set; }

    /// <summary>
    /// Признак заблокированного пользователя
    /// </summary>
    [Comment("Признак заблокированного пользователя")]
    public bool IsBlocked { get; private set; }

    /// <summary>
    /// Полное имя
    /// </summary>
    [NotMapped]
    public string? FullName
    {
        get => (!String.IsNullOrEmpty(FirstName) ? (FirstName + " ") : String.Empty) +
            (!String.IsNullOrEmpty(Patronymic) ? (Patronymic + " ") : String.Empty) +
            LastName;
    }

    /// <summary>
    /// Инициалы
    /// </summary>
    [NotMapped]
    public string? Initials 
    { 
        get => (!String.IsNullOrEmpty(FirstName) ? (FirstName[0] + ". ") : String.Empty) + 
            (!String.IsNullOrEmpty(Patronymic) ? (Patronymic[0] + ". ") : String.Empty) + 
            LastName; 
    }

    /// <summary>
    /// Конструктор с пустыми данными
    /// </summary>
    public User()
    {
        
    }

    /// <summary>
    /// Конструктор со стандартными данными без id
    /// </summary>
    /// <param name="login"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <param name="isBlocked"></param>
    public User(string? login, string? email, string? phone, bool isBlocked) : this()
    {
        UserName = login;
        Email = email;
        PhoneNumber = phone;
        IsBlocked = isBlocked;
    }

    /// <summary>
    /// Конструктор со стандартными данными c id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="login"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <param name="isBlocked"></param>
    public User(long id, string? login, string? email, string? phone, bool isBlocked) : this(login, email, phone, isBlocked)
    {
        Id = id;
    }

    /// <summary>
    /// Конструктор с ФИО
    /// </summary>
    /// <param name="id"></param>
    /// <param name="login"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <param name="lastName"></param>
    /// <param name="firstName"></param>
    /// <param name="patronymic"></param>
    /// <param name="isBlocked"></param>
    public User(long id, string? login, string? email, string? phone, bool isBlocked, string? lastName, string? firstName, string? patronymic)
        : this(id, login, email, phone, isBlocked)
    {
        LastName = lastName;
        FirstName = firstName;
        Patronymic = patronymic;
    }

    /// <summary>
    /// Конструктор с ФИО без id
    /// </summary>
    /// <param name="login"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <param name="lastName"></param>
    /// <param name="firstName"></param>
    /// <param name="patronymic"></param>
    /// <param name="isBlocked"></param>
    public User(string? login, string? email, string? phone, bool isBlocked, string? lastName, string? firstName, string? patronymic)
        : this(login, email, phone, isBlocked)
    {
        LastName = lastName;
        FirstName = firstName;
        Patronymic = patronymic;
    }

    /// <summary>
    /// Метод записи почты
    /// </summary>
    /// <param name="email"></param>
    public void SetEmail(string? email)
    {
        Email = email;
    }

    /// <summary>
    /// Метод записи номера телефона
    /// </summary>
    /// <param name="phone"></param>
    public void SetPhone(string? phone)
    {
        PhoneNumber = phone;
    }

    /// <summary>
    /// Метод записи фамилии
    /// </summary>
    /// <param name="lastName"></param>
    public void SetLastName(string? lastName)
    {
        LastName = lastName;
    }

    /// <summary>
    /// Метод записи имени
    /// </summary>
    /// <param name="firstName"></param>
    public void SetFirstName(string? firstName)
    {
        FirstName = firstName;
    }

    /// <summary>
    /// Метод записи отчества
    /// </summary>
    /// <param name="patronymic"></param>
    public void SetPatronymic(string? patronymic)
    {
        Patronymic = patronymic;
    }

    /// <summary>
    /// Метод записи признака блокировки
    /// </summary>
    /// <param name="isBlocked"></param>
    public void SetIsBlocked(bool isBlocked)
    {
        IsBlocked = isBlocked;
    }
}
