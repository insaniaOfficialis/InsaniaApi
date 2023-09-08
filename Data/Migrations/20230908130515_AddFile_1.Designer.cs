﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20230908130515_AddFile_1")]
    partial class AddFile_1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.General.File.File", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Первичный ключ таблицы");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("DateCreate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_create")
                        .HasComment("Дата создания");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_deleted")
                        .HasComment("Дата удаления");

                    b.Property<DateTime>("DateUpdate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_update")
                        .HasComment("Дата обновления");

                    b.Property<string>("Extention")
                        .HasColumnType("text")
                        .HasColumnName("extention")
                        .HasComment("Расширение файла");

                    b.Property<bool>("IsSystem")
                        .HasColumnType("boolean")
                        .HasColumnName("is_system")
                        .HasComment("Признак системной записи");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name")
                        .HasComment("Наименование файла");

                    b.Property<long>("TypeId")
                        .HasColumnType("bigint")
                        .HasColumnName("type_id")
                        .HasComment("Тип файла");

                    b.Property<string>("UserCreate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_create")
                        .HasComment("Пользователь, создавший");

                    b.Property<string>("UserUpdate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_update")
                        .HasComment("Пользователь, обновивший");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("re_files", t =>
                        {
                            t.HasComment("Файлы");
                        });
                });

            modelBuilder.Entity("Domain.Entities.General.File.FileType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Первичный ключ таблицы");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("alias")
                        .HasComment("Английское наименование");

                    b.Property<DateTime>("DateCreate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_create")
                        .HasComment("Дата создания");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_deleted")
                        .HasComment("Дата удаления");

                    b.Property<DateTime>("DateUpdate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_update")
                        .HasComment("Дата обновления");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name")
                        .HasComment("Наименование");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("path")
                        .HasComment("Путь к диретории");

                    b.Property<string>("UserCreate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_create")
                        .HasComment("Пользователь, создавший");

                    b.Property<string>("UserUpdate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_update")
                        .HasComment("Пользователь, обновивший");

                    b.HasKey("Id");

                    b.ToTable("dir_file_types", t =>
                        {
                            t.HasComment("Типы файлов");
                        });
                });

            modelBuilder.Entity("Domain.Entities.General.File.FileUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Первичный ключ таблицы");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("DateCreate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_create")
                        .HasComment("Дата создания");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_deleted")
                        .HasComment("Дата удаления");

                    b.Property<DateTime>("DateUpdate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_update")
                        .HasComment("Дата обновления");

                    b.Property<long>("FileId")
                        .HasColumnType("bigint")
                        .HasColumnName("file_id")
                        .HasComment("Файл");

                    b.Property<string>("UserCreate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_create")
                        .HasComment("Пользователь, создавший");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id")
                        .HasComment("Пользователь");

                    b.Property<string>("UserUpdate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_update")
                        .HasComment("Пользователь, обновивший");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("UserId");

                    b.ToTable("un_users_files", t =>
                        {
                            t.HasComment("Связь файлов с пользователями");
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<long>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("sys_roles", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityRole<long>");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.ToTable("RoleClaims", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser<long>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("sys_users", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser<long>");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.ToTable("UserClaims", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.ToTable("UserLogins", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("RoleId", "UserId");

                    b.ToTable("sys_users_roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.ToTable("UserTokens", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Domain.Entities.Identification.Role", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityRole<long>");

                    b.ToTable(t =>
                        {
                            t.HasComment("Роли");
                        });

                    b.HasDiscriminator().HasValue("Role");
                });

            modelBuilder.Entity("Domain.Entities.Identification.User", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser<long>");

                    b.Property<string>("FirstName")
                        .HasColumnType("text")
                        .HasComment("Имя");

                    b.Property<string>("LastName")
                        .HasColumnType("text")
                        .HasComment("Фамилия");

                    b.Property<string>("Patronymic")
                        .HasColumnType("text")
                        .HasComment("Отчество");

                    b.ToTable(t =>
                        {
                            t.HasComment("Пользователи");
                        });

                    b.HasDiscriminator().HasValue("User");
                });

            modelBuilder.Entity("Domain.Entities.General.File.File", b =>
                {
                    b.HasOne("Domain.Entities.General.File.FileType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Domain.Entities.General.File.FileUser", b =>
                {
                    b.HasOne("Domain.Entities.General.File.File", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}