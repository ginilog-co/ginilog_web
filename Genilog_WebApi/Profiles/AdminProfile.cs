using AutoMapper;
using Genilog_WebApi.Model.AdminsModel;

namespace Genilog_WebApi.Profiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<AdminModelTable, AdminModelTableDto>()
                .ReverseMap();

        }
    }
}
