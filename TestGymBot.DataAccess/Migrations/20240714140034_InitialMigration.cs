using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGymBot.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonProps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    NeckGirth = table.Column<int>(type: "int", nullable: false),
                    ChestСircumference = table.Column<int>(type: "int", nullable: false),
                    ShoulderGirth = table.Column<int>(type: "int", nullable: false),
                    ArmCircumference = table.Column<int>(type: "int", nullable: false),
                    ForearmGirth = table.Column<int>(type: "int", nullable: false),
                    WaistCircumference = table.Column<int>(type: "int", nullable: false),
                    BellyGirth = table.Column<int>(type: "int", nullable: false),
                    ButtockGirth = table.Column<int>(type: "int", nullable: false),
                    HipGirth = table.Column<int>(type: "int", nullable: false),
                    ShinGirth = table.Column<int>(type: "int", nullable: false),
                    DateOfRecording = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonProps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalSession = table.Column<int>(type: "int", nullable: false),
                    CurrentSession = table.Column<int>(type: "int", nullable: false),
                    TotalCompletedSession = table.Column<int>(type: "int", nullable: false),
                    PropsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_PersonProps_PropsId",
                        column: x => x.PropsId,
                        principalTable: "PersonProps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 7, 14, 19, 0, 33, 433, DateTimeKind.Local).AddTicks(3392)),
                    PersonEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => new { x.PersonId, x.TimeId });
                    table.ForeignKey(
                        name: "FK_Records_Persons_PersonEntityId",
                        column: x => x.PersonEntityId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Records_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Records_Times_TimeEntityId",
                        column: x => x.TimeEntityId,
                        principalTable: "Times",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Records_Times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "Times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_PropsId",
                table: "Persons",
                column: "PropsId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_PersonEntityId",
                table: "Records",
                column: "PersonEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_TimeEntityId",
                table: "Records",
                column: "TimeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_TimeId",
                table: "Records",
                column: "TimeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Times");

            migrationBuilder.DropTable(
                name: "PersonProps");
        }
    }
}
