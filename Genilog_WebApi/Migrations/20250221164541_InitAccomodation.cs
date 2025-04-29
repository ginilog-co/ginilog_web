using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitAccomodation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccomodationChatModels",
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
                    table.PrimaryKey("PK_AccomodationChatModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckInTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckOutTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    AccomodationAdvertType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    AccomodationImages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationFacilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookAccomodationReservatioModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationLocality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomNumber = table.Column<int>(type: "int", nullable: false),
                    MaximumNoOfGuest = table.Column<int>(type: "int", nullable: true),
                    RoomPrice = table.Column<double>(type: "float", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomImages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomFeatures = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBooked = table.Column<bool>(type: "bit", nullable: true),
                    UpdateddAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAccomodationReservatioModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationFriday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationFriday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationFriday_AccomodationDataModels_AccomodationDataModelId",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationMonday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationMonday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationMonday_AccomodationDataModels_AccomodationDataModelId",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationReviewModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatingNum = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationDataTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationReviewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationReviewModels_AccomodationDataModels_AccomodationDataTableId",
                        column: x => x.AccomodationDataTableId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationSaturday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationSaturday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationSaturday_AccomodationDataModels_AccomodationDataModelId",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationSunday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationSunday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationSunday_AccomodationDataModels_AccomodationDataModelId",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationThursday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationThursday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationThursday_AccomodationDataModels_AccomodationDataModelId",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationTuesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationTuesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationTuesday_AccomodationDataModels_AccomodationDataModelId",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationWednesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationWednesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationWednesday_AccomodationDataModels_AccomodationDataModelId",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationFriday_AccomodationDataModelId",
                table: "AccomodationFriday",
                column: "AccomodationDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationMonday_AccomodationDataModelId",
                table: "AccomodationMonday",
                column: "AccomodationDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationReviewModels_AccomodationDataTableId",
                table: "AccomodationReviewModels",
                column: "AccomodationDataTableId");

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationSaturday_AccomodationDataModelId",
                table: "AccomodationSaturday",
                column: "AccomodationDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationSunday_AccomodationDataModelId",
                table: "AccomodationSunday",
                column: "AccomodationDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationThursday_AccomodationDataModelId",
                table: "AccomodationThursday",
                column: "AccomodationDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationTuesday_AccomodationDataModelId",
                table: "AccomodationTuesday",
                column: "AccomodationDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccomodationWednesday_AccomodationDataModelId",
                table: "AccomodationWednesday",
                column: "AccomodationDataModelId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccomodationChatModels");

            migrationBuilder.DropTable(
                name: "AccomodationFriday");

            migrationBuilder.DropTable(
                name: "AccomodationMonday");

            migrationBuilder.DropTable(
                name: "AccomodationReviewModels");

            migrationBuilder.DropTable(
                name: "AccomodationSaturday");

            migrationBuilder.DropTable(
                name: "AccomodationSunday");

            migrationBuilder.DropTable(
                name: "AccomodationThursday");

            migrationBuilder.DropTable(
                name: "AccomodationTuesday");

            migrationBuilder.DropTable(
                name: "AccomodationWednesday");

            migrationBuilder.DropTable(
                name: "BookAccomodationReservatioModels");

            migrationBuilder.DropTable(
                name: "AccomodationDataModels");
        }
    }
}
