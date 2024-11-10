using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AlterTableClientSpefication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Hours",
                table: "ClientSpecifications",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "ClientSpecifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "ClientSpecifications");

            migrationBuilder.AlterColumn<int>(
                name: "Hours",
                table: "ClientSpecifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
