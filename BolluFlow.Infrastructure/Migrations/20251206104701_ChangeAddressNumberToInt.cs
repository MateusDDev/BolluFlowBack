using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAddressNumberToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "number",
                table: "tb_delivery_address");

            migrationBuilder.AddColumn<int>(
                name: "number",
                table: "tb_delivery_address",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "number",
                table: "tb_delivery_address");

            migrationBuilder.AddColumn<string>(
                name: "number",
                table: "tb_delivery_address",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
