using Domain.Entities.General.File;
using Domain.Entities.General.System;
using Domain.Entities.Geography;
using Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Domain.Entities.General.File.File;

namespace Data;

public class ApplicationContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
{
    public DbSet<FileType> FileTypes { get; set; } //типы файлов
    public DbSet<File> Files { get; set; } //файлы
    public DbSet<FileUser> FilesUsers { get; set; } //связь файлов с пользователями
    public DbSet<Parametr> Parametrs { get; set; } //параметры
    public DbSet<Country> Countries { get; set; } //страны
    public DbSet<Region> Regions { get; set; } //регионы

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserRole<long>>().ToTable("sys_users_roles").HasKey(x => new { x.RoleId, x.UserId });
        modelBuilder.Entity<IdentityUser<long>>().ToTable("sys_users");
        modelBuilder.Entity<IdentityRole<long>>().ToTable("sys_roles");
        modelBuilder.Entity<IdentityUserClaim<long>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
        modelBuilder.Entity<IdentityUserLogin<long>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
        modelBuilder.Entity<IdentityRoleClaim<long>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
        modelBuilder.Entity<IdentityUserToken<long>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
    }

    /// <summary>
    /// Конструктор контекста
    /// </summary>
    /// <param name="options"></param>
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        /*Создаём базу и накатываем первоначальные таблицы*/
        Database.Migrate();
    }
}
