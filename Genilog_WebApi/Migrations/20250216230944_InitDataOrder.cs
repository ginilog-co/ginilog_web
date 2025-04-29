using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitDataOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemModelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCost = table.Column<double>(type: "float", nullable: false),
                    ItemQuantity = table.Column<int>(type: "int", nullable: false),
                    PackageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedDeliveryTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderLocality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderLatitude = table.Column<double>(type: "float", nullable: false),
                    SenderLongitude = table.Column<double>(type: "float", nullable: false),
                    RecieverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverLocality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverLatitude = table.Column<double>(type: "float", nullable: false),
                    RecieverLongitude = table.Column<double>(type: "float", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RiderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "float", nullable: false),
                    CurrentLongitude = table.Column<double>(type: "float", nullable: false),
                    CurrentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmationImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingCost = table.Column<double>(type: "float", nullable: false),
                    TrnxReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "bit", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PackageImageLists = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RiderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderModelDatas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderModelDatas");
        }
    }
}
