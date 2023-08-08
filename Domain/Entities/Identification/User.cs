﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Identification;

/// <summary>
/// Сущность пользователей
/// </summary>
[Table("r_users")]
[Comment("Пользователи")]
public class User : IdentityUser<int>
{
    /// <summary>
    /// Фамилия
    /// </summary>
    [Column("last_name")]
    [Comment("Фамилия")]
    public string? LastName { get; private set; }

    /// <summary>
    /// Имя
    /// </summary>
    [Column("first_name")]
    [Comment("Имя")]
    public string? FirstName { get; private set; }

    /// <summary>
    /// Отчество
    /// </summary>
    [Column("patronymic")]
    [Comment("Отчество")]
    public string? Patronymic { get; private set; }

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
    /// Конструктор со стандартными данными
    /// </summary>
    /// <param name="id"></param>
    /// <param name="login"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    public User(int id, string? login, string? email, string? phone): this()
    {
        Id = id;
        UserName = login;
        Email = email;
        PhoneNumber = phone;
    }

    /// <summary>
    /// Конструктор с ФИО
    /// </summary>
    /// <param name="id"></param>
    /// <param name="login"></param>
    /// <param name="email"></param>
    /// <param name="phone"></param>
    /// <param name="lastName"></param>
    /// <param name="firatName"></param>
    /// <param name="patronymic"></param>
    public User(int id, string? login, string? email, string? phone, string? lastName, string? firatName, string? patronymic) : this(id, login, email, phone)
    {
        LastName = lastName;
        FirstName = firatName;
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
}