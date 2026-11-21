using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreAddressPickupAndAddressComplement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "store_complement",
                table: "tb_user",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "store_neighborhood",
                table: "tb_user",
                type: "varchar(120)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "store_number",
                table: "tb_user",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "store_street",
                table: "tb_user",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_pickup",
                table: "tb_order",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "complement",
                table: "tb_delivery_address",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "store_complement",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "store_neighborhood",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "store_number",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "store_street",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "is_pickup",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "tb_delivery_address");
        }
    }
}
