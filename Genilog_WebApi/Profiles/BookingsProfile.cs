using AutoMapper;
using Genilog_WebApi.Model.BookingsModel;

namespace Genilog_WebApi.Profiles
{
    public class BookingsProfile : Profile
    {
        public BookingsProfile()
        {
            CreateMap<HotelDataModel, HotelDataModelDto>()
                .ReverseMap();
            CreateMap<HotelImages, HotelImagesDto>()
             .ReverseMap();
            CreateMap<HotelFacilities, HotelFacilitiesDto>()
             .ReverseMap();
            CreateMap<HotelReviewModel, HotelReviewModelDto>()
             .ReverseMap();
            CreateMap<HotelMondayModel, HotelMondayModelDto>()
         .ReverseMap();

            CreateMap<HotelTuesdayModel, HotelTuesdayModelDto>()
                .ReverseMap();

            CreateMap<HotelWednesdayModel, HotelWednesdayModelDto>()
                .ReverseMap();

            CreateMap<HotelThursdayModel, HotelThursdayModelDto>()
                .ReverseMap();

            CreateMap<HotelFridayModel, HotelFridayModelDto>()
                .ReverseMap();

            CreateMap<HotelSaturdayModel, HotelSaturdayModelDto>()
                .ReverseMap();

            CreateMap<HotelSundayModel, HotelSundayModelDto>()
                .ReverseMap();

            CreateMap<AirlineDataModel, AirlineDataModelDto>()
              .ReverseMap();
            CreateMap<AirlineImages, AirlineImagesDto>()
             .ReverseMap();
            CreateMap<AirCraftList, AirCraftListDto>()
             .ReverseMap();
            CreateMap<AirlinePayment, AirlinePaymentDto>()
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
