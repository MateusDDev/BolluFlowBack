using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    public partial class RecipeStockProduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "minimum_stock_quantity",
                table: "tb_recipe",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "stock_quantity",
                table: "tb_recipe",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "recipe_productions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_productions", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_productions_tb_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "tb_recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_recipe_conversion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    quantity_in_base_unit = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_recipe_conversion", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_recipe_conversion_tb_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "tb_recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_productions_recipe_id",
                table: "recipe_productions",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_recipe_conversion_recipe_id",
                table: "tb_recipe_conversion",
                column: "recipe_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recipe_productions");

            migrationBuilder.DropTable(
                name: "tb_recipe_conversion");

            migrationBuilder.DropColumn(
                name: "minimum_stock_quantity",
                table: "tb_recipe");

            migrationBuilder.DropColumn(
                name: "stock_quantity",
                table: "tb_recipe");
        }
    }
}
