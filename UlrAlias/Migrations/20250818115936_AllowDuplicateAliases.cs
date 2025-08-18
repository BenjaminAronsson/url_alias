using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UlrAlias.Migrations
{
    /// <inheritdoc />
    public partial class AllowDuplicateAliases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AliasEntries_Alias",
                table: "AliasEntries");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "AliasEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AliasEntries_Alias",
                table: "AliasEntries",
                column: "Alias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AliasEntries_Alias",
                table: "AliasEntries");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpiresAt",
                table: "AliasEntries",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AliasEntries_Alias",
                table: "AliasEntries",
                column: "Alias",
                unique: true);
        }
    }
}
