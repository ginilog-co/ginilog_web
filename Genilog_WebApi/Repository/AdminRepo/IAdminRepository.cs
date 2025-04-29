
using Genilog_WebApi.Model.AdminsModel;

namespace Genilog_WebApi.Repository.AdminRepo
{
    public interface IAdminRepository
    {
        Task<IEnumerable<AdminModelTable>> GetAllAsync();
        Task<AdminModelTable> GetAsync(Guid id);
        Task<AdminModelTable> AddAsync(AdminModelTable region);
        Task<AdminModelTable> DeleteAsync(Guid id);
        Task<AdminModelTable> UpdateAsync(Guid id, AdminModelTable region);
        // ADVERT LINE
        Task<IEnumerable<AdvertHolderModel>> GetAllAdvertAsync();
        Task<AdvertHolderModel> GetAdvertAsync(Guid id);
        Task<AdvertHolderModel> AddAdvertAsync(AdvertHolderModel region);
        Task<AdvertHolderModel> DeleteAdvertAsync(Guid id);
        Task<AdvertHolderModel> UpdateAdvertAsync(Guid id, AdvertHolderModel region);
    }
}
