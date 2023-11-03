using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdInformationArticleId_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ordinal_number",
                table: "re_information_articles_details",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Порядковый номер");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ordinal_number",
                table: "re_information_articles_details");
        }
    }
}
