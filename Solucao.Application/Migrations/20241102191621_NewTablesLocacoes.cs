using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class NewTablesLocacoes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Freight",
                table: "Clients",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Clients",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal");

            migrationBuilder.CreateTable(
                name: "ClientSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    SpecificationId = table.Column<Guid>(nullable: false),
                    Hours = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSpecifications_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSpecifications_Specifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalTable: "Specifications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentRelantionships",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRelantionships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    EquipmentRelationshipId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientEquipment_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientEquipment_EquipmentRelantionships_EquipmentRelationshipId",
                        column: x => x.EquipmentRelationshipId,
                        principalTable: "EquipmentRelantionships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentRelationshipEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EquipmentRelationshipId = table.Column<Guid>(nullable: false),
                    EquipmentId = table.Column<Guid>(nullable: false),
                    EquipamentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRelationshipEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentRelationshipEquipment_Equipaments_EquipamentId",
                        column: x => x.EquipamentId,
                        principalTable: "Equipaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentRelationshipEquipment_EquipmentRelantionships_EquipmentRelationshipId",
                        column: x => x.EquipmentRelationshipId,
                        principalTable: "EquipmentRelantionships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Time = table.Column<string>(type: "char(5)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClientEquipmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeValues_ClientEquipment_ClientEquipmentId",
                        column: x => x.ClientEquipmentId,
                        principalTable: "ClientEquipment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientEquipment_ClientId",
                table: "ClientEquipment",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEquipment_EquipmentRelationshipId",
                table: "ClientEquipment",
                column: "EquipmentRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSpecifications_ClientId",
                table: "ClientSpecifications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSpecifications_SpecificationId",
                table: "ClientSpecifications",
                column: "SpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipamentId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipamentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRelationshipEquipment_EquipmentRelationshipId",
                table: "EquipmentRelationshipEquipment",
                column: "EquipmentRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeValues_ClientEquipmentId",
                table: "TimeValues",
                column: "ClientEquipmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientSpecifications");

            migrationBuilder.DropTable(
                name: "EquipmentRelationshipEquipment");

            migrationBuilder.DropTable(
                name: "TimeValues");

            migrationBuilder.DropTable(
                name: "ClientEquipment");

            migrationBuilder.DropTable(
                name: "EquipmentRelantionships");

            migrationBuilder.AlterColumn<decimal>(
                name: "Freight",
                table: "Clients",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Clients",
                type: "decimal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
