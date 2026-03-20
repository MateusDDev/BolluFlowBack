using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    public partial class FinancialTimelineAndRecipes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "excluded_at",
                table: "tb_financial_entry",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reference_type",
                table: "tb_financial_entry",
                type: "text",
                nullable: false,
                defaultValue: "None");

            migrationBuilder.CreateTable(
                name: "tb_financial_entry_exclusion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    financial_entry_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    preserve_source = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_financial_entry_exclusion", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_financial_entry_exclusion_tb_financial_entry_financial_entry_id",
                        column: x => x.financial_entry_id,
                        principalTable: "tb_financial_entry",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.DropTable(name: "tb_fixed_account");

            migrationBuilder.CreateTable(
                name: "tb_fixed_account",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    start_month = table.Column<DateOnly>(type: "date", nullable: false),
                    end_month = table.Column<DateOnly>(type: "date", nullable: true),
                    default_status = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_fixed_account", x => x.id);
                });

            migrationBuilder.AddColumn<string>(
                name: "base_unit",
                table: "tb_recipe",
                type: "text",
                nullable: false,
                defaultValue: "unidade");

            migrationBuilder.AddColumn<decimal>(
                name: "stock_quantity",
                table: "tb_recipe",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "tb_recipe_conversion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    quantity_in_base_unit = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "tb_recipe_stock_movement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<long>(type: "bigint", nullable: false),
                    conversion_id = table.Column<long>(type: "bigint", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity_in_base_unit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    production_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_recipe_stock_movement", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_recipe_stock_movement_tb_recipe_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "tb_recipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entry_exclusion_financial_entry_id",
                table: "tb_financial_entry_exclusion",
                column: "financial_entry_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_fixed_account_user_id_start_month_end_month",
                table: "tb_fixed_account",
                columns: new[] { "user_id", "start_month", "end_month" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_recipe_conversion_recipe_id",
                table: "tb_recipe_conversion",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_recipe_stock_movement_recipe_id",
                table: "tb_recipe_stock_movement",
                column: "recipe_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "tb_financial_entry_exclusion");
            migrationBuilder.DropTable(name: "tb_recipe_conversion");
            migrationBuilder.DropTable(name: "tb_recipe_stock_movement");
            migrationBuilder.DropTable(name: "tb_fixed_account");

            migrationBuilder.DropColumn(name: "excluded_at", table: "tb_financial_entry");
            migrationBuilder.DropColumn(name: "reference_type", table: "tb_financial_entry");
            migrationBuilder.DropColumn(name: "base_unit", table: "tb_recipe");
            migrationBuilder.DropColumn(name: "stock_quantity", table: "tb_recipe");

            migrationBuilder.CreateTable(
                name: "tb_fixed_account",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    default_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    due_day = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_fixed_account", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_fixed_account_user_id_is_active",
                table: "tb_fixed_account",
                columns: new[] { "user_id", "is_active" });
        }
    }
}
