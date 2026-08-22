using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GroceryShopping.Migrations
{
    /// <inheritdoc />
    public partial class SeedFreshMartProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Fresh and delicious fruits", "Fruits" },
                    { 2, "Fresh and healthy vegetables", "Vegetables" },
                    { 3, "Milk and dairy products", "Dairy" },
                    { 4, "Fresh bakery products", "Bakery" },
                    { 5, "Tasty snacks and packaged foods", "Snacks" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsAvailable", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 1, 1, "Fresh red apples", "/images/products/apples.jpg", true, "Fresh Apples", 180m, 50 },
                    { 2, 1, "Fresh ripe bananas", "/images/products/bananas.jpg", true, "Fresh Bananas", 60m, 80 },
                    { 3, 2, "Fresh potatoes", "/images/products/potatoes.jpg", true, "Potatoes", 40m, 100 },
                    { 4, 3, "Full cream fresh milk", "/images/products/milk.jpg", true, "Fresh Milk", 60m, 50 },
                    { 5, 4, "Healthy brown bread", "/images/products/bread.jpg", true, "Brown Bread", 50m, 40 },
                    { 6, 5, "Crispy potato chips", "/images/products/chips.jpg", true, "Potato Chips", 30m, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
