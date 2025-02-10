using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitDataForOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogisticsId",
                table: "OrderModelDatas",
                newName: "RiderId");

            migrationBuilder.RenameColumn(
                name: "ItemImage",
                table: "OrderModelDatas",
                newName: "RiderName");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "OrderModelDatas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "OrderModelDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PackageImageLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrlFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderModelDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageImageLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageImageLists_OrderModelDatas_OrderModelDataId",
                        column: x => x.OrderModelDataId,
                        principalTable: "OrderModelDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageImageLists_OrderModelDataId",
                table: "PackageImageLists",
                column: "OrderModelDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageImageLists");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "OrderModelDatas");

            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "OrderModelDatas");

            migrationBuilder.RenameColumn(
                name: "RiderName",
                table: "OrderModelDatas",
                newName: "ItemImage");

            migrationBuilder.RenameColumn(
                name: "RiderId",
                table: "OrderModelDatas",
                newName: "LogisticsId");
        }
    }
}
