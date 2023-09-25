using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unique_number",
                table: "dir_regions");

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
                name: "dir_personal_names",
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
                    table.PrimaryKey("PK_dir_personal_names", x => x.id);
                },
                comment: "Имена");

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
                name: "un_nations_personal_names",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    probability = table.Column<double>(type: "double precision", nullable: false, comment: "Вероятность выпадения"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_nations_personal_names", x => x.id);
                },
                comment: "Связь наций с именами");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dir_areas");

            migrationBuilder.DropTable(
                name: "dir_nations");

            migrationBuilder.DropTable(
                name: "dir_personal_names");

            migrationBuilder.DropTable(
                name: "un_nations_personal_names");

            migrationBuilder.DropTable(
                name: "dir_fractions");

            migrationBuilder.DropTable(
                name: "dir_geographical_objects");

            migrationBuilder.DropTable(
                name: "dir_races");

            migrationBuilder.DropTable(
                name: "dir_types_geographical_objects");

            migrationBuilder.AddColumn<string>(
                name: "unique_number",
                table: "dir_regions",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "Уникальный номер");
        }
    }
}
