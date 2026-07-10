using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SESMAN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbTableBugFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseLogs_RequestLogs_RequestLogId1",
                table: "ResponseLogs");

            migrationBuilder.DropIndex(
                name: "IX_ResponseLogs_RequestLogId1",
                table: "ResponseLogs");

            migrationBuilder.DropColumn(
                name: "RequestLogId1",
                table: "ResponseLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestLogId1",
                table: "ResponseLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResponseLogs_RequestLogId1",
                table: "ResponseLogs",
                column: "RequestLogId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseLogs_RequestLogs_RequestLogId1",
                table: "ResponseLogs",
                column: "RequestLogId1",
                principalTable: "RequestLogs",
                principalColumn: "Id");
        }
    }
}
