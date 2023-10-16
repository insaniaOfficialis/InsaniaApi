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
            migrationBuilder.DropForeignKey(
                name: "FK_un_users_files_re_files_file_id",
                table: "un_users_files");

            migrationBuilder.DropForeignKey(
                name: "FK_un_users_files_sys_users_user_id",
                table: "un_users_files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_un_users_files",
                table: "un_users_files");

            migrationBuilder.RenameTable(
                name: "un_users_files",
                newName: "un_files_users");

            migrationBuilder.RenameIndex(
                name: "IX_un_users_files_user_id",
                table: "un_files_users",
                newName: "IX_un_files_users_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_un_users_files_file_id",
                table: "un_files_users",
                newName: "IX_un_files_users_file_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_un_files_users",
                table: "un_files_users",
                column: "id");

            migrationBuilder.CreateTable(
                name: "re_information_articles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false, comment: "Заголовок"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_information_articles", x => x.id);
                },
                comment: "Информационные статьи");

            migrationBuilder.CreateTable(
                name: "re_information_articles_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false, comment: "Текст"),
                    information_article_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на информационную статью"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_information_articles_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_re_information_articles_details_re_information_articles_inf~",
                        column: x => x.information_article_id,
                        principalTable: "re_information_articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Детальная часть информационных статей");

            migrationBuilder.CreateTable(
                name: "un_files_information_articles_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<long>(type: "bigint", nullable: false, comment: "Файл"),
                    information_article_detail_id = table.Column<long>(type: "bigint", nullable: false, comment: "Ссылка на детальную часть информационной статьи"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_un_files_information_articles_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_un_files_information_articles_details_re_files_file_id",
                        column: x => x.file_id,
                        principalTable: "re_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_un_files_information_articles_details_re_information_articl~",
                        column: x => x.information_article_detail_id,
                        principalTable: "re_information_articles_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Связь файлов с детальными частями информационных статей");

            migrationBuilder.CreateIndex(
                name: "IX_re_information_articles_details_information_article_id",
                table: "re_information_articles_details",
                column: "information_article_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_files_information_articles_details_file_id",
                table: "un_files_information_articles_details",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_files_information_articles_details_information_article_d~",
                table: "un_files_information_articles_details",
                column: "information_article_detail_id");

            migrationBuilder.AddForeignKey(
                name: "FK_un_files_users_re_files_file_id",
                table: "un_files_users",
                column: "file_id",
                principalTable: "re_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_un_files_users_sys_users_user_id",
                table: "un_files_users",
                column: "user_id",
                principalTable: "sys_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_un_files_users_re_files_file_id",
                table: "un_files_users");

            migrationBuilder.DropForeignKey(
                name: "FK_un_files_users_sys_users_user_id",
                table: "un_files_users");

            migrationBuilder.DropTable(
                name: "un_files_information_articles_details");

            migrationBuilder.DropTable(
                name: "re_information_articles_details");

            migrationBuilder.DropTable(
                name: "re_information_articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_un_files_users",
                table: "un_files_users");

            migrationBuilder.RenameTable(
                name: "un_files_users",
                newName: "un_users_files");

            migrationBuilder.RenameIndex(
                name: "IX_un_files_users_user_id",
                table: "un_users_files",
                newName: "IX_un_users_files_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_un_files_users_file_id",
                table: "un_users_files",
                newName: "IX_un_users_files_file_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_un_users_files",
                table: "un_users_files",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_un_users_files_re_files_file_id",
                table: "un_users_files",
                column: "file_id",
                principalTable: "re_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_un_users_files_sys_users_user_id",
                table: "un_users_files",
                column: "user_id",
                principalTable: "sys_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
