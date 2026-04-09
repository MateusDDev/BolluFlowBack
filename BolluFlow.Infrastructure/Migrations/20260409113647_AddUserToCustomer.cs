using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_tb_customer_user_id",
                table: "tb_customer",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tb_customer_users_user_id",
                table: "tb_customer",
                column: "user_id",
                principalTable: "tb_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tb_customer_users_user_id",
                table: "tb_customer");

            migrationBuilder.DropIndex(
                name: "ix_tb_customer_user_id",
                table: "tb_customer");
        }
    }
}
