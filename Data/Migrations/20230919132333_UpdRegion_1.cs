using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdRegion_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "country_id",
                table: "dir_regions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Ссылка на страну");

            migrationBuilder.CreateIndex(
                name: "IX_dir_regions_country_id",
                table: "dir_regions",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dir_regions_dir_countries_country_id",
                table: "dir_regions",
                column: "country_id",
                principalTable: "dir_countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dir_regions_dir_countries_country_id",
                table: "dir_regions");

            migrationBuilder.DropIndex(
                name: "IX_dir_regions_country_id",
                table: "dir_regions");

            migrationBuilder.DropColumn(
                name: "country_id",
                table: "dir_regions");
        }
    }
}
