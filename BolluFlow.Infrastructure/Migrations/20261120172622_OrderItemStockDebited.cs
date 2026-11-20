using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemStockDebited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "stock_debited",
                table: "tb_order_item",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE tb_order_item i SET stock_debited = TRUE FROM tb_order o, tb_product p WHERE o.id = i.order_id AND p.id = i.product_id AND p.track_stock AND o.status IN ('InProduction', 'Ready', 'Delivered');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stock_debited",
                table: "tb_order_item");
        }
    }
}
