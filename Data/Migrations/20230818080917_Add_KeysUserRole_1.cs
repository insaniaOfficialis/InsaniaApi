using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_KeysUserRole_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_un_users_roles_RoleId",
                table: "un_users_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_un_users_roles_UserId",
                table: "un_users_roles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_un_users_roles_r_roles_RoleId",
                table: "un_users_roles",
                column: "RoleId",
                principalTable: "r_roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_un_users_roles_r_users_UserId",
                table: "un_users_roles",
                column: "UserId",
                principalTable: "r_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_un_users_roles_r_roles_RoleId",
                table: "un_users_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_un_users_roles_r_users_UserId",
                table: "un_users_roles");

            migrationBuilder.DropIndex(
                name: "IX_un_users_roles_RoleId",
                table: "un_users_roles");

            migrationBuilder.DropIndex(
                name: "IX_un_users_roles_UserId",
                table: "un_users_roles");
        }
    }
}
