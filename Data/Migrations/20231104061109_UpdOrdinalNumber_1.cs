using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdOrdinalNumber_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ordinal_number",
                table: "un_files_news_details",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Порядковый номер");

            migrationBuilder.AddColumn<long>(
                name: "ordinal_number",
                table: "un_files_information_articles_details",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Порядковый номер");

            migrationBuilder.AddColumn<long>(
                name: "ordinal_number",
                table: "re_news_details",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Порядковый номер");

            migrationBuilder.AddColumn<long>(
                name: "ordinal_number",
                table: "re_news",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Порядковый номер");

            migrationBuilder.AddColumn<long>(
                name: "ordinal_number",
                table: "re_information_articles",
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
                table: "un_files_news_details");

            migrationBuilder.DropColumn(
                name: "ordinal_number",
                table: "un_files_information_articles_details");

            migrationBuilder.DropColumn(
                name: "ordinal_number",
                table: "re_news_details");

            migrationBuilder.DropColumn(
                name: "ordinal_number",
                table: "re_news");

            migrationBuilder.DropColumn(
                name: "ordinal_number",
                table: "re_information_articles");
        }
    }
}
