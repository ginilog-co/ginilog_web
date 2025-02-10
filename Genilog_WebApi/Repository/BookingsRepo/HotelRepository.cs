using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.BookingsModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.PlacesRepo
{
    public class HotelRepository(Genilog_Data_Context mAAP_Context) : IHotelRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<HotelDataModel> AddAsync(HotelDataModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            dataInfo.HotelMonday!.Id = dataInfo.Id;
            dataInfo.HotelMonday!.HotelDataModelId = dataInfo.Id;
            dataInfo.HotelWednesday!.Id = dataInfo.Id;
            dataInfo.HotelWednesday!.HotelDataModelId = dataInfo.Id;
            dataInfo.HotelThursday!.Id = dataInfo.Id;
            dataInfo.HotelThursday!.HotelDataModelId = dataInfo.Id;
            dataInfo.HotelFriday!.Id = dataInfo.Id;
            dataInfo.HotelFriday!.HotelDataModelId = dataInfo.Id;
            dataInfo.HotelSaturday!.Id = dataInfo.Id;
            dataInfo.HotelSaturday!.HotelDataModelId = dataInfo.Id;
            dataInfo.HotelSunday!.Id = dataInfo.Id;
            dataInfo.HotelSunday!.HotelDataModelId = dataInfo.Id;
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<HotelDataModel> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.HotelDataModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.HotelDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<HotelDataModel>> GetAllAsync()
        {
            return await mAAP_Context.HotelDataModels!
                  .Include(x => x.HotelMonday)
                .Include(x => x.HotelTuesday)
                .Include(x => x.HotelWednesday)
                .Include(x => x.HotelThursday)
                .Include(x => x.HotelFriday)
                .Include(x => x.HotelSaturday)
                .Include(x => x.HotelSunday)
                  .Include(x => x.HotelImages)
                  .Include(x => x.HotelFacilities)
                  .Include(x => x.HotelReviewModels)
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }


        public async Task<HotelDataModel> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.HotelDataModels!
                  .Include(x => x.HotelMonday)
                .Include(x => x.HotelTuesday)
                .Include(x => x.HotelWednesday)
                .Include(x => x.HotelThursday)
                .Include(x => x.HotelFriday)
                .Include(x => x.HotelSaturday)
                .Include(x => x.HotelSunday)
                  .Include(x => x.HotelImages)
                  .Include(x => x.HotelFacilities)
                  .Include(x => x.HotelReviewModels).
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<HotelDataModel> UpdateAsync(Guid id, HotelDataModel dataInfo)
        {
            var dataValue = await mAAP_Context.HotelDataModels!
                 .Include(x => x.HotelMonday)
                .Include(x => x.HotelTuesday)
                .Include(x => x.HotelWednesday)
                .Include(x => x.HotelThursday)
                .Include(x => x.HotelFriday)
                .Include(x => x.HotelSaturday)
                .Include(x => x.HotelSunday)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.HotelName = dataInfo.HotelName;
                dataValue.HotelDescription = dataInfo.HotelDescription;
                dataValue.CheckInTime = dataInfo.CheckInTime;
                dataValue.CheckOutTime = dataInfo.CheckOutTime;
                dataValue.HotelWebsite = dataInfo.HotelWebsite;
                dataValue.HotelPhoneNo = dataInfo.HotelPhoneNo;
                dataValue.NoOfRooms = dataInfo.NoOfRooms;
                dataValue.Location = dataInfo.Location;
                dataValue.Locality = dataInfo.Locality;
                dataValue.Postcode = dataInfo.Postcode;
                dataValue.Longitude = dataInfo.Longitude;
                dataValue.Latitude = dataInfo.Latitude;
                dataValue.Rating = dataValue.Rating;
                dataValue.BookingAmount = dataInfo.BookingAmount;
                dataValue.HotelAdvertType = dataInfo.HotelAdvertType;
                dataValue.Available = dataInfo.Available;
                dataValue.HotelLogo = dataInfo.HotelLogo;
                // Monday
                dataValue.HotelMonday!.HourStart = dataInfo.HotelMonday!.HourStart;
                dataValue.HotelMonday!.HourEnd = dataInfo.HotelMonday!.HourEnd;
                dataValue.HotelMonday!.IsClosed = dataInfo.HotelMonday!.IsClosed;
                // Tuesday
                dataValue.HotelTuesday!.HourStart = dataInfo.HotelTuesday!.HourStart;
                dataValue.HotelTuesday!.HourEnd = dataInfo.HotelTuesday!.HourEnd;
                dataValue.HotelTuesday.IsClosed = dataInfo.HotelTuesday.IsClosed;
                // Wednesday
                dataValue.HotelWednesday!.HourStart = dataInfo.HotelWednesday!.HourStart;
                dataValue.HotelWednesday!.HourEnd = dataInfo.HotelWednesday!.HourStart;
                dataValue.HotelWednesday.IsClosed = dataInfo.HotelWednesday.IsClosed;
                // Thursday
                dataValue.HotelThursday!.HourStart = dataInfo.HotelThursday!.HourStart;
                dataValue.HotelThursday!.HourEnd = dataInfo.HotelThursday!.HourEnd;
                dataValue.HotelThursday.IsClosed = dataInfo.HotelThursday.IsClosed;
                // Friday
                dataValue.HotelFriday!.HourStart = dataInfo.HotelFriday!.HourStart;
                dataValue.HotelFriday!.HourEnd = dataInfo.HotelFriday.HourEnd;
                dataValue.HotelFriday.IsClosed = dataInfo.HotelFriday.IsClosed;
                // Saturday
                dataValue.HotelSaturday!.HourStart = dataInfo.HotelSaturday!.HourStart;
                dataValue.HotelSaturday!.HourEnd = dataInfo.HotelSaturday!.HourEnd;
                dataValue.HotelSaturday.IsClosed = dataInfo.HotelSaturday!.IsClosed;
                // Sunday
                dataValue.HotelSunday!.HourStart = dataInfo.HotelSunday!.HourStart;
                dataValue.HotelSunday!.HourEnd = dataInfo.HotelSunday!.HourEnd;
                dataValue.HotelSunday.IsClosed = dataInfo.HotelSunday.IsClosed;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }
        public async Task<HotelImages> AddHotelImageAsync(HotelImages dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<HotelImages> DeleteHotelImageAsync(Guid id)
        {
            var tickets = await mAAP_Context.HotelImages!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.HotelImages!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }


        public async Task<HotelFacilities> AddHotelFacilitiesAsync(HotelFacilities dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<HotelFacilities> DeleteHotelFacilitiesAsync(Guid id)
        {
            var tickets = await mAAP_Context.HotelFacilities!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.HotelFacilities!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }


        public async Task<HotelReviewModel> AddHotelReviewAsync(HotelReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<HotelReviewModel> DeleteHotelReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.HotelReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.HotelReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        // Hotel Chat
        public async Task<IEnumerable<HotelChatModel>> GetAllHotelChatAsync()
        {
            return await mAAP_Context.HotelChatModels!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<HotelChatModel> AddHotelChatAsync(HotelChatModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<HotelChatModel> DeleteHotelChatAsync(Guid id)
        {
            var tickets = await mAAP_Context.HotelChatModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.HotelChatModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<HotelChatModel> UpdateHotelChatAsync(Guid id, HotelChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.HotelChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<HotelChatModel> UpdateHotelIsReadChatAsync(Guid id, HotelChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.HotelChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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
