using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SESMAN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth",
                table: "SavedRequests",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auth",
                table: "RequestLogs",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth",
                table: "SavedRequests");

            migrationBuilder.DropColumn(
                name: "Auth",
                table: "RequestLogs");
        }
    }
}
