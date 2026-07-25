using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvTap.Modules.Users.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailOtpVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                schema: "users",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Accounts created before OTP verification existed (including the seeded admin)
            // are grandfathered in as verified so they don't get locked out of login.
            migrationBuilder.Sql("UPDATE users.\"Users\" SET \"IsEmailVerified\" = TRUE;");

            migrationBuilder.CreateTable(
                name: "EmailOtps",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOtps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOtps_Email",
                schema: "users",
                table: "EmailOtps",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailOtps",
                schema: "users");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                schema: "users",
                table: "Users");
        }
    }
}
