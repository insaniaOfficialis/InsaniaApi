using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdLog_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "out",
                table: "re_logs",
                newName: "data_out");

            migrationBuilder.RenameColumn(
                name: "in",
                table: "re_logs",
                newName: "data_in");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "data_out",
                table: "re_logs",
                newName: "out");

            migrationBuilder.RenameColumn(
                name: "data_in",
                table: "re_logs",
                newName: "in");
        }
    }
}
