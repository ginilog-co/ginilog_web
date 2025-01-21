using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitFistData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminModelTables",
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
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatePublished = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminModelTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyRegNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ValueCharge = table.Column<double>(type: "float", nullable: false),
                    NoOfTrucks = table.Column<int>(type: "int", nullable: false),
                    NofOfBikes = table.Column<int>(type: "int", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTokenModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceTokenId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokenModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatePublished = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    EmailTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneVerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNoConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNoTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: true),
                    LockOutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockOutEndEnabled = table.Column<bool>(type: "bit", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralUsers", x => x.Id);
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
                name: "NotificationModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationModels", x => x.Id);
                });

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
                    ItemImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    LogisticsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "float", nullable: false),
                    CurrentLongitude = table.Column<double>(type: "float", nullable: false),
                    ConfirmationImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingCost = table.Column<double>(type: "float", nullable: false),
                    TrnxReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "bit", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayoutDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PayOutStatus = table.Column<bool>(type: "bit", nullable: false),
                    PaymentTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayOutDateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayoutDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlacesChatModels",
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
                    table.PrimaryKey("PK_PlacesChatModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlacesDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceOverview = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacesAdditionalInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancelationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacesHighlights = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckInTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckOutTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacePhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    BookingAmount = table.Column<double>(type: "float", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    PlaceAdvertType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPayment = table.Column<bool>(type: "bit", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacesDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RidersChatModelDatas",
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
                    table.PrimaryKey("PK_RidersChatModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RidersModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RidersModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersDataModelTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferralCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserStatus = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersDataModelTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyReviewModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatingNum = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyModelDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyReviewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyReviewModels_CompanyModelDatas_CompanyModelDataId",
                        column: x => x.CompanyModelDataId,
                        principalTable: "CompanyModelDatas",
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

            migrationBuilder.CreateTable(
                name: "PlaceFacilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Facilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceFacilities_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceFriday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceFriday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceFriday_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceImages_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceMonday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceMonday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceMonday_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceReviewModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatingNum = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceReviewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceReviewModels_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceSaturday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceSaturday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceSaturday_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceSunday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceSunday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceSunday_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceThursday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceThursday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceThursday_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceTuesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceTuesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceTuesday_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceWednesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HourStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceWednesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceWednesday_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceWhatToExpects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacesDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceWhatToExpects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceWhatToExpects_PlacesDataModels_PlacesDataModelId",
                        column: x => x.PlacesDataModelId,
                        principalTable: "PlacesDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RidersReviewModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatingNum = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RidersModelDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RidersReviewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RidersReviewModels_RidersModelDatas_RidersModelDataId",
                        column: x => x.RidersModelDataId,
                        principalTable: "RidersModelDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneralUsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Roles_GeneralUsers_GeneralUsersId",
                        column: x => x.GeneralUsersId,
                        principalTable: "GeneralUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Roles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressPostCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    UsersDataModelTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAddresses_UsersDataModelTables_UsersDataModelTableId",
                        column: x => x.UsersDataModelTableId,
                        principalTable: "UsersDataModelTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviewModels_CompanyModelDataId",
                table: "CompanyReviewModels",
                column: "CompanyModelDataId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddresses_UsersDataModelTableId",
                table: "DeliveryAddresses",
                column: "UsersDataModelTableId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PlaceFacilities_PlacesDataModelId",
                table: "PlaceFacilities",
                column: "PlacesDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceFriday_PlacesDataModelId",
                table: "PlaceFriday",
                column: "PlacesDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceImages_PlacesDataModelId",
                table: "PlaceImages",
                column: "PlacesDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceMonday_PlacesDataModelId",
                table: "PlaceMonday",
                column: "PlacesDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceReviewModels_PlacesDataModelId",
                table: "PlaceReviewModels",
                column: "PlacesDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceSaturday_PlacesDataModelId",
                table: "PlaceSaturday",
                column: "PlacesDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceSunday_PlacesDataModelId",
                table: "PlaceSunday",
                column: "PlacesDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceThursday_PlacesDataModelId",
                table: "PlaceThursday",
                column: "PlacesDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceTuesday_PlacesDataModelId",
                table: "PlaceTuesday",
                column: "PlacesDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceWednesday_PlacesDataModelId",
                table: "PlaceWednesday",
                column: "PlacesDataModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceWhatToExpects_PlacesDataModelId",
                table: "PlaceWhatToExpects",
                column: "PlacesDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_RidersReviewModels_RidersModelDataId",
                table: "RidersReviewModels",
                column: "RidersModelDataId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Roles_GeneralUsersId",
                table: "User_Roles",
                column: "GeneralUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Roles_RoleId",
                table: "User_Roles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminModelTables");

            migrationBuilder.DropTable(
                name: "CompanyReviewModels");

            migrationBuilder.DropTable(
                name: "DeliveryAddresses");

            migrationBuilder.DropTable(
                name: "DeviceTokenModels");

            migrationBuilder.DropTable(
                name: "FeedbackModelDatas");

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
                name: "NotificationModels");

            migrationBuilder.DropTable(
                name: "OrderModelDatas");

            migrationBuilder.DropTable(
                name: "PayoutDataModels");

            migrationBuilder.DropTable(
                name: "PlaceFacilities");

            migrationBuilder.DropTable(
                name: "PlaceFriday");

            migrationBuilder.DropTable(
                name: "PlaceImages");

            migrationBuilder.DropTable(
                name: "PlaceMonday");

            migrationBuilder.DropTable(
                name: "PlaceReviewModels");

            migrationBuilder.DropTable(
                name: "PlaceSaturday");

            migrationBuilder.DropTable(
                name: "PlacesChatModels");

            migrationBuilder.DropTable(
                name: "PlaceSunday");

            migrationBuilder.DropTable(
                name: "PlaceThursday");

            migrationBuilder.DropTable(
                name: "PlaceTuesday");

            migrationBuilder.DropTable(
                name: "PlaceWednesday");

            migrationBuilder.DropTable(
                name: "PlaceWhatToExpects");

            migrationBuilder.DropTable(
                name: "RidersChatModelDatas");

            migrationBuilder.DropTable(
                name: "RidersReviewModels");

            migrationBuilder.DropTable(
                name: "User_Roles");

            migrationBuilder.DropTable(
                name: "CompanyModelDatas");

            migrationBuilder.DropTable(
                name: "UsersDataModelTables");

            migrationBuilder.DropTable(
                name: "HotelDataModels");

            migrationBuilder.DropTable(
                name: "PlacesDataModels");

            migrationBuilder.DropTable(
                name: "RidersModelDatas");

            migrationBuilder.DropTable(
                name: "GeneralUsers");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
