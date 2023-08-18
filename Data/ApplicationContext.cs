using Domain.Entities.Identification;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<UserRole> UsersRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
