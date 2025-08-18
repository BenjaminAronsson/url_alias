using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UlrAlias.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAliasIdAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AliasEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AliasEntries_Alias_ExpiresAt",
                table: "AliasEntries",
                columns: new[] { "Alias", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AliasEntries_ExpiresAt",
                table: "AliasEntries",
                column: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries");

            migrationBuilder.DropIndex(
                name: "IX_AliasEntries_Alias_ExpiresAt",
                table: "AliasEntries");

            migrationBuilder.DropIndex(
                name: "IX_AliasEntries_ExpiresAt",
                table: "AliasEntries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AliasEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AliasEntries",
                table: "AliasEntries",
                column: "Alias");
        }
    }
}
