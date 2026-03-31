using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    public partial class RemoveStockQuantityFromProductComponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stock_quantity",
                table: "tb_product_component");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "stock_quantity",
                table: "tb_product_component",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
