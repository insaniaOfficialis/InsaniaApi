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
                name: "dir_access_rights",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<long>(type: "bigint", nullable: true, comment: "Ссылка на родительский элемент"),
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
                    table.PrimaryKey("PK_dir_access_rights", x => x.id);
                    table.ForeignKey(
                        name: "FK_dir_access_rights_dir_access_rights_parent_id",
                        column: x => x.parent_id,
                        principalTable: "dir_access_rights",
                        principalColumn: "id");
                },
                comment: "Права доступа");

            migrationBuilder.CreateTable(
                name: "dir_climates",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    color = table.Column<string>(type: "text", nullable: false, comment: "Цвет на карте"),
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
                    table.PrimaryKey("PK_dir_climates", x => x.id);
                },
                comment: "Климат");

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
                name: "dir_fractions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("PK_dir_fractions", x => x.id);
                },
                comment: "Фракции");

            migrationBuilder.CreateTable(
                name: "dir_last_names",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("PK_dir_last_names", x => x.id);
                },
                comment: "Фамилии");

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
                name: "dir_personal_names",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gender = table.Column<bool>(type: "boolean", nullable: false, comment: "Пол (истина - мужской/ложь - женский)"),
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
                    table.PrimaryKey("PK_dir_personal_names", x => x.id);
                },
                comment: "Имена");

            migrationBuilder.CreateTable(
                name: "dir_prefixes_name",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("PK_dir_prefixes_name", x => x.id);
                },
                comment: "Префиксы имён");

            migrationBuilder.CreateTable(
                name: "dir_races",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("PK_dir_races", x => x.id);
                },
                comment: "Расы");

            migrationBuilder.CreateTable(
                name: "dir_regions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<int>(type: "integer", nullable: false, comment: "Номер на карте"),
                    color = table.Column<string>(type: "text", nullable: false, comment: "Цвет на карте"),
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
                    table.PrimaryKey("PK_dir_regions", x => x.id);
                },
                comment: "Регионы");

            migrationBuilder.CreateTable(
                name: "dir_terrains",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    color = table.Column<string>(type: "text", nullable: false, comment: "Цвет на карте"),
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
                    table.PrimaryKey("PK_dir_terrains", x => x.id);
                },
                comment: "Рельеф");

            migrationBuilder.CreateTable(
                name: "dir_types_geographical_objects",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    table.PrimaryKey("PK_dir_types_geographical_objects", x => x.id);
                },
                comment: "Типы географических объектов");

            migrationBuilder.CreateTable(
                name: "dir_types_settlements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    minimum_size_in_pixels = table.Column<int>(type: "integer", nullable: false, comment: "Минимальный размер в пикселях"),
                    maximum_size_in_pixels = table.Column<int>(type: "integer", nullable: false, comment: "Максимальный размер в пикселях"),
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
                    table.PrimaryKey("PK_dir_types_settlements", x => x.id);
                },
                comment: "Типы населённых пунктов");

            migrationBuilder.CreateTable(
                name: "re_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    method = table.Column<string>(type: "text", nullable: false, comment: "Наименование вызываемого метода"),
                    type = table.Column<string>(type: "text", nullable: false, comment: "Тип вызываемого метода"),
                    success = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак успешного выполнения"),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата начала"),
                    date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата окончания"),
                    data_in = table.Column<string>(type: "text", nullable: true, comment: "Данные на вход"),
                    data_out = table.Column<string>(type: "text", nullable: true, comment: "Данные на выход"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_logs", x => x.id);
                },
                comment: "Логи");

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
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: true, comment: "Признак заблокированного пользователя"),
                    Gender = table.Column<bool>(type: "boolean", nullable: true, comment: "Пол (истина - мужской/ложь - женский)")
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
                name: "dir_nations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    race_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на расу"),
                    language_for_personal_names = table.Column<string>(type: "text", nullable: true, comment: "Язык для имён"),
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
                    table.PrimaryKey("PK_dir_nations", x => x.id);
                    table.ForeignKey(
                        name: "FK_dir_nations_dir_races_race_id",
                        column: x => x.race_id,
                        principalTable: "dir_races",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Нации");

            migrationBuilder.CreateTable(
                name: "dir_geographical_objects",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на тип географического объекта"),
                    parent_id = table.Column<long>(type: "bigint", nullable: true, comment: "Ссылка на родительский географический объект"),
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
                    table.PrimaryKey("PK_dir_geographical_objects", x => x.id);
                    table.ForeignKey(
                        name: "FK_dir_geographical_objects_dir_geographical_objects_parent_id",
                        column: x => x.parent_id,
                        principalTable: "dir_geographical_objects",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_dir_geographical_objects_dir_types_geographical_objects_typ~",
                        column: x => x.type_id,
                        principalTable: "dir_types_geographical_objects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Географические объекты");

            migrationBuilder.CreateTable(
                name: "un_roles_access_rights",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на роль"),
                    access_right_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на право доступа"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_roles_access_rights", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_roles_access_rights_dir_access_rights_access_right_id",
                        column: x => x.access_right_id,
                        principalTable: "dir_access_rights",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_roles_access_rights_sys_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "sys_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь ролей с правами доступа");

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

            migrationBuilder.CreateTable(
                name: "un_nations_last_names",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    probability = table.Column<double>(type: "double precision", nullable: false, comment: "Вероятность выпадения"),
                    nation_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на нацию"),
                    last_name_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на фамилию"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_nations_last_names", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_nations_last_names_dir_last_names_last_name_id",
                        column: x => x.last_name_id,
                        principalTable: "dir_last_names",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_nations_last_names_dir_nations_nation_id",
                        column: x => x.nation_id,
                        principalTable: "dir_nations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь наций с фамилиями");

            migrationBuilder.CreateTable(
                name: "un_nations_personal_names",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    probability = table.Column<double>(type: "double precision", nullable: false, comment: "Вероятность выпадения"),
                    nation_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на нацию"),
                    personal_name_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на имя"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_nations_personal_names", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_nations_personal_names_dir_nations_nation_id",
                        column: x => x.nation_id,
                        principalTable: "dir_nations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_nations_personal_names_dir_personal_names_personal_name_~",
                        column: x => x.personal_name_id,
                        principalTable: "dir_personal_names",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь наций с именами");

            migrationBuilder.CreateTable(
                name: "un_nations_prefix_names",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    probability = table.Column<double>(type: "double precision", nullable: false, comment: "Вероятность выпадения"),
                    nation_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на нацию"),
                    prefix_name_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на префикс имени"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_nations_prefix_names", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_nations_prefix_names_dir_nations_nation_id",
                        column: x => x.nation_id,
                        principalTable: "dir_nations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_nations_prefix_names_dir_prefixes_name_prefix_name_id",
                        column: x => x.prefix_name_id,
                        principalTable: "dir_prefixes_name",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь наций с префиксами имён");

            migrationBuilder.CreateTable(
                name: "dir_areas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    size_in_pixels = table.Column<int>(type: "integer", nullable: false, comment: "Размер в пикселях"),
                    number = table.Column<int>(type: "integer", nullable: false, comment: "Номер на карте"),
                    color = table.Column<string>(type: "text", nullable: false, comment: "Цвет на карте"),
                    geographical_object_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на географический объект"),
                    country_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на страну"),
                    region_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на регион"),
                    fraction_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на фракцию"),
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
                    table.PrimaryKey("PK_dir_areas", x => x.id);
                    table.ForeignKey(
                        name: "FK_dir_areas_dir_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "dir_countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dir_areas_dir_fractions_fraction_id",
                        column: x => x.fraction_id,
                        principalTable: "dir_fractions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dir_areas_dir_geographical_objects_geographical_object_id",
                        column: x => x.geographical_object_id,
                        principalTable: "dir_geographical_objects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dir_areas_dir_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "dir_regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Области");

            migrationBuilder.CreateTable(
                name: "dir_settlements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    size_in_pixels = table.Column<int>(type: "integer", nullable: false, comment: "Размер в пикселях"),
                    type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на тип населённого пункта"),
                    area_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на географический объект"),
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
                    table.PrimaryKey("PK_dir_settlements", x => x.id);
                    table.ForeignKey(
                        name: "FK_dir_settlements_dir_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "dir_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dir_settlements_dir_types_settlements_type_id",
                        column: x => x.type_id,
                        principalTable: "dir_types_settlements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Населённые пункты");

            migrationBuilder.CreateTable(
                name: "re_population_areas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    area_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на область"),
                    nation_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на область"),
                    quantity = table.Column<int>(type: "integer", nullable: false, comment: "Количество разумных"),
                    gender = table.Column<bool>(type: "boolean", nullable: false, comment: "Пол (истина - мужской/ложь - женский)"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_population_areas", x => x.id);
                    table.ForeignKey(
                        name: "FK_re_population_areas_dir_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "dir_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_re_population_areas_dir_nations_nation_id",
                        column: x => x.nation_id,
                        principalTable: "dir_nations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Население областей");

            migrationBuilder.CreateTable(
                name: "un_climates_areas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    climate_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на климат"),
                    area_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на область"),
                    size_in_pixels = table.Column<int>(type: "integer", nullable: false, comment: "Размер в пикселях"),
                    is_marine = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак морского климата"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_climates_areas", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_climates_areas_dir_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "dir_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_climates_areas_dir_climates_climate_id",
                        column: x => x.climate_id,
                        principalTable: "dir_climates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь климата с областями");

            migrationBuilder.CreateTable(
                name: "un_terrains_areas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    terrain_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на рельеф"),
                    area_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на область"),
                    size_in_pixels = table.Column<int>(type: "integer", nullable: false, comment: "Размер в пикселях"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_terrains_areas", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_terrains_areas_dir_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "dir_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_terrains_areas_dir_terrains_terrain_id",
                        column: x => x.terrain_id,
                        principalTable: "dir_terrains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь рельефа с областями");

            migrationBuilder.CreateTable(
                name: "re_population_settlements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    settlement_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на населённый пуцнкт"),
                    nation_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на область"),
                    quantity = table.Column<int>(type: "integer", nullable: false, comment: "Количество разумных"),
                    gender = table.Column<bool>(type: "boolean", nullable: false, comment: "Пол (истина - мужской/ложь - женский)"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_population_settlements", x => x.id);
                    table.ForeignKey(
                        name: "FK_re_population_settlements_dir_nations_nation_id",
                        column: x => x.nation_id,
                        principalTable: "dir_nations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_re_population_settlements_dir_settlements_settlement_id",
                        column: x => x.settlement_id,
                        principalTable: "dir_settlements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Население населённых пунктов");

            migrationBuilder.CreateIndex(
                name: "IX_dir_access_rights_parent_id",
                table: "dir_access_rights",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_areas_country_id",
                table: "dir_areas",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_areas_fraction_id",
                table: "dir_areas",
                column: "fraction_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_areas_geographical_object_id",
                table: "dir_areas",
                column: "geographical_object_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_areas_region_id",
                table: "dir_areas",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_geographical_objects_parent_id",
                table: "dir_geographical_objects",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_geographical_objects_type_id",
                table: "dir_geographical_objects",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_nations_race_id",
                table: "dir_nations",
                column: "race_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_settlements_area_id",
                table: "dir_settlements",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_dir_settlements_type_id",
                table: "dir_settlements",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_re_files_type_id",
                table: "re_files",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_re_population_areas_area_id",
                table: "re_population_areas",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_re_population_areas_nation_id",
                table: "re_population_areas",
                column: "nation_id");

            migrationBuilder.CreateIndex(
                name: "IX_re_population_settlements_nation_id",
                table: "re_population_settlements",
                column: "nation_id");

            migrationBuilder.CreateIndex(
                name: "IX_re_population_settlements_settlement_id",
                table: "re_population_settlements",
                column: "settlement_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_climates_areas_area_id",
                table: "un_climates_areas",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_climates_areas_climate_id",
                table: "un_climates_areas",
                column: "climate_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_last_names_last_name_id",
                table: "un_nations_last_names",
                column: "last_name_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_last_names_nation_id",
                table: "un_nations_last_names",
                column: "nation_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_personal_names_nation_id",
                table: "un_nations_personal_names",
                column: "nation_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_personal_names_personal_name_id",
                table: "un_nations_personal_names",
                column: "personal_name_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_prefix_names_nation_id",
                table: "un_nations_prefix_names",
                column: "nation_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_prefix_names_prefix_name_id",
                table: "un_nations_prefix_names",
                column: "prefix_name_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_roles_access_rights_access_right_id",
                table: "un_roles_access_rights",
                column: "access_right_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_roles_access_rights_role_id",
                table: "un_roles_access_rights",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_terrains_areas_area_id",
                table: "un_terrains_areas",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_terrains_areas_terrain_id",
                table: "un_terrains_areas",
                column: "terrain_id");

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
                name: "dir_parameters");

            migrationBuilder.DropTable(
                name: "re_logs");

            migrationBuilder.DropTable(
                name: "re_population_areas");

            migrationBuilder.DropTable(
                name: "re_population_settlements");

            migrationBuilder.DropTable(
                name: "sys_users_roles");

            migrationBuilder.DropTable(
                name: "un_climates_areas");

            migrationBuilder.DropTable(
                name: "un_nations_last_names");

            migrationBuilder.DropTable(
                name: "un_nations_personal_names");

            migrationBuilder.DropTable(
                name: "un_nations_prefix_names");

            migrationBuilder.DropTable(
                name: "un_roles_access_rights");

            migrationBuilder.DropTable(
                name: "un_terrains_areas");

            migrationBuilder.DropTable(
                name: "un_users_files");

            migrationBuilder.DropTable(
                name: "dir_settlements");

            migrationBuilder.DropTable(
                name: "dir_climates");

            migrationBuilder.DropTable(
                name: "dir_last_names");

            migrationBuilder.DropTable(
                name: "dir_personal_names");

            migrationBuilder.DropTable(
                name: "dir_nations");

            migrationBuilder.DropTable(
                name: "dir_prefixes_name");

            migrationBuilder.DropTable(
                name: "dir_access_rights");

            migrationBuilder.DropTable(
                name: "sys_roles");

            migrationBuilder.DropTable(
                name: "dir_terrains");

            migrationBuilder.DropTable(
                name: "re_files");

            migrationBuilder.DropTable(
                name: "sys_users");

            migrationBuilder.DropTable(
                name: "dir_areas");

            migrationBuilder.DropTable(
                name: "dir_types_settlements");

            migrationBuilder.DropTable(
                name: "dir_races");

            migrationBuilder.DropTable(
                name: "dir_file_types");

            migrationBuilder.DropTable(
                name: "dir_countries");

            migrationBuilder.DropTable(
                name: "dir_fractions");

            migrationBuilder.DropTable(
                name: "dir_geographical_objects");

            migrationBuilder.DropTable(
                name: "dir_regions");

            migrationBuilder.DropTable(
                name: "dir_types_geographical_objects");
        }
    }
}
