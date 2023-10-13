using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLog_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "re_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Первичный ключ таблицы")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    method = table.Column<string>(type: "text", nullable: false, comment: "Наименование вызываемого метода"),
                    type = table.Column<string>(type: "text", nullable: false, comment: "Тип вызываемого метода"),
                    success = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак успешного выполнения"),
                    date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата начала"),
                    dateEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата окончания"),
                    @in = table.Column<string>(name: "in", type: "text", nullable: true, comment: "Данные на вход"),
                    @out = table.Column<string>(name: "out", type: "text", nullable: true, comment: "Данные на выход"),
                    date_create = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата создания"),
                    user_create = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, создавший"),
                    date_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата обновления"),
                    user_update = table.Column<string>(type: "text", nullable: false, comment: "Пользователь, обновивший"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Дата удаления"),
                    is_system = table.Column<bool>(type: "boolean", nullable: false, comment: "Признак системной записи")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_re_logs", x => x.id);
                },
                comment: "Логи");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "re_logs");
        }
    }
}
