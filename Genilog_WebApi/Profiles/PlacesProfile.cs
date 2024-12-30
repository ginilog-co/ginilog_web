using AutoMapper;
using Genilog_WebApi.Model.PlacesModel;

namespace Genilog_WebApi.Profiles
{
    public class PlacesProfile : Profile
    {
        public PlacesProfile()
        {
            CreateMap<HotelDataModel, HotelDataTableDto>()
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

            CreateMap<PlacesDataModel, PlacesDataModelDto>()
              .ReverseMap();
            CreateMap<PlaceImages, PlaceImagesDto>()
             .ReverseMap();
            CreateMap<PlaceFacilities, PlaceFacilitiesDto>()
             .ReverseMap();
            CreateMap<PlaceWhatToExpect, PlaceWhatToExpectDto>()
             .ReverseMap();
            CreateMap<PlaceReviewModel, PlaceReviewModelDto>()
             .ReverseMap();
            CreateMap<PlacesMondayModel, PlacesMondayModelDto>()
           .ReverseMap();

            CreateMap<PlacesTuesdayModel, PlacesTuesdayModelDto>()
                .ReverseMap();

            CreateMap<PlacesWednesdayModel, PlacesWednesdayModelDto>()
                .ReverseMap();

            CreateMap<PlacesThursdayModel, PlacesThursdayModelDto>()
                .ReverseMap();

            CreateMap<PlacesFridayModel, PlacesFridayModelDto>()
                .ReverseMap();

            CreateMap<PlacesSaturdayModel, PlacesSaturdayModelDto>()
                .ReverseMap();

            CreateMap<PlacesSundayModel, PlacesSundayModelDto>()
                .ReverseMap();

        }
    }
}
