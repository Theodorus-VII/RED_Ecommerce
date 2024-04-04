using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    public partial class FourthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Ratings_AspNetUsers_userId",
            //     table: "Ratings");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_Ratings_Products_productId",
            //     table: "Ratings");

            // migrationBuilder.DropColumn(
            //     name: "image",
            //     table: "Products");

            migrationBuilder.RenameColumn(
                name: "review",
                table: "Ratings",
                newName: "Review");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Ratings",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "productId",
                table: "Ratings",
                newName: "ProductId");

            // migrationBuilder.RenameColumn(
            //     name: "rating",
            //     table: "Ratings",
            //     newName: "RatingN");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_userId",
                table: "Ratings",
                newName: "IX_Ratings_UserId");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "details",
                table: "Products",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "count",
                table: "Products",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Products",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "brand",
                table: "Products",
                newName: "Brand");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ProudctId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => new { x.Url, x.ProudctId });
                    table.ForeignKey(
                        name: "FK_Images_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProductId",
                table: "Images",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Products_ProductId",
                table: "Ratings",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Products_ProductId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Review",
                table: "Ratings",
                newName: "review");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Ratings",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Ratings",
                newName: "productId");

            migrationBuilder.RenameColumn(
                name: "RatingN",
                table: "Ratings",
                newName: "rating");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                newName: "IX_Ratings_userId");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Products",
                newName: "details");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Products",
                newName: "count");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Products",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "Products",
                newName: "brand");

            migrationBuilder.AddColumn<string>(
                name: "image",
                table: "Products",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_userId",
                table: "Ratings",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Products_productId",
                table: "Ratings",
                column: "productId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
