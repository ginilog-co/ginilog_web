using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.LogisticsModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.PlacesRepo
{
    public class AccomodationRepository(Genilog_Data_Context mAAP_Context) : IAccomodationRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<AccomodationDataModel> AddAsync(AccomodationDataModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            dataInfo.AccomodationMonday!.Id = dataInfo.Id;
            dataInfo.AccomodationMonday!.AccomodationDataModelId = dataInfo.Id;
            dataInfo.AccomodationWednesday!.Id = dataInfo.Id;
            dataInfo.AccomodationWednesday!.AccomodationDataModelId = dataInfo.Id;
            dataInfo.AccomodationThursday!.Id = dataInfo.Id;
            dataInfo.AccomodationThursday!.AccomodationDataModelId = dataInfo.Id;
            dataInfo.AccomodationFriday!.Id = dataInfo.Id;
            dataInfo.AccomodationFriday!.AccomodationDataModelId = dataInfo.Id;
            dataInfo.AccomodationSaturday!.Id = dataInfo.Id;
            dataInfo.AccomodationSaturday!.AccomodationDataModelId = dataInfo.Id;
            dataInfo.AccomodationSunday!.Id = dataInfo.Id;
            dataInfo.AccomodationSunday!.AccomodationDataModelId = dataInfo.Id;
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AccomodationDataModel> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.AccomodationDataModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AccomodationDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<AccomodationDataModel>> GetAllAsync()
        {
            return await mAAP_Context.AccomodationDataModels!
                  .Include(x => x.AccomodationMonday)
                .Include(x => x.AccomodationTuesday)
                .Include(x => x.AccomodationWednesday)
                .Include(x => x.AccomodationThursday)
                .Include(x => x.AccomodationFriday)
                .Include(x => x.AccomodationSaturday)
                .Include(x => x.AccomodationSunday)
                  .Include(x => x.AccomodationReviewModels)
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }

        public async Task<AccomodationDataModel> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.AccomodationDataModels!
                  .Include(x => x.AccomodationMonday)
                .Include(x => x.AccomodationTuesday)
                .Include(x => x.AccomodationWednesday)
                .Include(x => x.AccomodationThursday)
                .Include(x => x.AccomodationFriday)
                .Include(x => x.AccomodationSaturday)
                .Include(x => x.AccomodationSunday)
                  .Include(x => x.AccomodationReviewModels).
               FirstOrDefaultAsync(x => x.Id == id|| x.AdminId==id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<AccomodationDataModel> UpdateAsync(Guid id, AccomodationDataModel dataInfo)
        {
            var dataValue = await mAAP_Context.AccomodationDataModels!
                 .Include(x => x.AccomodationMonday)
                .Include(x => x.AccomodationTuesday)
                .Include(x => x.AccomodationWednesday)
                .Include(x => x.AccomodationThursday)
                .Include(x => x.AccomodationFriday)
                .Include(x => x.AccomodationSaturday)
                .Include(x => x.AccomodationSunday)
                .FirstOrDefaultAsync(x => x.Id == id || x.AdminId == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.AccomodationName = dataInfo.AccomodationName;
                dataValue.AccomodationDescription = dataInfo.AccomodationDescription;
                dataValue.CheckInTime = dataInfo.CheckInTime;
                dataValue.CheckOutTime = dataInfo.CheckOutTime;
                dataValue.AccomodationWebsite = dataInfo.AccomodationWebsite;
                dataValue.AccomodationPhoneNo = dataInfo.AccomodationPhoneNo;
                dataValue.NoOfRooms = dataInfo.NoOfRooms;
                dataValue.Location = dataInfo.Location;
                dataValue.Locality = dataInfo.Locality;
                dataValue.Postcode = dataInfo.Postcode;
                dataValue.Longitude = dataInfo.Longitude;
                dataValue.Latitude = dataInfo.Latitude;
                dataValue.Rating = dataValue.Rating;
                dataValue.BookingAmount = dataInfo.BookingAmount;
                dataValue.AccomodationAdvertType = dataInfo.AccomodationAdvertType;
                dataValue.Available = dataInfo.Available;
                dataValue.AccomodationLogo = dataInfo.AccomodationLogo;
                dataValue.AccomodationFacilities = dataInfo.AccomodationFacilities;
                dataValue.AccomodationImages = dataInfo.AccomodationImages;
                // Monday
                dataValue.AccomodationMonday!.HourStart = dataInfo.AccomodationMonday!.HourStart;
                dataValue.AccomodationMonday!.HourEnd = dataInfo.AccomodationMonday!.HourEnd;
                dataValue.AccomodationMonday!.IsClosed = dataInfo.AccomodationMonday!.IsClosed;
                // Tuesday
                dataValue.AccomodationTuesday!.HourStart = dataInfo.AccomodationTuesday!.HourStart;
                dataValue.AccomodationTuesday!.HourEnd = dataInfo.AccomodationTuesday!.HourEnd;
                dataValue.AccomodationTuesday.IsClosed = dataInfo.AccomodationTuesday.IsClosed;
                // Wednesday
                dataValue.AccomodationWednesday!.HourStart = dataInfo.AccomodationWednesday!.HourStart;
                dataValue.AccomodationWednesday!.HourEnd = dataInfo.AccomodationWednesday!.HourStart;
                dataValue.AccomodationWednesday.IsClosed = dataInfo.AccomodationWednesday.IsClosed;
                // Thursday
                dataValue.AccomodationThursday!.HourStart = dataInfo.AccomodationThursday!.HourStart;
                dataValue.AccomodationThursday!.HourEnd = dataInfo.AccomodationThursday!.HourEnd;
                dataValue.AccomodationThursday.IsClosed = dataInfo.AccomodationThursday.IsClosed;
                // Friday
                dataValue.AccomodationFriday!.HourStart = dataInfo.AccomodationFriday!.HourStart;
                dataValue.AccomodationFriday!.HourEnd = dataInfo.AccomodationFriday.HourEnd;
                dataValue.AccomodationFriday.IsClosed = dataInfo.AccomodationFriday.IsClosed;
                // Saturday
                dataValue.AccomodationSaturday!.HourStart = dataInfo.AccomodationSaturday!.HourStart;
                dataValue.AccomodationSaturday!.HourEnd = dataInfo.AccomodationSaturday!.HourEnd;
                dataValue.AccomodationSaturday.IsClosed = dataInfo.AccomodationSaturday!.IsClosed;
                // Sunday
                dataValue.AccomodationSunday!.HourStart = dataInfo.AccomodationSunday!.HourStart;
                dataValue.AccomodationSunday!.HourEnd = dataInfo.AccomodationSunday!.HourEnd;
                dataValue.AccomodationSunday.IsClosed = dataInfo.AccomodationSunday.IsClosed;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        public async Task<bool> AdminIdExistAsync(Guid adminId)
        {
            var dataValue = await mAAP_Context.AirlineDataModels!.FirstOrDefaultAsync(x => x.AdminId == adminId);
            if (dataValue == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<AccomodationReviewModel> AddAccomodationReviewAsync(AccomodationReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AccomodationReviewModel> DeleteAccomodationReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.AccomodationReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AccomodationReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        //Reservations
        public async Task<BookAccomodationReservatioModel> AddBookAccomodationReservationAsync(BookAccomodationReservatioModel dataInfo)
        {
           
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<BookAccomodationReservatioModel> DeleteBookAccomodationReservationAsync(Guid id)
        {
            var tickets = await mAAP_Context.BookAccomodationReservatioModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.BookAccomodationReservatioModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<BookAccomodationReservatioModel>> GetAllBookAccomodationReservationAsync()
        {
            return await mAAP_Context.BookAccomodationReservatioModels!
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }

        public async Task<BookAccomodationReservatioModel> GetBookAccomodationReservationAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.BookAccomodationReservatioModels!.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<BookAccomodationReservatioModel> UpdateBookAccomodationReservationAsync(Guid id, BookAccomodationReservatioModel dataInfo)
        {
            var dataValue = await mAAP_Context.BookAccomodationReservatioModels!
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.IsBooked = dataInfo.IsBooked;
                dataValue.RoomNumber = dataInfo.RoomNumber;
                dataValue.RoomFeatures = dataInfo.RoomFeatures;
                dataValue.QRCode = dataInfo.QRCode;
                dataValue.RoomPrice = dataInfo.RoomPrice;
                dataValue.RoomType = dataInfo.RoomType;
                dataValue.RoomImages= dataInfo.RoomImages;  
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        public async Task<BookAccomodationReservatioModel> BookAccomodationReservationRoomExistAsync(int roomNumber, Guid accommodationId, BookAccomodationReservatioModel dataInfo)        {
             var user = await mAAP_Context.BookAccomodationReservatioModels!.AnyAsync(x => x.RoomNumber == roomNumber && x.AccomodationId == accommodationId);
            if (user)
            {
                var dataValue = await mAAP_Context.BookAccomodationReservatioModels!.FirstOrDefaultAsync(x => x.RoomNumber == roomNumber && x.AccomodationId == accommodationId);
                dataValue!.MaximumNoOfGuest = dataInfo.MaximumNoOfGuest;
                dataValue.RoomPrice = dataInfo.RoomPrice;
                dataValue.IsBooked = dataInfo.IsBooked;
                dataValue.QRCode = dataInfo.QRCode;
                dataValue.CreatedAt = dataInfo.CreatedAt;
                dataValue.UpdateddAt = dataInfo.UpdateddAt;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
            else
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
           }


        //Customer
        public async Task<CustomerBookedReservation> AddCustomerBookedReservationAsync(CustomerBookedReservation dataInfo)
        {
           // dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<CustomerBookedReservation> DeleteCustomerBookedReservationAsync(Guid id)
        {
            var tickets = await mAAP_Context.CustomerBookedReservations!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.CustomerBookedReservations!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<CustomerBookedReservation>> GetAllCustomerBookedReservationAsync()
        {
            return await mAAP_Context.CustomerBookedReservations!
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }

        public async Task<CustomerBookedReservation> GetCustomerBookedReservationAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.CustomerBookedReservations!.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<CustomerBookedReservation> UpdateCustomerBookedReservationAsync(Guid id, CustomerBookedReservation dataInfo)
        {
            var dataValue = await mAAP_Context.CustomerBookedReservations!
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.TrnxReference = dataInfo.TrnxReference;
                dataValue.NumberOfGuests = dataInfo.NumberOfGuests;
                dataValue.CustomerPhoneNumber = dataInfo.CustomerPhoneNumber;
                dataValue.CustomerName = dataInfo.CustomerName;
                dataValue.CustomerEmail = dataInfo.CustomerEmail;
                dataValue.PaymentChannel = dataInfo.PaymentChannel;
                dataValue.Comment = dataInfo.Comment;
                dataValue.PaymentStatus = dataInfo.PaymentStatus;
                dataValue.TicketClosed = dataInfo.TicketClosed;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        public bool CustomerBookedReservationDateExistAsync(DateTime startDate, DateTime endDate, List<CustomerBookedReservation> existingReservations)
        {
            foreach (var reservation in existingReservations)
            {
                if (startDate <= reservation.ReservationEndDate && endDate >= reservation.ReservationStartDate)
                {
                    return false; // Overlaps with an existing reservation
                }
            }
            return true;
        }

        // Accomodation Chat
        public async Task<IEnumerable<AccomodationChatModel>> GetAllAccomodationChatAsync()
        {
            return await mAAP_Context.AccomodationChatModels!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<AccomodationChatModel> AddAccomodationChatAsync(AccomodationChatModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AccomodationChatModel> DeleteAccomodationChatAsync(Guid id)
        {
            var tickets = await mAAP_Context.AccomodationChatModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AccomodationChatModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AccomodationChatModel> UpdateAccomodationChatAsync(Guid id, AccomodationChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.AccomodationChatModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.Message = dataInfo.Message;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        public async Task<AccomodationChatModel> UpdateAccomodationIsReadChatAsync(Guid id, AccomodationChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.AccomodationChatModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.IsRead = dataInfo.IsRead;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }
    }
}
