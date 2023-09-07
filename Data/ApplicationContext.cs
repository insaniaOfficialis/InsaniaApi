using Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("un_users_roles").HasKey(x => new { x.RoleId, x.UserId });
        modelBuilder.Entity<IdentityUser<int>>().ToTable("r_users");
        modelBuilder.Entity<IdentityRole<int>>().ToTable("r_roles");
        modelBuilder.Entity<IdentityUserClaim<int>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
        modelBuilder.Entity<IdentityUserLogin<int>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
        modelBuilder.Entity<IdentityRoleClaim<int>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
        modelBuilder.Entity<IdentityUserToken<int>>().HasNoKey().Metadata.SetIsTableExcludedFromMigrations(true);
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
