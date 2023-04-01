using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    public partial class FixAdminPrimaryKey2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_flag",
                columns: table => new
                {
                    admin_flag_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    flag = table.Column<string>(type: "TEXT", nullable: false),
                    negative = table.Column<bool>(type: "INTEGER", nullable: false),
                    admin_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    server_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_flag", x => x.admin_flag_id);
                    table.ForeignKey(
                        name: "FK_admin_flag_admin__admin_server_id__admin_user_id",
                        columns: x => new { x.admin_id, x.server_id },
                        principalTable: "admin",
                        principalColumns: new[] { "user_id", "server_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_flag_admin_id_server_id",
                table: "admin_flag",
                columns: new[] { "admin_id", "server_id" });

            migrationBuilder.CreateIndex(
                name: "IX_admin_flag_flag_admin_id_server_id",
                table: "admin_flag",
                columns: new[] { "flag", "admin_id", "server_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_flag");
        }
    }
}
