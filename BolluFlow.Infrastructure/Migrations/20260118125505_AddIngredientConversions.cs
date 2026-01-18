using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientConversions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "unit_price",
                table: "tb_ingredient",
                newName: "unit_cost");

            migrationBuilder.RenameColumn(
                name: "unit",
                table: "tb_ingredient",
                newName: "base_unit");

            migrationBuilder.CreateTable(
                name: "tb_ingredient_conversion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ingredient_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    quantity_in_base_unit = table.Column<decimal>(type: "numeric", nullable: false),
                    ingredient_id1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tb_ingredient_conversion", x => x.id);
                    table.ForeignKey(
                        name: "fk_tb_ingredient_conversion_tb_ingredient_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "tb_ingredient",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tb_ingredient_conversion_tb_ingredient_ingredient_id1",
                        column: x => x.ingredient_id1,
                        principalTable: "tb_ingredient",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_tb_ingredient_conversion_ingredient_id",
                table: "tb_ingredient_conversion",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "ix_tb_ingredient_conversion_ingredient_id1",
                table: "tb_ingredient_conversion",
                column: "ingredient_id1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_ingredient_conversion");

            migrationBuilder.RenameColumn(
                name: "unit_cost",
                table: "tb_ingredient",
                newName: "unit_price");

            migrationBuilder.RenameColumn(
                name: "base_unit",
                table: "tb_ingredient",
                newName: "unit");
        }
    }
}
