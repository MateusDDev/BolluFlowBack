using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderOwnerAndQuickSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_order_tb_customer_customer_id",
                table: "tb_order");

            migrationBuilder.AlterColumn<long>(
                name: "customer_id",
                table: "tb_order",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "delivered_at",
                table: "tb_order",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_quick_sale",
                table: "tb_order",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "tb_order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql("UPDATE tb_order o SET user_id = c.user_id FROM tb_customer c WHERE c.id = o.customer_id;");
            migrationBuilder.Sql("UPDATE tb_order o SET delivered_at = COALESCE((SELECT f.created_at FROM tb_financial_entries f WHERE f.event_key = 'order_delivered:' || o.id LIMIT 1), o.delivery_date, o.created_at) WHERE o.status = 'Delivered';");

            migrationBuilder.CreateIndex(
                name: "ix_tb_order_user_id_delivered_at",
                table: "tb_order",
                columns: new[] { "user_id", "delivered_at" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_order_user_id_status_created_at",
                table: "tb_order",
                columns: new[] { "user_id", "status", "created_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_tb_order_tb_customer_customer_id",
                table: "tb_order",
                column: "customer_id",
                principalTable: "tb_customer",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_order_users_user_id",
                table: "tb_order",
                column: "user_id",
                principalTable: "tb_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_order_tb_customer_customer_id",
                table: "tb_order");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_order_users_user_id",
                table: "tb_order");

            migrationBuilder.DropIndex(
                name: "ix_tb_order_user_id_delivered_at",
                table: "tb_order");

            migrationBuilder.DropIndex(
                name: "ix_tb_order_user_id_status_created_at",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "delivered_at",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "is_quick_sale",
                table: "tb_order");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "tb_order");

            migrationBuilder.AlterColumn<long>(
                name: "customer_id",
                table: "tb_order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_order_tb_customer_customer_id",
                table: "tb_order",
                column: "customer_id",
                principalTable: "tb_customer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
