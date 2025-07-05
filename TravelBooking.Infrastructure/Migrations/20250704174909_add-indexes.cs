using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomsTypes_PricePerNight",
                table: "RoomsTypes",
                column: "PricePerNight");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AdultCapacity",
                table: "Rooms",
                column: "AdultCapacity");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AdultCapacity_ChildrenCapacity",
                table: "Rooms",
                columns: new[] { "AdultCapacity", "ChildrenCapacity" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ChildrenCapacity",
                table: "Rooms",
                column: "ChildrenCapacity");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Name",
                table: "Hotels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Name_StarRating",
                table: "Hotels",
                columns: new[] { "Name", "StarRating" });

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_StarRating",
                table: "Hotels",
                column: "StarRating");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_StartDate_EndDate",
                table: "Discounts",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CheckIn_CheckOut",
                table: "Bookings",
                columns: new[] { "CheckIn", "CheckOut" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_RoomsTypes_PricePerNight",
                table: "RoomsTypes");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AdultCapacity",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AdultCapacity_ChildrenCapacity",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ChildrenCapacity",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_Name",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_Name_StarRating",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_StarRating",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_StartDate_EndDate",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Cities_Name",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CheckIn_CheckOut",
                table: "Bookings");
        }
    }
}
