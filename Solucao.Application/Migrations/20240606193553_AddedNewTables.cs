using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Solucao.Application.Migrations
{
    public partial class AddedNewTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "People",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalendarSpecificationConsumables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CalendarId = table.Column<Guid>(nullable: false),
                    SpecificationId = table.Column<Guid>(nullable: false),
                    Initial = table.Column<int>(nullable: false, defaultValue: 0),
                    Final = table.Column<int>(nullable: false, defaultValue: 0),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSpecificationConsumables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarSpecificationConsumables_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarSpecificationConsumables_Specifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalTable: "Specifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consumables",
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
                    table.PrimaryKey("PK_Consumables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TableName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RecordId = table.Column<Guid>(nullable: false),
                    Operation = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    OperationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Message = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Histories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEquipamentConsumables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<int>(nullable: false, defaultValue: 0),
                    CalendarId = table.Column<Guid>(nullable: false),
                    ConsumableId = table.Column<Guid>(nullable: false),
                    EquipamentId = table.Column<Guid>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEquipamentConsumables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEquipamentConsumables_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarEquipamentConsumables_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarEquipamentConsumables_Equipaments_EquipamentId",
                        column: x => x.EquipamentId,
                        principalTable: "Equipaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipamentConsumables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ConsumableId = table.Column<Guid>(nullable: false),
                    EquipamentId = table.Column<Guid>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipamentConsumables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipamentConsumables_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipamentConsumables_Equipaments_EquipamentId",
                        column: x => x.EquipamentId,
                        principalTable: "Equipaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_People_UserId",
                table: "People",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEquipamentConsumables_CalendarId",
                table: "CalendarEquipamentConsumables",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEquipamentConsumables_ConsumableId",
                table: "CalendarEquipamentConsumables",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEquipamentConsumables_EquipamentId",
                table: "CalendarEquipamentConsumables",
                column: "EquipamentId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSpecificationConsumables_CalendarId",
                table: "CalendarSpecificationConsumables",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSpecificationConsumables_SpecificationId",
                table: "CalendarSpecificationConsumables",
                column: "SpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentConsumables_ConsumableId",
                table: "EquipamentConsumables",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentConsumables_EquipamentId",
                table: "EquipamentConsumables",
                column: "EquipamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_UserId",
                table: "Histories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Users_UserId",
                table: "People",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Users_UserId",
                table: "People");

            migrationBuilder.DropTable(
                name: "CalendarEquipamentConsumables");

            migrationBuilder.DropTable(
                name: "CalendarSpecificationConsumables");

            migrationBuilder.DropTable(
                name: "EquipamentConsumables");

            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropTable(
                name: "Consumables");

            migrationBuilder.DropIndex(
                name: "IX_People_UserId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "People");
        }
    }
}
