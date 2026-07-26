using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvTap.Modules.Listings.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHour",
                schema: "listings",
                table: "Listings",
                type: "numeric(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                schema: "listings",
                table: "Listings",
                type: "numeric(12,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerHour",
                schema: "listings",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PricePerNight",
                schema: "listings",
                table: "Listings");
        }
    }
}
