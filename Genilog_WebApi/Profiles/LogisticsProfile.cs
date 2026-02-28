using AutoMapper;
using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Profiles
{
    public class LogisticsProfile : Profile
    {
        public LogisticsProfile()
        {
            CreateMap<RidersModelData, RidersModelDataDto>()
                .ReverseMap();
            CreateMap<CompanyModelData, CompanyModelDataDto>()
                .ReverseMap();

            CreateMap<RidersReviewModel, RidersReviewModelDto>()
               .ReverseMap();
            CreateMap<CompanyReviewModel, CompanyReviewModelDto>()
               .ReverseMap();

            CreateMap<OrderModelData, OrderModelDataDto>()
              .ReverseMap();
            CreateMap<OrderDeliveryFlow, OrderDeliveryFlowDto>()
              .ReverseMap();

            CreateMap<RidersChatModelData, RidersChatModelDataDto>()
           .ReverseMap();

        }
    }
}
