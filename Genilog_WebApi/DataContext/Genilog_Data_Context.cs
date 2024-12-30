
using Genilog_WebApi.Model.AdminsModel;
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model.InfoModel;
using Genilog_WebApi.Model.LogisticsModel;
using Genilog_WebApi.Model.Notification_Model;
using Genilog_WebApi.Model.PlacesModel;
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

           
            // Logistics

            modelBuilder.Entity<LogisticsReviewModel>()
                .HasOne(x => x.LogisticsDataModels)
                .WithMany(y => y.LogisticsReviewModels)
                .HasForeignKey(x => x.LogisticsDataModelId);

            //Places Model
            modelBuilder.Entity<PlacesDataModel>()
               .HasOne(a => a.PlacesMonday)
               .WithOne(b => b.PlacesDataModels)
               .HasForeignKey<PlacesMondayModel>(b => b.PlacesDataModelId);

            modelBuilder.Entity<PlacesDataModel>()
                .HasOne(a => a.PlacesTuesday)
                .WithOne(b => b.PlacesDataModels)
                .HasForeignKey<PlacesTuesdayModel>(b => b.PlacesDataModelId);

            modelBuilder.Entity<PlacesDataModel>()
                .HasOne(a => a.PlacesWednesday)
                .WithOne(b => b.PlacesDataModels)
                .HasForeignKey<PlacesWednesdayModel>(b => b.PlacesDataModelId);

            modelBuilder.Entity<PlacesDataModel>()
                .HasOne(a => a.PlacesThursday)
                .WithOne(b => b.PlacesDataModels)
                .HasForeignKey<PlacesThursdayModel>(b => b.PlacesDataModelId);

            modelBuilder.Entity<PlacesDataModel>()
                .HasOne(a => a.PlacesFriday)
                .WithOne(b => b.PlacesDataModels)
                .HasForeignKey<PlacesFridayModel>(b => b.PlacesDataModelId);

            modelBuilder.Entity<PlacesDataModel>()
                .HasOne(a => a.PlacesSaturday)
                .WithOne(b => b.PlacesDataModels)
                .HasForeignKey<PlacesSaturdayModel>(b => b.PlacesDataModelId);

            modelBuilder.Entity<PlacesDataModel>()
                .HasOne(a => a.PlacesSunday)
                .WithOne(b => b.PlacesDataModels)
                .HasForeignKey<PlacesSundayModel>(b => b.PlacesDataModelId);

            modelBuilder.Entity<PlaceImages>()
                .HasOne(x => x.PlacesDataModels)
                .WithMany(y => y.PlaceImages)
                .HasForeignKey(x => x.PlacesDataModelId);

            modelBuilder.Entity<PlaceFacilities>()
                .HasOne(x => x.PlacesDataModels)
                .WithMany(y => y.PlaceFacilities)
                .HasForeignKey(x => x.PlacesDataModelId);

            modelBuilder.Entity<PlaceWhatToExpect>()
                .HasOne(x => x.PlacesDataModels)
                .WithMany(y => y.PlaceWhatToExpects)
                .HasForeignKey(x => x.PlacesDataModelId);

            modelBuilder.Entity<PlaceReviewModel>()
                .HasOne(x => x.PlacesDataModels)
                .WithMany(y => y.PlaceReviewModels)
                .HasForeignKey(x => x.PlacesDataModelId);


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

        // Logistics
        public DbSet<LogisticsDataModel>? LogisticsDataModels { get; set; }
        public DbSet<LogisticsReviewModel>? LogisticsReviewModels { get; set; }
        public DbSet<OrderModelData>? OrderModelDatas { get; set; }
        public DbSet<LogisticsChatModelData>? LogisticsChatModelDatas { get; set; }

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

        // Places Data
        public DbSet<PlacesDataModel>? PlacesDataModels { get; set; }
        public DbSet<PlaceImages>? PlaceImages { get; set; }
        public DbSet<PlaceFacilities>? PlaceFacilities { get; set; }
        public DbSet<PlaceWhatToExpect>? PlaceWhatToExpects { get; set; }
        public DbSet<PlaceReviewModel>? PlaceReviewModels { get; set; }
        public DbSet<PlacesMondayModel>? PlaceMonday { get; set; }
        public DbSet<PlacesTuesdayModel>? PlaceTuesday { get; set; }
        public DbSet<PlacesWednesdayModel>? PlaceWednesday { get; set; }
        public DbSet<PlacesThursdayModel>? PlaceThursday { get; set; }
        public DbSet<PlacesFridayModel>? PlaceFriday { get; set; }
        public DbSet<PlacesSaturdayModel>? PlaceSaturday { get; set; }
        public DbSet<PlacesSundayModel>? PlaceSunday { get; set; }
        public DbSet<PlacesChatModel>? PlacesChatModels { get; set; }

        // Wallet System
        public DbSet<PayoutDataModel>? PayoutDataModels { get; set; }
    }
}

