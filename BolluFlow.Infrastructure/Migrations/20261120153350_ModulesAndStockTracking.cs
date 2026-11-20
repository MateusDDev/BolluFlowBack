using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModulesAndStockTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "allow_negative_stock",
                table: "tb_user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "enabled_modules",
                table: "tb_user",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "onboarding_completed_at",
                table: "tb_user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "track_stock",
                table: "tb_supply",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "track_stock",
                table: "tb_recipe",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "track_stock",
                table: "tb_product",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "track_stock",
                table: "tb_ingredient",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE tb_ingredient SET track_stock = stock_quantity > 0;");
            migrationBuilder.Sql("UPDATE tb_supply SET track_stock = stock_quantity > 0;");
            migrationBuilder.Sql("UPDATE tb_recipe SET track_stock = stock_quantity > 0;");
            migrationBuilder.Sql("UPDATE tb_product SET track_stock = stock_quantity > 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allow_negative_stock",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "enabled_modules",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "onboarding_completed_at",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "track_stock",
                table: "tb_supply");

            migrationBuilder.DropColumn(
                name: "track_stock",
                table: "tb_recipe");

            migrationBuilder.DropColumn(
                name: "track_stock",
                table: "tb_product");

            migrationBuilder.DropColumn(
                name: "track_stock",
                table: "tb_ingredient");
        }
    }
}
