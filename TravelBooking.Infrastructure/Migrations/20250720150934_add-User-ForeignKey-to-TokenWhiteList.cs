using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUserForeignKeytoTokenWhiteList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TokenWhiteList_UserId",
                table: "TokenWhiteList",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenWhiteList_Users_UserId",
                table: "TokenWhiteList",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenWhiteList_Users_UserId",
                table: "TokenWhiteList");

            migrationBuilder.DropIndex(
                name: "IX_TokenWhiteList_UserId",
                table: "TokenWhiteList");
        }
    }
}
