using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class imagetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ThumbnailImageId",
                table: "Hotels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ThumbnailImageId",
                table: "Cities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropColumn(
                name: "ThumbnailImageId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ThumbnailImageId",
                table: "Cities");
        }
    }
}
