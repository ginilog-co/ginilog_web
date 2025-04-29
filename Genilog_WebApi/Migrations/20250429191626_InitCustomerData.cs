using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitCustomerData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerBookedReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResevationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomNumber = table.Column<int>(type: "int", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    TrnxReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReservationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoOfDays = table.Column<int>(type: "int", nullable: true),
                    TicketClosed = table.Column<bool>(type: "bit", nullable: false),
                    TotalCost = table.Column<double>(type: "float", nullable: true),
                    UpdateddAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBookedReservations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerBookedReservations");
        }
    }
}
