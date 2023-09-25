using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "nation_id",
                table: "un_nations_personal_names",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Ссылка на нацию");

            migrationBuilder.AddColumn<long>(
                name: "personal_name_id",
                table: "un_nations_personal_names",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Ссылка на нацию");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_personal_names_nation_id",
                table: "un_nations_personal_names",
                column: "nation_id");

            migrationBuilder.CreateIndex(
                name: "IX_un_nations_personal_names_personal_name_id",
                table: "un_nations_personal_names",
                column: "personal_name_id");

            migrationBuilder.AddForeignKey(
                name: "FK_un_nations_personal_names_dir_nations_nation_id",
                table: "un_nations_personal_names",
                column: "nation_id",
                principalTable: "dir_nations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_un_nations_personal_names_dir_personal_names_personal_name_~",
                table: "un_nations_personal_names",
                column: "personal_name_id",
                principalTable: "dir_personal_names",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_un_nations_personal_names_dir_nations_nation_id",
                table: "un_nations_personal_names");

            migrationBuilder.DropForeignKey(
                name: "FK_un_nations_personal_names_dir_personal_names_personal_name_~",
                table: "un_nations_personal_names");

            migrationBuilder.DropIndex(
                name: "IX_un_nations_personal_names_nation_id",
                table: "un_nations_personal_names");

            migrationBuilder.DropIndex(
                name: "IX_un_nations_personal_names_personal_name_id",
                table: "un_nations_personal_names");

            migrationBuilder.DropColumn(
                name: "nation_id",
                table: "un_nations_personal_names");

            migrationBuilder.DropColumn(
                name: "personal_name_id",
                table: "un_nations_personal_names");
        }
    }
}
