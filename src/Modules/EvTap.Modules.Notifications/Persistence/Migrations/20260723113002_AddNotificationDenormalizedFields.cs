using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvTap.Modules.Notifications.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationDenormalizedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListingTitle",
                schema: "notifications",
                table: "Notifications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                schema: "notifications",
                table: "Notifications",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            // Backfill pre-existing rows from the users/listings schemas (same physical database,
            // one-time data fix only — application code must still never query across schemas directly).
            migrationBuilder.Sql(
                """
                UPDATE notifications."Notifications" n
                SET "RecipientEmail" = COALESCE(u."Email", 'unknown@evtap.local')
                FROM users."Users" u
                WHERE n."UserId" = u."Id" AND n."RecipientEmail" = '';
                """);

            migrationBuilder.Sql(
                """
                UPDATE notifications."Notifications" n
                SET "ListingTitle" = COALESCE(l."Title", 'Silinmiş elan')
                FROM listings."Listings" l
                WHERE n."ListingId" = l."Id" AND n."ListingTitle" = '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListingTitle",
                schema: "notifications",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                schema: "notifications",
                table: "Notifications");
        }
    }
}
