using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SESMAN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRawType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RawType",
                table: "SavedRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawType",
                table: "RequestLogs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawType",
                table: "SavedRequests");

            migrationBuilder.DropColumn(
                name: "RawType",
                table: "RequestLogs");
        }
    }
}
