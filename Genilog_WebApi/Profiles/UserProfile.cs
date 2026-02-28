using AutoMapper;
using Genilog_WebApi.Model.UsersDataModel;

namespace Genilog_WebApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UsersDataModelTable, UsersDataModelTableDto>()
                .ReverseMap();
            CreateMap<DeliveryAddress, DeliveryAddressDto>()
                .ReverseMap();

        }
    }
}
