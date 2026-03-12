using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Dashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tb_order_customer_id",
                table: "tb_order");

            migrationBuilder.CreateIndex(
                name: "ix_tb_supply_user_id_minimum_stock_quantity_stock_quantity",
                table: "tb_supply",
                columns: new[] { "user_id", "minimum_stock_quantity", "stock_quantity" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_product_user_id_minimum_stock_quantity_stock_quantity",
                table: "tb_product",
                columns: new[] { "user_id", "minimum_stock_quantity", "stock_quantity" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_order_customer_id_status_created_at",
                table: "tb_order",
                columns: new[] { "customer_id", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_ingredient_user_id_minimum_stock_quantity_stock_quantity",
                table: "tb_ingredient",
                columns: new[] { "user_id", "minimum_stock_quantity", "stock_quantity" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tb_supply_user_id_minimum_stock_quantity_stock_quantity",
                table: "tb_supply");

            migrationBuilder.DropIndex(
                name: "ix_tb_product_user_id_minimum_stock_quantity_stock_quantity",
                table: "tb_product");

            migrationBuilder.DropIndex(
                name: "ix_tb_order_customer_id_status_created_at",
                table: "tb_order");

            migrationBuilder.DropIndex(
                name: "ix_tb_ingredient_user_id_minimum_stock_quantity_stock_quantity",
                table: "tb_ingredient");

            migrationBuilder.CreateIndex(
                name: "ix_tb_order_customer_id",
                table: "tb_order",
                column: "customer_id");
        }
    }
}
