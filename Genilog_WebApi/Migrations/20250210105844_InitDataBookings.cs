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
                name: "HotelChatModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupChatId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelChatModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotelDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckInTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckOutTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    BookingAmount = table.Column<double>(type: "float", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    NoOfRooms = table.Column<int>(type: "int", nullable: false),
                    HotelAdvertType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelDataModels", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "HotelFacilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Facilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelDataTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelFacilities_HotelDataModels_HotelDataTableId",
                        column: x => x.HotelDataTableId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelFriday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    HotelDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelFriday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelFriday_HotelDataModels_HotelDataModelId",
                        column: x => x.HotelDataModelId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HotelDataTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelImages_HotelDataModels_HotelDataTableId",
                        column: x => x.HotelDataTableId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelMonday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    HotelDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelMonday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelMonday_HotelDataModels_HotelDataModelId",
                        column: x => x.HotelDataModelId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelReviewModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatingNum = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelDataTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelReviewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelReviewModels_HotelDataModels_HotelDataTableId",
                        column: x => x.HotelDataTableId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelSaturday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    HotelDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelSaturday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelSaturday_HotelDataModels_HotelDataModelId",
                        column: x => x.HotelDataModelId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelSunday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    HotelDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelSunday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelSunday_HotelDataModels_HotelDataModelId",
                        column: x => x.HotelDataModelId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelThursday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    HotelDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelThursday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelThursday_HotelDataModels_HotelDataModelId",
                        column: x => x.HotelDataModelId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelTuesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    HotelDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelTuesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelTuesday_HotelDataModels_HotelDataModelId",
                        column: x => x.HotelDataModelId,
                        principalTable: "HotelDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelWednesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    HotelDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelWednesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelWednesday_HotelDataModels_HotelDataModelId",
                        column: x => x.HotelDataModelId,
                        principalTable: "HotelDataModels",
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

            migrationBuilder.CreateIndex(
                name: "IX_HotelFacilities_HotelDataTableId",
                table: "HotelFacilities",
                column: "HotelDataTableId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelFriday_HotelDataModelId",
                table: "HotelFriday",
                column: "HotelDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelImages_HotelDataTableId",
                table: "HotelImages",
                column: "HotelDataTableId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelMonday_HotelDataModelId",
                table: "HotelMonday",
                column: "HotelDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelReviewModels_HotelDataTableId",
                table: "HotelReviewModels",
                column: "HotelDataTableId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelSaturday_HotelDataModelId",
                table: "HotelSaturday",
                column: "HotelDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelSunday_HotelDataModelId",
                table: "HotelSunday",
                column: "HotelDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelThursday_HotelDataModelId",
                table: "HotelThursday",
                column: "HotelDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelTuesday_HotelDataModelId",
                table: "HotelTuesday",
                column: "HotelDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelWednesday_HotelDataModelId",
                table: "HotelWednesday",
                column: "HotelDataModelId",
                unique: true);
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

            migrationBuilder.DropTable(
                name: "HotelChatModels");

            migrationBuilder.DropTable(
                name: "HotelFacilities");

            migrationBuilder.DropTable(
                name: "HotelFriday");

            migrationBuilder.DropTable(
                name: "HotelImages");

            migrationBuilder.DropTable(
                name: "HotelMonday");

            migrationBuilder.DropTable(
                name: "HotelReviewModels");

            migrationBuilder.DropTable(
                name: "HotelSaturday");

            migrationBuilder.DropTable(
                name: "HotelSunday");

            migrationBuilder.DropTable(
                name: "HotelThursday");

            migrationBuilder.DropTable(
                name: "HotelTuesday");

            migrationBuilder.DropTable(
                name: "HotelWednesday");

            migrationBuilder.DropTable(
                name: "AirlineDataModels");

            migrationBuilder.DropTable(
                name: "HotelDataModels");
        }
    }
}
