using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitOrderAndBookingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PurchaseChannel",
                table: "OrderModelDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StaffId",
                table: "OrderModelDatas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "StaffName",
                table: "OrderModelDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "OrderModelDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseChannel",
                table: "CustomerBookedReservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StaffId",
                table: "CustomerBookedReservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "StaffName",
                table: "CustomerBookedReservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "CustomerBookedReservations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseChannel",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "StaffName",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "PurchaseChannel",
                table: "CustomerBookedReservations");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "CustomerBookedReservations");

            migrationBuilder.DropColumn(
                name: "StaffName",
                table: "CustomerBookedReservations");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "CustomerBookedReservations");
        }
    }
}
