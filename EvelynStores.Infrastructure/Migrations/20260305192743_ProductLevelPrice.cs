using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvelynStores.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductLevelPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductLevels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "ProductLevels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductLevels");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "ProductLevels");
        }
    }
}
