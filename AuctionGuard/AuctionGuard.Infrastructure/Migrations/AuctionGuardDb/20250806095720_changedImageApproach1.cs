using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionGuard.Infrastructure.Migrations.AuctionGuardDb
{
    /// <inheritdoc />
    public partial class changedImageApproach1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "properties");
        }
    }
}
