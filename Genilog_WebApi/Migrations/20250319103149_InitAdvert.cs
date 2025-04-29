using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitAdvert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvertHolderModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvertItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvertImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvertName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvertType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvertItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvertItemCost = table.Column<double>(type: "float", nullable: false),
                    AdvertDays4 = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertHolderModels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertHolderModels");
        }
    }
}
