using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
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
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
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
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_admin",
                table: "admin",
                column: "user_id");

            migrationBuilder.CreateTable(
                name: "admin_flag",
                columns: table => new
                {
                    admin_flag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flag = table.Column<string>(type: "text", nullable: false),
                    negative = table.Column<bool>(type: "boolean", nullable: false)
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
