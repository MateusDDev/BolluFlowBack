using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuDeliveryTimePixKeyAndChangeFor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "delivery_time_minutes",
                table: "tb_user",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pix_key",
                table: "tb_user",
                type: "varchar(140)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "change_for",
                table: "tb_order",
                type: "numeric(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "delivery_time_minutes",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "pix_key",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "change_for",
                table: "tb_order");
        }
    }
}
