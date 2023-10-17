using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNews_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dir_new_types",
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
                    table.PrimaryKey("PK_dir_new_types", x => x.id);
                },
                comment: "Типы новостей");

            migrationBuilder.CreateTable(
                name: "re_news",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false, comment: "Заголовок"),
                    introduction = table.Column<string>(type: "text", nullable: false, comment: "Вступление"),
                    type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на тип новости"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_news", x => x.id);
                    table.ForeignKey(
                        name: "FK_re_news_dir_new_types_type_id",
                        column: x => x.type_id,
                        principalTable: "dir_new_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Новости");

            migrationBuilder.CreateTable(
                name: "re_news_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false, comment: "Текст"),
                    news_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на новость"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_news_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_re_news_details_re_news_news_id",
                        column: x => x.news_id,
                        principalTable: "re_news",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Детальная часть новостей");

            migrationBuilder.CreateTable(
                name: "un_files_news_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<long>(type: "bigint", nullable: false, comment: "Файл"),
                    news_detail_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на детальную часть новости"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_files_news_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_files_news_details_re_files_file_id",
                        column: x => x.file_id,
                        principalTable: "re_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_files_news_details_re_news_details_news_detail_id",
                        column: x => x.news_detail_id,
                        principalTable: "re_news_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь файлов с детальными частями новостей");

            migrationBuilder.CreateIndex(
                name: "IX_re_news_type_id",
                table: "re_news",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_re_news_details_news_id",
                table: "re_news_details",
                column: "news_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_files_news_details_file_id",
                table: "un_files_news_details",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_files_news_details_news_detail_id",
                table: "un_files_news_details",
                column: "news_detail_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "un_files_news_details");

            migrationBuilder.DropTable(
                name: "re_news_details");

            migrationBuilder.DropTable(
                name: "re_news");

            migrationBuilder.DropTable(
                name: "dir_new_types");
        }
    }
}
