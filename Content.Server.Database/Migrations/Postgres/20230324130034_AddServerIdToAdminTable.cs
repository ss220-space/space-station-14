using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    public partial class AddServerIdToAdminTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trait_profile_id",
                table: "trait");

            migrationBuilder.AddColumn<int>(
                name: "server_id",
                table: "admin",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trait_profile_id_trait_name",
                table: "trait",
                columns: new[] { "profile_id", "trait_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin__server_id",
                table: "admin",
                column: "server_id");

            migrationBuilder.AddForeignKey(
                name: "FK_admin_server__server_id",
                table: "admin",
                column: "server_id",
                principalTable: "server",
                principalColumn: "server_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admin_server__server_id",
                table: "admin");

            migrationBuilder.DropIndex(
                name: "IX_trait_profile_id_trait_name",
                table: "trait");

            migrationBuilder.DropIndex(
                name: "IX_admin__server_id",
                table: "admin");

            migrationBuilder.DropColumn(
                name: "server_id",
                table: "admin");

            migrationBuilder.CreateIndex(
                name: "IX_trait_profile_id",
                table: "trait",
                column: "profile_id");
        }
    }
}
