using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    public partial class WhitelistServerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_whitelist",
                table: "whitelist");

            migrationBuilder.AddColumn<int>(
                name: "server_id",
                table: "whitelist",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_whitelist",
                table: "whitelist",
                columns: new[] { "user_id", "server_id" });

            migrationBuilder.CreateIndex(
                name: "IX_whitelist_server_id",
                table: "whitelist",
                column: "server_id");

            migrationBuilder.AddForeignKey(
                name: "FK_whitelist_server_server_id",
                table: "whitelist",
                column: "server_id",
                principalTable: "server",
                principalColumn: "server_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_whitelist_server_server_id",
                table: "whitelist");

            migrationBuilder.DropPrimaryKey(
                name: "PK_whitelist",
                table: "whitelist");

            migrationBuilder.DropIndex(
                name: "IX_whitelist_server_id",
                table: "whitelist");

            migrationBuilder.DropColumn(
                name: "server_id",
                table: "whitelist");

            migrationBuilder.AddPrimaryKey(
                name: "PK_whitelist",
                table: "whitelist",
                column: "user_id");
        }
    }
}
