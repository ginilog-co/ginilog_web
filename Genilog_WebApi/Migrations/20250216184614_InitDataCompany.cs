using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitDataCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "CompanyModelDatas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DeliveryTypes",
                table: "CompanyModelDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceAreas",
                table: "CompanyModelDatas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "CompanyModelDatas");

            migrationBuilder.DropColumn(
                name: "DeliveryTypes",
                table: "CompanyModelDatas");

            migrationBuilder.DropColumn(
                name: "ServiceAreas",
                table: "CompanyModelDatas");
        }
    }
}
