using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvTap.Modules.SavedSearches.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "savedsearches");

            migrationBuilder.CreateTable(
                name: "SavedSearches",
                schema: "savedsearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinPrice = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    MaxPrice = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    MinRooms = table.Column<int>(type: "integer", nullable: true),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearches_IsActive",
                schema: "savedsearches",
                table: "SavedSearches",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearches_UserId",
                schema: "savedsearches",
                table: "SavedSearches",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedSearches",
                schema: "savedsearches");
        }
    }
}
