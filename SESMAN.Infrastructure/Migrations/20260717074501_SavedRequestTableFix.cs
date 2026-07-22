using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SESMAN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SavedRequestTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SavedRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SavedRequests");
        }
    }
}
