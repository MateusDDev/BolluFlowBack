using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPreviousUnitCostToStockMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "previous_unit_cost",
                table: "tb_stock_movements",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "previous_unit_cost",
                table: "tb_stock_movements");
        }
    }
}
