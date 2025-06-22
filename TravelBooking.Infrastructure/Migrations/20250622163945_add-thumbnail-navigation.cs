using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addthumbnailnavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Hotels_ThumbnailImageId",
                table: "Hotels",
                column: "ThumbnailImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ThumbnailImageId",
                table: "Cities",
                column: "ThumbnailImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Images_ThumbnailImageId",
                table: "Cities",
                column: "ThumbnailImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Images_ThumbnailImageId",
                table: "Hotels",
                column: "ThumbnailImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Images_ThumbnailImageId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Images_ThumbnailImageId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_ThumbnailImageId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Cities_ThumbnailImageId",
                table: "Cities");
        }
    }
}
