using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SlugAndProductCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "tb_user",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "whatsapp",
                table: "tb_user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "tb_product",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tb_user_slug",
                table: "tb_user",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tb_user_slug",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "whatsapp",
                table: "tb_user");

            migrationBuilder.DropColumn(
                name: "category",
                table: "tb_product");
        }
    }
}
