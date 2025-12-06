using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolluFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBirthdayColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "birthday",
                table: "tb_customer",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birthday",
                table: "tb_customer");
        }
    }
}
