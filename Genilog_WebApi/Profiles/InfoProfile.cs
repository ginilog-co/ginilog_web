using AutoMapper;
using Genilog_WebApi.Model.InfoModel;

namespace Genilog_WebApi.Profiles
{
    public class InfoProfile : Profile
    {
        public InfoProfile()
        {
            CreateMap<FeedbackModelData, FeedbackModelDataDto>()
                .ReverseMap();
        }
    }
}
