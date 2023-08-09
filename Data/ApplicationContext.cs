﻿using Domain.Entities.Identification;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Конструктор контекста
    /// </summary>
    /// <param name="options"></param>
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        /*Создаём базу и накатываем первоначальные таблицы*/
        Database.EnsureCreated();
    }
}
