using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SESMAN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ResponseLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ResponseHeaders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RequestParameters",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RequestLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RequestHeaders",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ResponseLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ResponseHeaders");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RequestParameters");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RequestLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RequestHeaders");
        }
    }
}
