using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinancialUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tb_financial_entries_event_key",
                table: "tb_financial_entries");

            migrationBuilder.DropIndex(
                name: "ix_tb_financial_entries_origin_origin_id",
                table: "tb_financial_entries");

            migrationBuilder.DropIndex(
                name: "ix_tb_financial_entries_type_created_at",
                table: "tb_financial_entries");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "tb_financial_entries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "user_stock_unit_preferences",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    item_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    conversion_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_stock_unit_preferences", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entries_user_id_event_key",
                table: "tb_financial_entries",
                columns: new[] { "user_id", "event_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entries_user_id_origin_origin_id",
                table: "tb_financial_entries",
                columns: new[] { "user_id", "origin", "origin_id" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entries_user_id_type_created_at",
                table: "tb_financial_entries",
                columns: new[] { "user_id", "type", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_stock_unit_preferences_user_id_item_type_item_id",
                table: "user_stock_unit_preferences",
                columns: new[] { "user_id", "item_type", "item_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_stock_unit_preferences");

            migrationBuilder.DropIndex(
                name: "ix_tb_financial_entries_user_id_event_key",
                table: "tb_financial_entries");

            migrationBuilder.DropIndex(
                name: "ix_tb_financial_entries_user_id_origin_origin_id",
                table: "tb_financial_entries");

            migrationBuilder.DropIndex(
                name: "ix_tb_financial_entries_user_id_type_created_at",
                table: "tb_financial_entries");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "tb_financial_entries");

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entries_event_key",
                table: "tb_financial_entries",
                column: "event_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entries_origin_origin_id",
                table: "tb_financial_entries",
                columns: new[] { "origin", "origin_id" });

            migrationBuilder.CreateIndex(
                name: "ix_tb_financial_entries_type_created_at",
                table: "tb_financial_entries",
                columns: new[] { "type", "created_at" });
        }
    }
}
