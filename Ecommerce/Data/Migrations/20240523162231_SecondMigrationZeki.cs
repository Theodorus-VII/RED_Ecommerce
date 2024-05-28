using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigrationZeki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "PaymentInfos",
                type: "double",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "PaymentInfos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "PaymentInfos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PaymentInfos");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "PaymentInfos");

            migrationBuilder.AlterColumn<float>(
                name: "Amount",
                table: "PaymentInfos",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");
        }
    }
}
