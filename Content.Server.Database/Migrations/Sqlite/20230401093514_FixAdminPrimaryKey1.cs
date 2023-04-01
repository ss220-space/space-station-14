using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    public partial class FixAdminPrimaryKey1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admin_server__server_id",
                table: "admin");

            migrationBuilder.DropTable(
                name: "admin_flag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_admin",
                table: "admin");

            migrationBuilder.RenameIndex(
                name: "IX_admin__server_id",
                table: "admin",
                newName: "IX_admin_server_id");

            migrationBuilder.AlterColumn<int>(
                name: "server_id",
                table: "admin",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_admin",
                table: "admin",
                columns: new[] { "user_id", "server_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_admin_server_server_id",
                table: "admin",
                column: "server_id",
                principalTable: "server",
                principalColumn: "server_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admin_server_server_id",
                table: "admin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_admin",
                table: "admin");

            migrationBuilder.RenameIndex(
                name: "IX_admin_server_id",
                table: "admin",
                newName: "IX_admin__server_id");

            migrationBuilder.AlterColumn<int>(
                name: "server_id",
                table: "admin",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_admin",
                table: "admin",
                column: "user_id");

            migrationBuilder.CreateTable(
                name: "admin_flag",
                columns: table => new
                {
                    admin_flag_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    admin_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    flag = table.Column<string>(type: "TEXT", nullable: false),
                    negative = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_flag", x => x.admin_flag_id);
                    table.ForeignKey(
                        name: "FK_admin_flag_admin_admin_id",
                        column: x => x.admin_id,
                        principalTable: "admin",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_flag_admin_id",
                table: "admin_flag",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_flag_flag_admin_id",
                table: "admin_flag",
                columns: new[] { "flag", "admin_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_admin_server__server_id",
                table: "admin",
                column: "server_id",
                principalTable: "server",
                principalColumn: "server_id");
        }
    }
}
