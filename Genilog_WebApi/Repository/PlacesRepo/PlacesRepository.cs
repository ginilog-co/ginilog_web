using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.PlacesModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.PlacesRepo
{
    public class PlacesRepository(Genilog_Data_Context mAAP_Context) : IPlacesRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<PlacesDataModel> AddAsync(PlacesDataModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            dataInfo.PlacesMonday!.Id=dataInfo.Id;
            dataInfo.PlacesMonday!.PlacesDataModelId=dataInfo.Id;
            dataInfo.PlacesWednesday!.Id=dataInfo.Id;
            dataInfo.PlacesWednesday!.PlacesDataModelId=dataInfo.Id;
            dataInfo.PlacesThursday!.Id=dataInfo.Id;
            dataInfo.PlacesThursday!.PlacesDataModelId=dataInfo.Id;
            dataInfo.PlacesFriday!.Id=dataInfo.Id;
            dataInfo.PlacesFriday!.PlacesDataModelId=dataInfo.Id;
            dataInfo.PlacesSaturday!.Id=dataInfo.Id;
            dataInfo.PlacesSaturday!.PlacesDataModelId=dataInfo.Id;
            dataInfo.PlacesSunday!.Id=dataInfo.Id;
            dataInfo.PlacesSunday!.PlacesDataModelId=dataInfo.Id;
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<PlacesDataModel> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.PlacesDataModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.PlacesDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<PlacesDataModel>> GetAllAsync()
        {
            return await mAAP_Context.PlacesDataModels!
                .Include(x => x.PlacesMonday)
                .Include(x => x.PlacesTuesday)
                .Include(x => x.PlacesWednesday)
                .Include(x => x.PlacesThursday)
                .Include(x => x.PlacesFriday)
                .Include(x => x.PlacesSaturday)
                .Include(x => x.PlacesSunday)
                  .Include(x => x.PlaceImages)
                  .Include(x => x.PlaceFacilities)
                  .Include(x => x.PlaceWhatToExpects)
                  .Include(x => x.PlaceReviewModels)
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }


        public async Task<PlacesDataModel> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.PlacesDataModels!
                .Include(x => x.PlacesMonday)
                .Include(x => x.PlacesTuesday)
                .Include(x => x.PlacesWednesday)
                .Include(x => x.PlacesThursday)
                .Include(x => x.PlacesFriday)
                .Include(x => x.PlacesSaturday)
                .Include(x => x.PlacesSunday)
                  .Include(x => x.PlaceImages)
                  .Include(x => x.PlaceFacilities)
                  .Include(x => x.PlaceWhatToExpects)
                  .Include(x => x.PlaceReviewModels).
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PlacesDataModel> UpdateAsync(Guid id, PlacesDataModel dataInfo)
        {
            var dataValue = await mAAP_Context.PlacesDataModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.PlaceName = dataInfo.PlaceName;
                dataValue.PlaceEmail = dataInfo.PlaceEmail;
                dataValue.CheckInTime = dataInfo.CheckInTime;
                dataValue.CheckOutTime = dataInfo.CheckOutTime;
                dataValue.PlaceWebsite = dataInfo.PlaceWebsite;
                dataValue.PlacePhoneNo = dataInfo.PlacePhoneNo;
                dataValue.Location = dataInfo.Location;
                dataValue.Locality = dataInfo.Locality;
                dataValue.Postcode = dataInfo.Postcode;
                dataValue.Longitude = dataInfo.Longitude;
                dataValue.Latitude = dataInfo.Latitude;
                dataValue.Rating = dataValue.Rating;
                dataValue.BookingAmount = dataInfo.BookingAmount;
                dataValue.PlaceAdvertType = dataInfo.PlaceAdvertType;
                dataValue.PlaceOverview = dataInfo.PlaceOverview;
                dataValue.PlacesAdditionalInfo = dataInfo.PlacesAdditionalInfo;
                dataValue.CancelationPolicy = dataInfo.CancelationPolicy;
                dataValue.PlacesHighlights = dataInfo.PlacesHighlights;
                dataValue.Available = dataInfo.Available;
                dataValue.PlaceLogo = dataInfo.PlaceLogo;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

    
        public async Task<PlacesDataModel> UpdateTimeSheduleAsync(Guid id, PlacesDataModel dataInfo)
        {
            var dataValue = await mAAP_Context.PlacesDataModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Monday
                dataValue.PlacesMonday!.HourStart = dataInfo.PlacesMonday!.HourStart;
                dataValue.PlacesMonday!.HourEnd = dataInfo.PlacesMonday!.HourEnd;
                dataValue.PlacesMonday!.IsClosed = dataInfo.PlacesMonday!.IsClosed;
                // Tuesday
                dataValue.PlacesTuesday!.HourStart = dataInfo.PlacesTuesday!.HourStart;
                dataValue.PlacesTuesday!.HourEnd = dataInfo.PlacesTuesday!.HourEnd;
                dataValue.PlacesTuesday.IsClosed = dataInfo.PlacesTuesday.IsClosed;
                // Wednesday
                dataValue.PlacesWednesday!.HourStart = dataInfo.PlacesWednesday!.HourStart;
                dataValue.PlacesWednesday!.HourEnd = dataInfo.PlacesWednesday!.HourStart;
                dataValue.PlacesWednesday.IsClosed = dataInfo.PlacesWednesday.IsClosed;
                // Thursday
                dataValue.PlacesThursday!.HourStart = dataInfo.PlacesThursday!.HourStart;
                dataValue.PlacesThursday!.HourEnd = dataInfo.PlacesThursday!.HourEnd;
                dataValue.PlacesThursday.IsClosed = dataInfo.PlacesThursday.IsClosed;
                // Friday
                dataValue.PlacesFriday!.HourStart = dataInfo.PlacesFriday!.HourStart;
                dataValue.PlacesFriday!.HourEnd = dataInfo.PlacesFriday.HourEnd;
                dataValue.PlacesFriday.IsClosed = dataInfo.PlacesFriday.IsClosed;
                // Saturday
                dataValue.PlacesSaturday!.HourStart = dataInfo.PlacesSaturday!.HourStart;
                dataValue.PlacesSaturday!.HourEnd = dataInfo.PlacesSaturday!.HourEnd;
                dataValue.PlacesSaturday.IsClosed = dataInfo.PlacesSaturday!.IsClosed;
                // Sunday
                dataValue.PlacesSunday!.HourStart = dataInfo.PlacesSunday!.HourStart;
                dataValue.PlacesSunday!.HourEnd = dataInfo.PlacesSunday!.HourEnd;
                dataValue.PlacesSunday.IsClosed = dataInfo.PlacesSunday.IsClosed;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        public async Task<PlaceImages> AddPlaceImageAsync(PlaceImages dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<PlaceImages> DeletePlaceImageAsync(Guid id)
        {
            var tickets = await mAAP_Context.PlaceImages!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.PlaceImages!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<PlaceFacilities> AddPlaceFacilitiesAsync(PlaceFacilities dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<PlaceFacilities> DeletePlaceFacilitiesAsync(Guid id)
        {
            var tickets = await mAAP_Context.PlaceFacilities!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.PlaceFacilities!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<PlaceWhatToExpect> AddPlaceWhatToExpectAsync(PlaceWhatToExpect dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<PlaceWhatToExpect> DeletePlaceWhatToExpectAsync(Guid id)
        {
            var tickets = await mAAP_Context.PlaceWhatToExpects!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.PlaceWhatToExpects!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<PlaceReviewModel> AddPlaceReviewAsync(PlaceReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<PlaceReviewModel> DeletePlaceReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.PlaceReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.PlaceReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        // Places Chat
        public async Task<IEnumerable<PlacesChatModel>> GetAllPlacesChatAsync()
        {
            return await mAAP_Context.PlacesChatModels!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<PlacesChatModel> AddPlacesChatAsync(PlacesChatModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<PlacesChatModel> DeletePlacesChatAsync(Guid id)
        {
            var tickets = await mAAP_Context.PlacesChatModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.PlacesChatModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<PlacesChatModel> UpdatePlacesChatAsync(Guid id, PlacesChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.PlacesChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<PlacesChatModel> UpdatePlacesIsReadChatAsync(Guid id, PlacesChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.PlacesChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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
