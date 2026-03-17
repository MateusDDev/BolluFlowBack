using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    public partial class FinancialBasic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_financial_entry",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    is_fixed = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    is_automatic_from_sale = table.Column<bool>(type: "boolean", nullable: false),
                    source_order_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_financial_entry", x => x.id);
                });

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
                name: "ix_tb_financial_entry_user_id_date",
                table: "tb_financial_entry",
                columns: new[] { "user_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entry_user_id_status",
                table: "tb_financial_entry",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entry_user_id_type",
                table: "tb_financial_entry",
                columns: new[] { "user_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entry_user_id_source_order_id",
                table: "tb_financial_entry",
                columns: new[] { "user_id", "source_order_id" },
                unique: true,
                filter: "source_order_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_tb_fixed_account_user_id_is_active",
                table: "tb_fixed_account",
                columns: new[] { "user_id", "is_active" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "tb_financial_entry");
            migrationBuilder.DropTable(name: "tb_fixed_account");
        }
    }
}
