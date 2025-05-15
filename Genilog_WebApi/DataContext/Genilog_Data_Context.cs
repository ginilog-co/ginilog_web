
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

            modelBuilder.Entity<OrderDeliveryFlow>()
              .HasOne(x => x.OrderModelDatas)
              .WithMany(y => y.OrderDeliveryFlows)
              .HasForeignKey(x => x.OrderModelDataId);

            //Airline Model
            modelBuilder.Entity<AirLineServiceLocation>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirLineServiceLocations)
                .HasForeignKey(x => x.AirlineDataModelId);

            modelBuilder.Entity<AirCraftList>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirCraftList)
                .HasForeignKey(x => x.AirlineDataModelId);

            modelBuilder.Entity<AirlineReviewModel>()
                .HasOne(x => x.AirlineDataModels)
                .WithMany(y => y.AirlineReviewModels)
                .HasForeignKey(x => x.AirlineDataModelId);


            // Accomodation Data
            modelBuilder.Entity<AccomodationDataModel>()
            .HasOne(a => a.AccomodationMonday)
            .WithOne(b => b.AccomodationDataModels)
            .HasForeignKey<AccomodationMondayModel>(b => b.AccomodationDataModelId);

            modelBuilder.Entity<AccomodationDataModel>()
                .HasOne(a => a.AccomodationTuesday)
                .WithOne(b => b.AccomodationDataModels)
                .HasForeignKey<AccomodationTuesdayModel>(b => b.AccomodationDataModelId);

            modelBuilder.Entity<AccomodationDataModel>()
                .HasOne(a => a.AccomodationWednesday)
                .WithOne(b => b.AccomodationDataModels)
                .HasForeignKey<AccomodationWednesdayModel>(b => b.AccomodationDataModelId);

            modelBuilder.Entity<AccomodationDataModel>()
                .HasOne(a => a.AccomodationThursday)
                .WithOne(b => b.AccomodationDataModels)
                .HasForeignKey<AccomodationThursdayModel>(b => b.AccomodationDataModelId);

            modelBuilder.Entity<AccomodationDataModel>()
                .HasOne(a => a.AccomodationFriday)
                .WithOne(b => b.AccomodationDataModels)
                .HasForeignKey<AccomodationFridayModel>(b => b.AccomodationDataModelId);

            modelBuilder.Entity<AccomodationDataModel>()
                .HasOne(a => a.AccomodationSaturday)
                .WithOne(b => b.AccomodationDataModels)
                .HasForeignKey<AccomodationSaturdayModel>(b => b.AccomodationDataModelId);

            modelBuilder.Entity<AccomodationDataModel>()
                .HasOne(a => a.AccomodationSunday)
                .WithOne(b => b.AccomodationDataModels)
                .HasForeignKey<AccomodationSundayModel>(b => b.AccomodationDataModelId);

            modelBuilder.Entity<AccomodationReviewModel>()
             .HasOne(x => x.AccomodationDataTables)
            .WithMany(y => y.AccomodationReviewModels)
            .HasForeignKey(x => x.AccomodationDataTableId);
        }
        public DbSet<GeneralUsers>? GeneralUsers { get; set; }
        public DbSet<UsersDataModelTable>? UsersDataModelTables { get; set; }
        public DbSet<DeviceTokenModel>? DeviceTokenModels { get; set; }
        public DbSet<DeliveryAddress>? DeliveryAddresses { get; set; }
        public DbSet<Roles>? Roles { get; set; }
        public DbSet<User_Role>? User_Roles { get; set; }
        public DbSet<AdminModelTable>? AdminModelTables { get; set; }
        public DbSet<AdvertHolderModel>? AdvertHolderModels { get; set; }
        public DbSet<CompanyApplyDataModel>? CompanyApplyDataModels { get; set; }
        public DbSet<FeedbackModelData>? FeedbackModelDatas { get; set; }
        public DbSet<NotificationModel>? NotificationModels { get; set; }

        // Company
        public DbSet<CompanyModelData>? CompanyModelDatas { get; set; }
        public DbSet<CompanyReviewModel>? CompanyReviewModels { get; set; }
        public DbSet<RidersModelData>? RidersModelDatas { get; set; }
        public DbSet<RidersReviewModel>? RidersReviewModels { get; set; }
        public DbSet<OrderModelData>? OrderModelDatas { get; set; }
        public DbSet<RidersChatModelData>? RidersChatModelDatas { get; set; }
        public DbSet<OrderDeliveryFlow>? OrderDeliveryFlows { get; set; }

        // Accomodations Data
        public DbSet<AccomodationDataModel>? AccomodationDataModels { get; set; }
        public DbSet<AccomodationReviewModel>? AccomodationReviewModels { get; set; }
        public DbSet<AccomodationMondayModel>? AccomodationMonday { get; set; }
        public DbSet<AccomodationTuesdayModel>? AccomodationTuesday { get; set; }
        public DbSet<AccomodationWednesdayModel>? AccomodationWednesday { get; set; }
        public DbSet<AccomodationThursdayModel>? AccomodationThursday { get; set; }
        public DbSet<AccomodationFridayModel>? AccomodationFriday { get; set; }
        public DbSet<AccomodationSaturdayModel>? AccomodationSaturday { get; set; }
        public DbSet<AccomodationSundayModel>? AccomodationSunday { get; set; }
        public DbSet<AccomodationChatModel>? AccomodationChatModels { get; set; }
        public DbSet<BookAccomodationReservatioModel>? BookAccomodationReservatioModels { get; set; }
        public DbSet<CustomerBookedReservation>? CustomerBookedReservations { get; set; }

        // AirLine Data
        public DbSet<AirlineDataModel>? AirlineDataModels { get; set; }
        public DbSet<AirCraftList>? AirCraftList { get; set; }
        public DbSet<AirlineReviewModel>? AirlineReviewModels { get; set; }
        public DbSet<AirLineServiceLocation>? AirLineServiceLocations { get; set; }
        public DbSet<AirlineChatModel>? AirlineChatModels { get; set; }
        public DbSet<FlightTicketBookModel>? FlightTicketBookModels { get; set; }
        // Wallet
        public DbSet<TransactionDataModel>? TransactionDataModels { get; set; }
    }
}

