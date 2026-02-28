using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitAddedReservationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckInTime",
                table: "BookAccomodationReservatioModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutTime",
                table: "BookAccomodationReservatioModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "BookAccomodationReservatioModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "BookAccomodationReservatioModels");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "BookAccomodationReservatioModels");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "BookAccomodationReservatioModels");
        }
    }
}
