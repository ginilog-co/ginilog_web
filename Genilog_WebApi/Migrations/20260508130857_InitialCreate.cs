using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genilog_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccomodationChatModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupChatId = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    MessageType = table.Column<string>(type: "text", nullable: true),
                    ItemImageURL = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationChatModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccomodationName = table.Column<string>(type: "text", nullable: true),
                    AccomodationLogo = table.Column<string>(type: "text", nullable: true),
                    AccomodationEmail = table.Column<string>(type: "text", nullable: true),
                    AccomodationDescription = table.Column<string>(type: "text", nullable: true),
                    AccomodationType = table.Column<string>(type: "text", nullable: true),
                    CheckInTime = table.Column<string>(type: "text", nullable: true),
                    CheckOutTime = table.Column<string>(type: "text", nullable: true),
                    AccomodationWebsite = table.Column<string>(type: "text", nullable: true),
                    AccomodationPhoneNo = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    Postcode = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    BookingAmount = table.Column<double>(type: "double precision", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    NoOfRooms = table.Column<int>(type: "integer", nullable: false),
                    AccomodationAdvertType = table.Column<string>(type: "text", nullable: true),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    AccomodationImages = table.Column<List<string>>(type: "text[]", nullable: true),
                    AccomodationFacilities = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminModelTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    SurName = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    StaffCode = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Branch = table.Column<string>(type: "text", nullable: true),
                    AdminType = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    CompanyUserName = table.Column<string>(type: "text", nullable: true),
                    CompanyType = table.Column<List<string>>(type: "text[]", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DatePublished = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminModelTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvertHolderModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvertItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvertImage = table.Column<string>(type: "text", nullable: true),
                    AdvertName = table.Column<string>(type: "text", nullable: true),
                    AdvertType = table.Column<string>(type: "text", nullable: true),
                    AdvertItemDescription = table.Column<string>(type: "text", nullable: true),
                    AdvertItemCost = table.Column<double>(type: "double precision", nullable: false),
                    TransRef = table.Column<string>(type: "text", nullable: true),
                    TransStatus = table.Column<bool>(type: "boolean", nullable: false),
                    AdvertDays4 = table.Column<int>(type: "integer", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertHolderModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirlineChatModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupChatId = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    MessageType = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineChatModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirlineDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AirlineName = table.Column<string>(type: "text", nullable: true),
                    AirlineLogo = table.Column<string>(type: "text", nullable: true),
                    AirlineEmail = table.Column<string>(type: "text", nullable: true),
                    AirlineInfo = table.Column<string>(type: "text", nullable: true),
                    AirlineType = table.Column<string>(type: "text", nullable: true),
                    AirlinePhoneNo = table.Column<string>(type: "text", nullable: true),
                    AirlineWebsite = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    BookingAmount = table.Column<double>(type: "double precision", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    AirlineImages = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookAccomodationReservatioModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccomodationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccomodationName = table.Column<string>(type: "text", nullable: true),
                    AccomodationType = table.Column<string>(type: "text", nullable: true),
                    AccomodationLocality = table.Column<string>(type: "text", nullable: true),
                    AccomodationState = table.Column<string>(type: "text", nullable: true),
                    AccomodationImage = table.Column<string>(type: "text", nullable: true),
                    TicketNum = table.Column<string>(type: "text", nullable: true),
                    RoomNumber = table.Column<int>(type: "integer", nullable: false),
                    MaximumNoOfGuest = table.Column<int>(type: "integer", nullable: true),
                    RoomPrice = table.Column<double>(type: "double precision", nullable: false),
                    RoomType = table.Column<string>(type: "text", nullable: true),
                    RoomImages = table.Column<List<string>>(type: "text[]", nullable: true),
                    RoomFeatures = table.Column<List<string>>(type: "text[]", nullable: true),
                    QRCode = table.Column<string>(type: "text", nullable: true),
                    IsBooked = table.Column<bool>(type: "boolean", nullable: true),
                    UpdateddAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckInTime = table.Column<string>(type: "text", nullable: true),
                    CheckOutTime = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAccomodationReservatioModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyApplyDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    SurName = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    CompanyUserName = table.Column<string>(type: "text", nullable: true),
                    CompanyAddress = table.Column<string>(type: "text", nullable: true),
                    CompanyType = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyApplyDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyEmail = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    CompanyLogo = table.Column<string>(type: "text", nullable: true),
                    CompanyRegNo = table.Column<string>(type: "text", nullable: true),
                    CompanyInfo = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    ValueCharge = table.Column<double>(type: "double precision", nullable: false),
                    NoOfTrucks = table.Column<int>(type: "integer", nullable: false),
                    NofOfBikes = table.Column<int>(type: "integer", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyAddress = table.Column<string>(type: "text", nullable: true),
                    PostCodes = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    AccountName = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    DeliveryTypes = table.Column<List<string>>(type: "text[]", nullable: true),
                    ServiceAreas = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerBookedReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResevationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccomodationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccomodationName = table.Column<string>(type: "text", nullable: true),
                    AccomodationType = table.Column<string>(type: "text", nullable: true),
                    AccomodationLocation = table.Column<string>(type: "text", nullable: true),
                    AccomodationImage = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomNumber = table.Column<int>(type: "integer", nullable: false),
                    QRCode = table.Column<string>(type: "text", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    CustomerEmail = table.Column<string>(type: "text", nullable: true),
                    NumberOfGuests = table.Column<int>(type: "integer", nullable: false),
                    TrnxReference = table.Column<string>(type: "text", nullable: true),
                    PaymentChannel = table.Column<string>(type: "text", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    TicketNum = table.Column<string>(type: "text", nullable: true),
                    ReservationStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReservationEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NoOfDays = table.Column<int>(type: "integer", nullable: true),
                    TicketClosed = table.Column<bool>(type: "boolean", nullable: false),
                    TotalCost = table.Column<double>(type: "double precision", nullable: true),
                    UpdateddAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffName = table.Column<string>(type: "text", nullable: true),
                    PurchaseChannel = table.Column<string>(type: "text", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBookedReservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTokenModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceTokenId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokenModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Feedback = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    DatePublished = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightTicketBookModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AirlineId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AirlineName = table.Column<string>(type: "text", nullable: true),
                    DepartureAirpot = table.Column<string>(type: "text", nullable: true),
                    ReturnAirpot = table.Column<string>(type: "text", nullable: true),
                    OperatedBy = table.Column<string>(type: "text", nullable: true),
                    FlightSpeed = table.Column<string>(type: "text", nullable: true),
                    TicketNum = table.Column<string>(type: "text", nullable: true),
                    DapatureTime = table.Column<string>(type: "text", nullable: true),
                    AvailabeTimeInterval = table.Column<string>(type: "text", nullable: true),
                    Dapature = table.Column<string>(type: "text", nullable: true),
                    Destination = table.Column<string>(type: "text", nullable: true),
                    TicketType = table.Column<string>(type: "text", nullable: true),
                    StopPlaces = table.Column<List<string>>(type: "text[]", nullable: true),
                    BigLuggageKg = table.Column<int>(type: "integer", nullable: true),
                    SmallLuggageKg = table.Column<int>(type: "integer", nullable: true),
                    Stops = table.Column<int>(type: "integer", nullable: true),
                    Available = table.Column<bool>(type: "boolean", nullable: true),
                    IsReturn = table.Column<bool>(type: "boolean", nullable: true),
                    TicketPrice = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTicketBookModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: true),
                    VerificationToken = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: true),
                    EmailTokenExpires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhoneVerificationToken = table.Column<string>(type: "text", nullable: true),
                    PhoneNoConfirmed = table.Column<bool>(type: "boolean", nullable: true),
                    PhoneNoTokenExpires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhoneVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "text", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: true),
                    LockOutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockOutEndEnabled = table.Column<bool>(type: "boolean", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    NotificationType = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingNum = table.Column<string>(type: "text", nullable: true),
                    ItemName = table.Column<string>(type: "text", nullable: true),
                    ItemDescription = table.Column<string>(type: "text", nullable: true),
                    ItemModelNumber = table.Column<string>(type: "text", nullable: true),
                    ItemCost = table.Column<double>(type: "double precision", nullable: false),
                    ItemWeight = table.Column<double>(type: "double precision", nullable: false),
                    ItemQuantity = table.Column<int>(type: "integer", nullable: false),
                    PackageType = table.Column<string>(type: "text", nullable: true),
                    ExpectedDeliveryTime = table.Column<string>(type: "text", nullable: true),
                    OrderStatus = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderName = table.Column<string>(type: "text", nullable: true),
                    SenderPhoneNo = table.Column<string>(type: "text", nullable: true),
                    SenderEmail = table.Column<string>(type: "text", nullable: true),
                    SenderAddress = table.Column<string>(type: "text", nullable: true),
                    SenderState = table.Column<string>(type: "text", nullable: true),
                    SenderCountry = table.Column<string>(type: "text", nullable: true),
                    SenderLocality = table.Column<string>(type: "text", nullable: true),
                    SenderPostalCode = table.Column<string>(type: "text", nullable: true),
                    SenderLatitude = table.Column<double>(type: "double precision", nullable: false),
                    SenderLongitude = table.Column<double>(type: "double precision", nullable: false),
                    RecieverName = table.Column<string>(type: "text", nullable: true),
                    RecieverPhoneNo = table.Column<string>(type: "text", nullable: true),
                    RecieverEmail = table.Column<string>(type: "text", nullable: true),
                    RecieverAddress = table.Column<string>(type: "text", nullable: true),
                    RecieverState = table.Column<string>(type: "text", nullable: true),
                    RecieverCountry = table.Column<string>(type: "text", nullable: true),
                    RecieverLocality = table.Column<string>(type: "text", nullable: true),
                    RecieverPostalCode = table.Column<string>(type: "text", nullable: true),
                    RecieverLatitude = table.Column<double>(type: "double precision", nullable: false),
                    RecieverLongitude = table.Column<double>(type: "double precision", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    RiderId = table.Column<Guid>(type: "uuid", nullable: false),
                    RiderName = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    CompanyPhoneNo = table.Column<string>(type: "text", nullable: true),
                    CompanyEmail = table.Column<string>(type: "text", nullable: true),
                    CompanyAddress = table.Column<string>(type: "text", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "double precision", nullable: false),
                    CurrentLongitude = table.Column<double>(type: "double precision", nullable: false),
                    CurrentLocation = table.Column<string>(type: "text", nullable: true),
                    ConfirmationImage = table.Column<string>(type: "text", nullable: true),
                    ShippingCost = table.Column<double>(type: "double precision", nullable: false),
                    VatCost = table.Column<double>(type: "double precision", nullable: false),
                    TrnxReference = table.Column<string>(type: "text", nullable: true),
                    PaymentChannel = table.Column<string>(type: "text", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "boolean", nullable: false),
                    QRCode = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PackageImageLists = table.Column<List<string>>(type: "text[]", nullable: true),
                    RiderType = table.Column<string>(type: "text", nullable: true),
                    ShippingType = table.Column<string>(type: "text", nullable: true),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffName = table.Column<string>(type: "text", nullable: true),
                    PurchaseChannel = table.Column<string>(type: "text", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RidersChatModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupChatId = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    MessageType = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RidersChatModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RidersModelDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    ProfilePicture = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    PostCodes = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    AccountName = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RidersModelDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    TransactionType = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    TrnxRef = table.Column<string>(type: "text", nullable: true),
                    TrnxStatus = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDataModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersDataModelTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    ProfilePicture = table.Column<string>(type: "text", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    ReferralCode = table.Column<string>(type: "text", nullable: true),
                    UserStatus = table.Column<bool>(type: "boolean", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    PostCodes = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    MoneyBoxBalance = table.Column<double>(type: "double precision", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    AccountName = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersDataModelTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationFriday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HourStart = table.Column<string>(type: "text", nullable: true),
                    HourEnd = table.Column<string>(type: "text", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationFriday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationFriday_AccomodationDataModels_AccomodationDataM~",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationMonday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HourStart = table.Column<string>(type: "text", nullable: true),
                    HourEnd = table.Column<string>(type: "text", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationMonday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationMonday_AccomodationDataModels_AccomodationDataM~",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationReviewModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    ReviewMessage = table.Column<string>(type: "text", nullable: true),
                    RatingNum = table.Column<double>(type: "double precision", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccomodationDataTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationReviewModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationReviewModels_AccomodationDataModels_Accomodatio~",
                        column: x => x.AccomodationDataTableId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationSaturday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HourStart = table.Column<string>(type: "text", nullable: true),
                    HourEnd = table.Column<string>(type: "text", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationSaturday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationSaturday_AccomodationDataModels_AccomodationDat~",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationSunday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HourStart = table.Column<string>(type: "text", nullable: true),
                    HourEnd = table.Column<string>(type: "text", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationSunday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationSunday_AccomodationDataModels_AccomodationDataM~",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationThursday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HourStart = table.Column<string>(type: "text", nullable: true),
                    HourEnd = table.Column<string>(type: "text", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationThursday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationThursday_AccomodationDataModels_AccomodationDat~",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationTuesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HourStart = table.Column<string>(type: "text", nullable: true),
                    HourEnd = table.Column<string>(type: "text", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationTuesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationTuesday_AccomodationDataModels_AccomodationData~",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccomodationWednesday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HourStart = table.Column<string>(type: "text", nullable: true),
                    HourEnd = table.Column<string>(type: "text", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: true),
                    AccomodationDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccomodationWednesday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccomodationWednesday_AccomodationDataModels_AccomodationDa~",
                        column: x => x.AccomodationDataModelId,
                        principalTable: "AccomodationDataModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirCraftList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Manufacturer = table.Column<string>(type: "text", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    AirlineDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    ReviewMessage = table.Column<string>(type: "text", nullable: true),
                    RatingNum = table.Column<double>(type: "double precision", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AirlineDataModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    AirlineDataModelId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    ReviewMessage = table.Column<string>(type: "text", nullable: true),
                    RatingNum = table.Column<double>(type: "double precision", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyModelDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderStatus = table.Column<string>(type: "text", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "double precision", nullable: false),
                    CurrentLongitude = table.Column<double>(type: "double precision", nullable: false),
                    CurrentLocation = table.Column<string>(type: "text", nullable: true),
                    OrderModelDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    ReviewMessage = table.Column<string>(type: "text", nullable: true),
                    RatingNum = table.Column<double>(type: "double precision", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RidersModelDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneralUsersId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    PhoneNo = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    AddressPostCodes = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    HouseNo = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    UsersDataModelTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
