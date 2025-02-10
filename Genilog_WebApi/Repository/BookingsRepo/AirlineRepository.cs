using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.BookingsModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.BookingsRepo
{
    public class AirlineRepository(Genilog_Data_Context mAAP_Context) : IAirlineRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<AirlineDataModel> AddAsync(AirlineDataModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlineDataModel> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlineDataModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlineDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<AirlineDataModel>> GetAllAsync()
        {
            return await mAAP_Context.AirlineDataModels!
                  .Include(x => x.AirlineImages)
                  .Include(x => x.AirCraftList)
                  .Include(x => x.AirlinePayments)
                  .Include(x => x.AirLineServiceLocations)
                  .Include(x => x.AirlineReviewModels)
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }


        public async Task<AirlineDataModel> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.AirlineDataModels!
                  .Include(x => x.AirlineImages)
                  .Include(x => x.AirCraftList)
                   .Include(x => x.AirLineServiceLocations)
                  .Include(x => x.AirlinePayments)
                  .Include(x => x.AirlineReviewModels).
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<AirlineDataModel> UpdateAsync(Guid id, AirlineDataModel dataInfo)
        {
            var dataValue = await mAAP_Context.AirlineDataModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.AirlineName = dataInfo.AirlineName;
                dataValue.AirlineEmail = dataInfo.AirlineEmail;
                dataValue.AirlineInfo = dataInfo.AirlineInfo;
                dataValue.AirlineType = dataInfo.AirlineType;
                dataValue.AirlineWebsite = dataInfo.AirlineWebsite;
                dataValue.AirlinePhoneNo = dataInfo.AirlinePhoneNo;
                dataValue.State = dataInfo.State;
                dataValue.Locality = dataInfo.Locality;
                dataValue.Country = dataInfo.Country;
                dataValue.Rating = dataValue.Rating;
                dataValue.BookingAmount = dataInfo.BookingAmount;
                dataValue.Available = dataInfo.Available;
                dataValue.AirlineLogo = dataInfo.AirlineLogo;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }


        public async Task<AirlineImages> AddAirlineImageAsync(AirlineImages dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlineImages> DeleteAirlineImageAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlineImages!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlineImages!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirCraftList> AddAirCraftListAsync(AirCraftList dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirCraftList> DeleteAirCraftListAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirCraftList!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirCraftList!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirlinePayment> AddAirlinePaymentAsync(AirlinePayment dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlinePayment> DeleteAirlinePaymentAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlinePayments!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlinePayments!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirlineReviewModel> AddAirlineReviewAsync(AirlineReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlineReviewModel> DeleteAirlineReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlineReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlineReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirLineServiceLocation> AddAirLineServiceLocationAsync(AirLineServiceLocation dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirLineServiceLocation> DeleteAirLineServiceLocationAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirLineServiceLocations!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirLineServiceLocations!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        // Airline Ticket
        public async Task<IEnumerable<FlightTicketBookModel>> GetAllAirlineFlightTicketAsync()
        {
            return await mAAP_Context.FlightTicketBookModels!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<FlightTicketBookModel> AddAirlineFlightTicketAsync(FlightTicketBookModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<FlightTicketBookModel> GetAirlineFlightTicketAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.FlightTicketBookModels!.
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<FlightTicketBookModel> DeleteAirlineFlightTicketAsync(Guid id)
        {
            var tickets = await mAAP_Context.FlightTicketBookModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.FlightTicketBookModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<FlightTicketBookModel> UpdateAirlineFlightTicketAsync(Guid id, FlightTicketBookModel dataInfo)
        {
            var dataValue = await mAAP_Context.FlightTicketBookModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.DapatureTime = dataInfo.DapatureTime;
                dataValue.AvailabeTimeInterval = dataInfo.AvailabeTimeInterval;
                dataValue.Dapature = dataInfo.Dapature;
                dataValue.Destination = dataInfo.Destination;
                dataValue.Available = dataInfo.Available;
                dataValue.IsReturn = dataInfo.IsReturn;
                dataValue.TicketPrice = dataInfo.TicketPrice;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        // Places Chat
        public async Task<IEnumerable<AirlineChatModel>> GetAllAirlineChatAsync()
        {
            return await mAAP_Context.AirlineChatModels!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<AirlineChatModel> AddAirlineChatAsync(AirlineChatModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlineChatModel> DeleteAirlineChatAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlineChatModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlineChatModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirlineChatModel> UpdateAirlineChatAsync(Guid id, AirlineChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.AirlineChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<AirlineChatModel> UpdateAirlinesIsReadChatAsync(Guid id, AirlineChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.AirlineChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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
