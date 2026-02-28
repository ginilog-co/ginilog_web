using AutoMapper;
using Genilog_WebApi.Model.WalletModel;

namespace Genilog_WebApi.Profiles
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<TransactionDataModel, TransactionDataModelDto>()
                .ReverseMap();
        }
    }
}
