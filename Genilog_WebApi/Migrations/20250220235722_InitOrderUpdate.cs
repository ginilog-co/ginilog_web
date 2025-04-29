using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitOrderUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ItemWeight",
                table: "OrderModelDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "RecieverCountry",
                table: "OrderModelDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderCountry",
                table: "OrderModelDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VatCost",
                table: "OrderModelDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "OrderDeliveryFlows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "float", nullable: false),
                    CurrentLongitude = table.Column<double>(type: "float", nullable: false),
                    CurrentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderModelDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDeliveryFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDeliveryFlows_OrderModelDatas_OrderModelDataId",
                        column: x => x.OrderModelDataId,
                        principalTable: "OrderModelDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDeliveryFlows_OrderModelDataId",
                table: "OrderDeliveryFlows",
                column: "OrderModelDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDeliveryFlows");

            migrationBuilder.DropColumn(
                name: "ItemWeight",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "RecieverCountry",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "SenderCountry",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "VatCost",
                table: "OrderModelDatas");
        }
    }
}
