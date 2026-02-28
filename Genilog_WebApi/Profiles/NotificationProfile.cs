using AutoMapper;
using Genilog_WebApi.Model.Notification_Model;

namespace Genilog_WebApi.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationModel, NotificationModelDto>()
                .ReverseMap();
        }
    }
}
