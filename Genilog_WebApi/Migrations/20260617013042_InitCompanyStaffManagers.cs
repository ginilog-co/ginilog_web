using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitCompanyStaffManagers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AdminModelTables");

            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "AdminModelTables");

            migrationBuilder.DropColumn(
                name: "CompanyUserName",
                table: "AdminModelTables");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "AdminModelTables");

            migrationBuilder.CreateTable(
                name: "CompanyManagerStaffDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyManagerStaffDataModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyManagerStaffDataModels");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AdminModelTables",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                table: "AdminModelTables",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyUserName",
                table: "AdminModelTables",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "AdminModelTables",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
