using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Financial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "financial_entries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    origin = table.Column<string>(type: "text", nullable: false),
                    origin_id = table.Column<long>(type: "bigint", nullable: true),
                    event_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_entries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_movements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_type = table.Column<string>(type: "text", nullable: false),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    movement_type = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    total_value = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    event_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stock_movements", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_financial_entries_event_key",
                table: "financial_entries",
                column: "event_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_financial_entries_origin_origin_id",
                table: "financial_entries",
                columns: new[] { "origin", "origin_id" });

            migrationBuilder.CreateIndex(
                name: "ix_financial_entries_type_created_at",
                table: "financial_entries",
                columns: new[] { "type", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movements_event_key",
                table: "stock_movements",
                column: "event_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_movements_item_type_item_id_created_at",
                table: "stock_movements",
                columns: new[] { "item_type", "item_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movements_movement_type_created_at",
                table: "stock_movements",
                columns: new[] { "movement_type", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "financial_entries");

            migrationBuilder.DropTable(
                name: "stock_movements");
        }
    }
}
