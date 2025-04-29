using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitDataBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirlineChatModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupChatId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineChatModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirlineDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlinePhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingAmount = table.Column<double>(type: "float", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineDataModels", x => x.Id);
                });

       
            migrationBuilder.CreateTable(
                name: "AirCraftList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    AirlineDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirCraftList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirCraftList_AirlineDataModels_AirlineDataModelId",
                        column: x => x.AirlineDataModelId,
                        principalTable: "AirlineDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirlineImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlineImages_AirlineDataModels_AirlineDataModelId",
                        column: x => x.AirlineDataModelId,
                        principalTable: "AirlineDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirlinePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    AirlineDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlinePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlinePayments_AirlineDataModels_AirlineDataModelId",
                        column: x => x.AirlineDataModelId,
                        principalTable: "AirlineDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirlineReviewModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatingNum = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineReviewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirlineReviewModels_AirlineDataModels_AirlineDataModelId",
                        column: x => x.AirlineDataModelId,
                        principalTable: "AirlineDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirLineServiceLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirlineDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirLineServiceLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirLineServiceLocations_AirlineDataModels_AirlineDataModelId",
                        column: x => x.AirlineDataModelId,
                        principalTable: "AirlineDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

        

            migrationBuilder.CreateIndex(
                name: "IX_AirCraftList_AirlineDataModelId",
                table: "AirCraftList",
                column: "AirlineDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AirlineImages_AirlineDataModelId",
                table: "AirlineImages",
                column: "AirlineDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AirlinePayments_AirlineDataModelId",
                table: "AirlinePayments",
                column: "AirlineDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AirlineReviewModels_AirlineDataModelId",
                table: "AirlineReviewModels",
                column: "AirlineDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AirLineServiceLocations_AirlineDataModelId",
                table: "AirLineServiceLocations",
                column: "AirlineDataModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirCraftList");

            migrationBuilder.DropTable(
                name: "AirlineChatModels");

            migrationBuilder.DropTable(
                name: "AirlineImages");

            migrationBuilder.DropTable(
                name: "AirlinePayments");

            migrationBuilder.DropTable(
                name: "AirlineReviewModels");

            migrationBuilder.DropTable(
                name: "AirLineServiceLocations");
        }
    }
}
