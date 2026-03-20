using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    public partial class Finance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_fixed_cost",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    start_month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    end_month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("pk_tb_fixed_cost", x => x.id); });

            migrationBuilder.CreateTable(
                name: "tb_purchase",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_type = table.Column<string>(type: "text", nullable: false),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    item_name_snapshot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    conversion_name_snapshot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    quantity_in_base_unit = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    purchased_at = table.Column<DateOnly>(type: "date", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("pk_tb_purchase", x => x.id); });

            migrationBuilder.CreateTable(
                name: "tb_financial_transaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    source_type = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    event_date = table.Column<DateOnly>(type: "date", nullable: false),
                    month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    source_entity_type = table.Column<string>(type: "text", nullable: false),
                    source_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_automatic = table.Column<bool>(type: "boolean", nullable: false),
                    metadata = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("pk_tb_financial_transaction", x => x.id); });

            migrationBuilder.CreateIndex(
                name: "ix_tb_fixed_cost_start_month_end_month_deleted_at",
                table: "tb_fixed_cost",
                columns: new[] { "start_month", "end_month", "deleted_at" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_purchase_item_type_item_id_deleted_at",
                table: "tb_purchase",
                columns: new[] { "item_type", "item_id", "deleted_at" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_transaction_month_type_deleted_at",
                table: "tb_financial_transaction",
                columns: new[] { "month", "type", "deleted_at" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_transaction_source_entity_type_source_entity_id",
                table: "tb_financial_transaction",
                columns: new[] { "source_entity_type", "source_entity_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "tb_financial_transaction");
            migrationBuilder.DropTable(name: "tb_fixed_cost");
            migrationBuilder.DropTable(name: "tb_purchase");
        }
    }
}
