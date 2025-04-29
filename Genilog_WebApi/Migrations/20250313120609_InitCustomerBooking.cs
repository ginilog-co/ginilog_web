using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitCustomerBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirlineImages");

            migrationBuilder.DropTable(
                name: "AirlinePayments");

            migrationBuilder.AddColumn<string>(
                name: "DepartureAirpot",
                table: "FlightTicketBookModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAirpot",
                table: "FlightTicketBookModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirlineImages",
                table: "AirlineDataModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureAirpot",
                table: "FlightTicketBookModels");

            migrationBuilder.DropColumn(
                name: "ReturnAirpot",
                table: "FlightTicketBookModels");

            migrationBuilder.DropColumn(
                name: "AirlineImages",
                table: "AirlineDataModels");

            migrationBuilder.CreateTable(
                name: "AirlineImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlineImages_AirlineDataModels_AirlineDataModelId",
                        column: x => x.AirlineDataModelId,
                        principalTable: "AirlineDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirlinePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Titles = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlinePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlinePayments_AirlineDataModels_AirlineDataModelId",
                        column: x => x.AirlineDataModelId,
                        principalTable: "AirlineDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineImages_AirlineDataModelId",
                table: "AirlineImages",
                column: "AirlineDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AirlinePayments_AirlineDataModelId",
                table: "AirlinePayments",
                column: "AirlineDataModelId");
        }
    }
}
