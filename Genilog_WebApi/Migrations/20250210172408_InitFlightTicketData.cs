using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitFlightTicketData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightTicketBookModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapatureTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailabeTimeInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dapature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopPlaces = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BigLuggageKg = table.Column<int>(type: "int", nullable: true),
                    SmallLuggageKg = table.Column<int>(type: "int", nullable: true),
                    Stops = table.Column<int>(type: "int", nullable: true),
                    Available = table.Column<bool>(type: "bit", nullable: true),
                    IsReturn = table.Column<bool>(type: "bit", nullable: true),
                    TicketPrice = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTicketBookModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightTicketBookModels");
        }
    }
}
