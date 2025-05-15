using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitAllData : Migration
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
                    AdminType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatePublished = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminModelTables", x => x.Id);
                });

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
                    AirlineImages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineDataModels", x => x.Id);
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
                name: "CompanyApplyDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyApplyDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    DeliveryTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceAreas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerBookedReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResevationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccomodationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomNumber = table.Column<int>(type: "int", nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    TrnxReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReservationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoOfDays = table.Column<int>(type: "int", nullable: true),
                    TicketClosed = table.Column<bool>(type: "bit", nullable: false),
                    TotalCost = table.Column<double>(type: "float", nullable: true),
                    UpdateddAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBookedReservations", x => x.Id);
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
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatePublished = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightTicketBookModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirlineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartureAirpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnAirpot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapatureTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailabeTimeInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dapature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopPlaces = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BigLuggageKg = table.Column<int>(type: "int", nullable: true),
                    SmallLuggageKg = table.Column<int>(type: "int", nullable: true),
                    Stops = table.Column<int>(type: "int", nullable: true),
                    Available = table.Column<bool>(type: "bit", nullable: true),
                    IsReturn = table.Column<bool>(type: "bit", nullable: true),
                    TicketPrice = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTicketBookModels", x => x.Id);
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
                name: "NotificationModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemModelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCost = table.Column<double>(type: "float", nullable: false),
                    ItemWeight = table.Column<double>(type: "float", nullable: false),
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
                    SenderCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderLocality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderLatitude = table.Column<double>(type: "float", nullable: false),
                    SenderLongitude = table.Column<double>(type: "float", nullable: false),
                    RecieverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverLocality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverLatitude = table.Column<double>(type: "float", nullable: false),
                    RecieverLongitude = table.Column<double>(type: "float", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "float", nullable: false),
                    CurrentLongitude = table.Column<double>(type: "float", nullable: false),
                    CurrentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmationImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingCost = table.Column<double>(type: "float", nullable: false),
                    VatCost = table.Column<double>(type: "float", nullable: false),
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
                name: "TransactionDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrnxRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrnxStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDataModels", x => x.Id);
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
                    MoneyBoxBalance = table.Column<double>(type: "float", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersDataModelTables", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_AirCraftList_AirlineDataModelId",
                table: "AirCraftList",
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
                name: "IX_CompanyReviewModels_CompanyModelDataId",
                table: "CompanyReviewModels",
                column: "CompanyModelDataId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddresses_UsersDataModelTableId",
                table: "DeliveryAddresses",
                column: "UsersDataModelTableId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDeliveryFlows_OrderModelDataId",
                table: "OrderDeliveryFlows",
                column: "OrderModelDataId");

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
                name: "AdminModelTables");

            migrationBuilder.DropTable(
                name: "AdvertHolderModels");

            migrationBuilder.DropTable(
                name: "AirCraftList");

            migrationBuilder.DropTable(
                name: "AirlineChatModels");

            migrationBuilder.DropTable(
                name: "AirlineReviewModels");

            migrationBuilder.DropTable(
                name: "AirLineServiceLocations");

            migrationBuilder.DropTable(
                name: "BookAccomodationReservatioModels");

            migrationBuilder.DropTable(
                name: "CompanyApplyDataModels");

            migrationBuilder.DropTable(
                name: "CompanyReviewModels");

            migrationBuilder.DropTable(
                name: "CustomerBookedReservations");

            migrationBuilder.DropTable(
                name: "DeliveryAddresses");

            migrationBuilder.DropTable(
                name: "DeviceTokenModels");

            migrationBuilder.DropTable(
                name: "FeedbackModelDatas");

            migrationBuilder.DropTable(
                name: "FlightTicketBookModels");

            migrationBuilder.DropTable(
                name: "NotificationModels");

            migrationBuilder.DropTable(
                name: "OrderDeliveryFlows");

            migrationBuilder.DropTable(
                name: "RidersChatModelDatas");

            migrationBuilder.DropTable(
                name: "RidersReviewModels");

            migrationBuilder.DropTable(
                name: "TransactionDataModels");

            migrationBuilder.DropTable(
                name: "User_Roles");

            migrationBuilder.DropTable(
                name: "AccomodationDataModels");

            migrationBuilder.DropTable(
                name: "AirlineDataModels");

            migrationBuilder.DropTable(
                name: "CompanyModelDatas");

            migrationBuilder.DropTable(
                name: "UsersDataModelTables");

            migrationBuilder.DropTable(
                name: "OrderModelDatas");

            migrationBuilder.DropTable(
                name: "RidersModelDatas");

            migrationBuilder.DropTable(
                name: "GeneralUsers");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
