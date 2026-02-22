using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvelynStores.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReOrderLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReOrderLevel",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReOrderLevel",
                table: "Products");
        }
    }
}
