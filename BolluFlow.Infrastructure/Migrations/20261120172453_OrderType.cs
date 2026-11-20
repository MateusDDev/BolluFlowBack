using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "tb_order",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE tb_order SET type = CASE WHEN is_quick_sale THEN 'DirectSale' ELSE 'CustomOrder' END;");

            migrationBuilder.DropColumn(
                name: "is_quick_sale",
                table: "tb_order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "tb_order");

            migrationBuilder.AddColumn<bool>(
                name: "is_quick_sale",
                table: "tb_order",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
