using AutoMapper;
using Genilog_WebApi.Model.BookingsModel;

namespace Genilog_WebApi.Profiles
{
    public class BookingsProfile : Profile
    {
        public BookingsProfile()
        {
            CreateMap<AccomodationDataModel, AccomodationDataModelDto>()
                .ReverseMap();
            CreateMap<BookAccomodationReservatioModel, BookAccomodationReservatioModelDto>()
             .ReverseMap(); 
            CreateMap<CustomerBookedReservation, CustomerBookedReservationDto>()
             .ReverseMap();
            CreateMap<AccomodationReviewModel, AccomodationReviewModelDto>()
             .ReverseMap();
            CreateMap<AccomodationMondayModel, AccomodationMondayModelDto>()
         .ReverseMap();

            CreateMap<AccomodationTuesdayModel, AccomodationTuesdayModelDto>()
                .ReverseMap();

            CreateMap<AccomodationWednesdayModel, AccomodationWednesdayModelDto>()
                .ReverseMap();

            CreateMap<AccomodationThursdayModel, AccomodationThursdayModelDto>()
                .ReverseMap();

            CreateMap<AccomodationFridayModel, AccomodationFridayModelDto>()
                .ReverseMap();

            CreateMap<AccomodationSaturdayModel, AccomodationSaturdayModelDto>()
                .ReverseMap();

            CreateMap<AccomodationSundayModel, AccomodationSundayModelDto>()
                .ReverseMap();

            CreateMap<AirlineDataModel, AirlineDataModelDto>()
              .ReverseMap();
            CreateMap<AirCraftList, AirCraftListDto>()
             .ReverseMap();
            CreateMap<AirlineReviewModel, AirlineReviewModelDto>()
             .ReverseMap();
            CreateMap<AirLineServiceLocation, AirLineServiceLocationDto>()
           .ReverseMap();
            CreateMap<FlightTicketBookModel, FlightTicketBookModelDto>()
           .ReverseMap();

        }
    }
}
