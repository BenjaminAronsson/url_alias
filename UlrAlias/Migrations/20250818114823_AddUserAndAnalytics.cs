using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UlrAlias.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AliasEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "AliasEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AliasEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AliasEntries_Alias",
                table: "AliasEntries",
                column: "Alias",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries");

            migrationBuilder.DropIndex(
                name: "IX_AliasEntries_Alias",
                table: "AliasEntries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AliasEntries");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "AliasEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AliasEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries",
                column: "Alias");
        }
    }
}
