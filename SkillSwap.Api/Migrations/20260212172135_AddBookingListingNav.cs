using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSwap.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingListingNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ListingId",
                table: "Bookings",
                column: "ListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Listings_ListingId",
                table: "Bookings",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Listings_ListingId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ListingId",
                table: "Bookings");
        }
    }
}
