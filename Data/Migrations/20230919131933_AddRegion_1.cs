using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegion_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dir_regions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<int>(type: "integer", nullable: false, comment: "Номер на карте"),
                    color = table.Column<string>(type: "text", nullable: false, comment: "Цвет на карте"),
                    unique_number = table.Column<string>(type: "text", nullable: false, comment: "Уникальный номер"),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dir_regions");
        }
    }
}
