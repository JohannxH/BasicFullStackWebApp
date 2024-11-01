using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasicFullStackWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Tokens",
                newName: "TokenString");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "Tokens",
                newName: "IssuedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Tokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "Tokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tokens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$2d1BFhO78r1NVIINApHC4uLyvpoh/lysA9mphwbVTj7gm8sCB38Hi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$qlJYSoU/N9.VUXqRXE6kNuTSjkqwiDx3l2Loy6Yo3olfY3kLqCZWi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tokens");

            migrationBuilder.RenameColumn(
                name: "TokenString",
                table: "Tokens",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "IssuedAt",
                table: "Tokens",
                newName: "ExpiryDate");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$wGIFzUM6ezrvnG9YiJ1Q3ufpCKcpwhIGg8G2oINAhPVCNJnwHfEyq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$pproU5bzdFPQ9x/n2weP8e8GRIZcsTr5osLRLwvinXNY7E/ScYwpK");
        }
    }
}
