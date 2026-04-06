using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderDiscountShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                table: "tb_order",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "discount_type",
                table: "tb_order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "shipping_fee",
                table: "tb_order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discount",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "discount_type",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "shipping_fee",
                table: "tb_order");
        }
    }
}
