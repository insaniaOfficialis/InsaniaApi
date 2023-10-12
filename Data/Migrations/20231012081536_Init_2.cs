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

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_prefix_names_nation_id",
                table: "un_nations_prefix_names",
                column: "nation_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_prefix_names_prefix_name_id",
                table: "un_nations_prefix_names",
                column: "prefix_name_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "un_nations_prefix_names");
        }
    }
}
