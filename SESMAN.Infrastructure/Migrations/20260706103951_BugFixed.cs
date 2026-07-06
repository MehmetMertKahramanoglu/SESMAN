using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SESMAN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BugFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHeaders_HistoryLogs_RequestLogId",
                table: "RequestHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestParameters_HistoryLogs_RequestLogId",
                table: "RequestParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseLogs_HistoryLogs_RequestLogId",
                table: "ResponseLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoryLogs",
                table: "HistoryLogs");

            migrationBuilder.RenameTable(
                name: "HistoryLogs",
                newName: "RequestLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "RequestLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestLogs",
                table: "RequestLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHeaders_RequestLogs_RequestLogId",
                table: "RequestHeaders",
                column: "RequestLogId",
                principalTable: "RequestLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestParameters_RequestLogs_RequestLogId",
                table: "RequestParameters",
                column: "RequestLogId",
                principalTable: "RequestLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseLogs_RequestLogs_RequestLogId",
                table: "ResponseLogs",
                column: "RequestLogId",
                principalTable: "RequestLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHeaders_RequestLogs_RequestLogId",
                table: "RequestHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestParameters_RequestLogs_RequestLogId",
                table: "RequestParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseLogs_RequestLogs_RequestLogId",
                table: "ResponseLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestLogs",
                table: "RequestLogs");

            migrationBuilder.RenameTable(
                name: "RequestLogs",
                newName: "HistoryLogs");

            migrationBuilder.AlterColumn<int>(
                name: "Method",
                table: "HistoryLogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoryLogs",
                table: "HistoryLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHeaders_HistoryLogs_RequestLogId",
                table: "RequestHeaders",
                column: "RequestLogId",
                principalTable: "HistoryLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestParameters_HistoryLogs_RequestLogId",
                table: "RequestParameters",
                column: "RequestLogId",
                principalTable: "HistoryLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseLogs_HistoryLogs_RequestLogId",
                table: "ResponseLogs",
                column: "RequestLogId",
                principalTable: "HistoryLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
