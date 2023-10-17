using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdNewsType_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_re_news_dir_new_types_type_id",
                table: "re_news");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dir_new_types",
                table: "dir_new_types");

            migrationBuilder.RenameTable(
                name: "dir_new_types",
                newName: "dir_news_types");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dir_news_types",
                table: "dir_news_types",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_re_news_dir_news_types_type_id",
                table: "re_news",
                column: "type_id",
                principalTable: "dir_news_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_re_news_dir_news_types_type_id",
                table: "re_news");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dir_news_types",
                table: "dir_news_types");

            migrationBuilder.RenameTable(
                name: "dir_news_types",
                newName: "dir_new_types");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dir_new_types",
                table: "dir_new_types",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_re_news_dir_new_types_type_id",
                table: "re_news",
                column: "type_id",
                principalTable: "dir_new_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
