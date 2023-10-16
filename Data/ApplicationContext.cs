using Domain.Entities.General.File;
using Domain.Entities.General.System;
using Domain.Entities.Politics;
using Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Domain.Entities.General.File.File;
using Domain.Entities.Sociology;
using Domain.Entities.Geography;
using Domain.Entities.General.Log;
using Domain.Entities.Informations;

namespace Data;

public class ApplicationContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
{
    public DbSet<Parametr> Parametrs { get; set; } //параметры
    public DbSet<Log> Logs { get; set; } //логи
    public DbSet<AccessRight> AccessRights { get; set; } //права доступа
    public DbSet<RoleAcccessRight> RolesAcccessRights { get; set; } //связь ролей с правами доступа
    public DbSet<InformationArticle> InformationArticles { get; set; } //информационные статьи
    public DbSet<InformationArticleDetail> InformationArticlesDetails { get; set; } //детальные части информационных статьей
    public DbSet<FileType> FileTypes { get; set; } //типы файлов
    public DbSet<File> Files { get; set; } //файлы
    public DbSet<FileUser> FilesUsers { get; set; } //связь файлов с пользователями
    public DbSet<FileInformationArticleDetail> FilesInformationArticles { get; set; } //связь файлов с детальными частями информационных статей
    public DbSet<Country> Countries { get; set; } //страны
    public DbSet<Region> Regions { get; set; } //регионы
    public DbSet<Race> Races { get; set; } //расы
    public DbSet<Nation> Nations { get; set; } //нации
    public DbSet<PersonalName> PersonalNames { get; set; } //имена
    public DbSet<LastName> LastNames { get; set; } //фамилии
    public DbSet<PrefixName> PrefixNames { get; set; } //префиксы имён
    public DbSet<NationPersonalName> NationsPersonalNames { get; set; } //связь наций с именами
    public DbSet<NationLastName> NationsLastNames { get; set; } //связь наций с фамилиями
    public DbSet<NationPrefixName> NationsPrefixNames { get; set; } //связь наций с префиксами имён
    public DbSet<Fraction> Fractions { get; set; } //фракции
    public DbSet<Area> Areas { get; set; } //области
    public DbSet<TypeSettlement> TypesSettlements { get; set; } //типы населённых пунктов
    public DbSet<Settlement> Settlements { get; set; } //населённые пункты
    public DbSet<PopulationArea> PopulationAreas { get; set; } //население областей
    public DbSet<PopulationSettlement> PopulationSettlements { get; set; } //население населённых пунктов
    public DbSet<Climate> Climates { get; set; } //климаты
    public DbSet<Terrain> Terrains { get; set; } //рельефы
    public DbSet<ClimateArea> ClimatesAreas { get; set; } //климаты областей
    public DbSet<TerrainArea> TerrainsAreas { get; set; } //рельефы областей

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
        //Создаём базу и накатываем первоначальные таблицы
        Database.Migrate();
    }
}
