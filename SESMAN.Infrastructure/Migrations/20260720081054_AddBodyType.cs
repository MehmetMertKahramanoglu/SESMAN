using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SESMAN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBodyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedRequests_Collections_CollectionId",
                table: "SavedRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SavedRequests",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "CollectionId",
                table: "SavedRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyType",
                table: "SavedRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyType",
                table: "RequestLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedRequests_Collections_CollectionId",
                table: "SavedRequests",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedRequests_Collections_CollectionId",
                table: "SavedRequests");

            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "SavedRequests");

            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "RequestLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SavedRequests",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CollectionId",
                table: "SavedRequests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedRequests_Collections_CollectionId",
                table: "SavedRequests",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id");
        }
    }
}
