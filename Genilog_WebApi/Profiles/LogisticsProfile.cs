using AutoMapper;
using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Profiles
{
    public class LogisticsProfile : Profile
    {
        public LogisticsProfile()
        {
            CreateMap<LogisticsDataModel, LogisticsDataModelDto>()
                .ReverseMap();

            CreateMap<LogisticsReviewModel, LogisticsReviewModelDto>()
               .ReverseMap();

            CreateMap<OrderModelData, OrderModelDataDto>()
              .ReverseMap();

            CreateMap<LogisticsChatModelData, LogisticsChatModelDataDto>()
           .ReverseMap();

        }
    }
}
