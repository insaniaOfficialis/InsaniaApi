using Domain.Entities.Identification;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
