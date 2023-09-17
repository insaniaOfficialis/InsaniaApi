using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dir_countries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<int>(type: "integer", nullable: false, comment: "Номер на карте"),
                    color = table.Column<string>(type: "text", nullable: false, comment: "Цвет на карте"),
                    language_for_names = table.Column<string>(type: "text", nullable: true, comment: "Язык для названий"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Наименование"),
                    alias = table.Column<string>(type: "text", nullable: false, comment: "Английское наименование")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dir_countries", x => x.id);
                },
                comment: "Страны");

            migrationBuilder.CreateTable(
                name: "dir_file_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    path = table.Column<string>(type: "text", nullable: false, comment: "Путь к диретории"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Наименование"),
                    alias = table.Column<string>(type: "text", nullable: false, comment: "Английское наименование")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dir_file_types", x => x.id);
                },
                comment: "Типы файлов");

            migrationBuilder.CreateTable(
                name: "dir_parameters",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<string>(type: "text", nullable: false, comment: "Значение"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Наименование"),
                    alias = table.Column<string>(type: "text", nullable: false, comment: "Английское наименование")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dir_parameters", x => x.id);
                },
                comment: "Параметры");

            migrationBuilder.CreateTable(
                name: "sys_roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    NormalizedName = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_roles", x => x.Id);
                },
                comment: "Роли");

            migrationBuilder.CreateTable(
                name: "sys_users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true, comment: "Фамилия"),
                    FirstName = table.Column<string>(type: "text", nullable: true, comment: "Имя"),
                    Patronymic = table.Column<string>(type: "text", nullable: true, comment: "Отчество"),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: true, comment: "Признак заблокированного пользователя")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_users", x => x.Id);
                },
                comment: "Пользователи");

            migrationBuilder.CreateTable(
                name: "sys_users_roles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_users_roles", x => new { x.RoleId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "re_files",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Наименование файла"),
                    extention = table.Column<string>(type: "text", nullable: true, comment: "Расширение файла"),
                    type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Тип файла"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_re_files_dir_file_types_type_id",
                        column: x => x.type_id,
                        principalTable: "dir_file_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Файлы");

            migrationBuilder.CreateTable(
                name: "un_users_files",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<long>(type: "bigint", nullable: false, comment: "Файл"),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "Пользователь"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_users_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_users_files_re_files_file_id",
                        column: x => x.file_id,
                        principalTable: "re_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_users_files_sys_users_user_id",
                        column: x => x.user_id,
                        principalTable: "sys_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь файлов с пользователями");

            migrationBuilder.CreateIndex(
                name: "IX_re_files_type_id",
                table: "re_files",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_users_files_file_id",
                table: "un_users_files",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_users_files_user_id",
                table: "un_users_files",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dir_countries");

            migrationBuilder.DropTable(
                name: "dir_parameters");

            migrationBuilder.DropTable(
                name: "sys_roles");

            migrationBuilder.DropTable(
                name: "sys_users_roles");

            migrationBuilder.DropTable(
                name: "un_users_files");

            migrationBuilder.DropTable(
                name: "re_files");

            migrationBuilder.DropTable(
                name: "sys_users");

            migrationBuilder.DropTable(
                name: "dir_file_types");
        }
    }
}
