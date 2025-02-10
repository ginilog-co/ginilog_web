
using Genilog_WebApi.Model.AdminsModel;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.InfoModel;
using Genilog_WebApi.Model.LogisticsModel;
using Genilog_WebApi.Model.Notification_Model;
using Genilog_WebApi.Model.UsersDataModel;
using Genilog_WebApi.Model.WalletModel;
using Microsoft.EntityFrameworkCore;


namespace Genilog_WebApi.DataContext
{
    public class Genilog_Data_Context(DbContextOptions<Genilog_Data_Context> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_Role>()
                .HasOne(x => x.Roles)
                .WithMany(y => y.User_Roles)
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<DeliveryAddress>()
             .HasOne(x => x.UsersDataModelTable)
             .WithMany(y => y.DeliveryAddresses)
             .HasForeignKey(x => x.UsersDataModelTableId);


            // Company
            modelBuilder.Entity<CompanyReviewModel>()
                .HasOne(x => x.CompanyModelDatas)
                .WithMany(y => y.CompanyReviewModels)
                .HasForeignKey(x => x.CompanyModelDataId);

            modelBuilder.Entity<RidersReviewModel>()
                .HasOne(x => x.RidersModelDatas)
                .WithMany(y => y.RidersReviewModels)
                .HasForeignKey(x => x.RidersModelDataId);
            modelBuilder.Entity<PackageImageList>()
              .HasOne(x => x.OrderModelDatas)
              .WithMany(y => y.PackageImageLists)
              .HasForeignKey(x => x.OrderModelDataId);

            //Airline Model
           
          
            modelBuilder.Entity<AirlineImages>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirlineImages)
                .HasForeignKey(x => x.AirlineDataModelId);
            modelBuilder.Entity<AirLineServiceLocation>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirLineServiceLocations)
                .HasForeignKey(x => x.AirlineDataModelId);

            modelBuilder.Entity<AirCraftList>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirCraftList)
                .HasForeignKey(x => x.AirlineDataModelId);

            modelBuilder.Entity<AirlinePayment>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirlinePayments)
                .HasForeignKey(x => x.AirlineDataModelId);

            modelBuilder.Entity<AirlineReviewModel>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirlineReviewModels)
                .HasForeignKey(x => x.AirlineDataModelId);


            // Hotel Data
            modelBuilder.Entity<HotelDataModel>()
            .HasOne(a => a.HotelMonday)
            .WithOne(b => b.HotelDataModels)
            .HasForeignKey<HotelMondayModel>(b => b.HotelDataModelId);

            modelBuilder.Entity<HotelDataModel>()
                .HasOne(a => a.HotelTuesday)
                .WithOne(b => b.HotelDataModels)
                .HasForeignKey<HotelTuesdayModel>(b => b.HotelDataModelId);

            modelBuilder.Entity<HotelDataModel>()
                .HasOne(a => a.HotelWednesday)
                .WithOne(b => b.HotelDataModels)
                .HasForeignKey<HotelWednesdayModel>(b => b.HotelDataModelId);

            modelBuilder.Entity<HotelDataModel>()
                .HasOne(a => a.HotelThursday)
                .WithOne(b => b.HotelDataModels)
                .HasForeignKey<HotelThursdayModel>(b => b.HotelDataModelId);

            modelBuilder.Entity<HotelDataModel>()
                .HasOne(a => a.HotelFriday)
                .WithOne(b => b.HotelDataModels)
                .HasForeignKey<HotelFridayModel>(b => b.HotelDataModelId);

            modelBuilder.Entity<HotelDataModel>()
                .HasOne(a => a.HotelSaturday)
                .WithOne(b => b.HotelDataModels)
                .HasForeignKey<HotelSaturdayModel>(b => b.HotelDataModelId);

            modelBuilder.Entity<HotelDataModel>()
                .HasOne(a => a.HotelSunday)
                .WithOne(b => b.HotelDataModels)
                .HasForeignKey<HotelSundayModel>(b => b.HotelDataModelId);

            modelBuilder.Entity<HotelImages>()
            .HasOne(x => x.HotelDataTables)
           .WithMany(y => y.HotelImages)
           .HasForeignKey(x => x.HotelDataTableId);

            modelBuilder.Entity<HotelFacilities>()
             .HasOne(x => x.HotelDataTables)
            .WithMany(y => y.HotelFacilities)
            .HasForeignKey(x => x.HotelDataTableId);

            modelBuilder.Entity<HotelReviewModel>()
             .HasOne(x => x.HotelDataTables)
            .WithMany(y => y.HotelReviewModels)
            .HasForeignKey(x => x.HotelDataTableId);



        }
        public DbSet<GeneralUsers>? GeneralUsers { get; set; }
        public DbSet<UsersDataModelTable>? UsersDataModelTables { get; set; }
        public DbSet<DeviceTokenModel>? DeviceTokenModels { get; set; }
        public DbSet<DeliveryAddress>? DeliveryAddresses { get; set; }
        public DbSet<Roles>? Roles { get; set; }
        public DbSet<User_Role>? User_Roles { get; set; }
        public DbSet<AdminModelTable>? AdminModelTables { get; set; }
        public DbSet<FeedbackModelData>? FeedbackModelDatas { get; set; }
        public DbSet<NotificationModel>? NotificationModels { get; set; }

        // Company
        public DbSet<CompanyModelData>? CompanyModelDatas { get; set; }
        public DbSet<CompanyReviewModel>? CompanyReviewModels { get; set; }
        public DbSet<RidersModelData>? RidersModelDatas { get; set; }
        public DbSet<RidersReviewModel>? RidersReviewModels { get; set; }
        public DbSet<OrderModelData>? OrderModelDatas { get; set; }
        public DbSet<PackageImageList>? PackageImageLists { get; set; }
        public DbSet<RidersChatModelData>? RidersChatModelDatas { get; set; }

        // Hotels Data
        public DbSet<HotelDataModel>? HotelDataModels { get; set; }
        public DbSet<HotelImages>? HotelImages { get; set; }
        public DbSet<HotelFacilities>? HotelFacilities { get; set; }
        public DbSet<HotelReviewModel>? HotelReviewModels { get; set; }
        public DbSet<HotelMondayModel>? HotelMonday { get; set; }
        public DbSet<HotelTuesdayModel>? HotelTuesday { get; set; }
        public DbSet<HotelWednesdayModel>? HotelWednesday { get; set; }
        public DbSet<HotelThursdayModel>? HotelThursday { get; set; }
        public DbSet<HotelFridayModel>? HotelFriday { get; set; }
        public DbSet<HotelSaturdayModel>? HotelSaturday { get; set; }
        public DbSet<HotelSundayModel>? HotelSunday { get; set; }
        public DbSet<HotelChatModel>? HotelChatModels { get; set; }

        // AirLine Data
        public DbSet<AirlineDataModel>? AirlineDataModels { get; set; }
        public DbSet<AirlineImages>? AirlineImages { get; set; }
        public DbSet<AirCraftList>? AirCraftList { get; set; }
        public DbSet<AirlinePayment>? AirlinePayments { get; set; }
        public DbSet<AirlineReviewModel>? AirlineReviewModels { get; set; }
        public DbSet<AirLineServiceLocation>? AirLineServiceLocations { get; set; }
        public DbSet<AirlineChatModel>? AirlineChatModels { get; set; }
        public DbSet<FlightTicketBookModel>? FlightTicketBookModels { get; set; }

        // Wallet System
        public DbSet<PayoutDataModel>? PayoutDataModels { get; set; }
    }
}

