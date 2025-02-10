using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<CompanyModelData>> GetAllAsync();
        Task<CompanyModelData> GetAsync(Guid id);
        Task<CompanyModelData> AddAsync(CompanyModelData region);
        Task<CompanyModelData> DeleteAsync(Guid id);
        Task<CompanyModelData> UpdateAsync(Guid id, CompanyModelData region);

        // Company Review
        Task<CompanyReviewModel> AddCompanyReviewAsync(CompanyReviewModel region);
        Task<CompanyReviewModel> DeleteCompanyReviewAsync(Guid id);


        // Order
        Task<IEnumerable<OrderModelData>> GetAllOrderAsync();
        Task<OrderModelData> GetOrderAsync(Guid id);
        Task<OrderModelData> AddOrderAsync(OrderModelData region);
        Task<OrderModelData> DeleteOrderAsync(Guid id);
        Task<OrderModelData> UpdateOrderAsync(Guid id, OrderModelData region);
        Task<OrderModelData> AssignRiderAsync(Guid id, OrderModelData region);
        // Order Image List
        Task<PackageImageList> AddPackageImagesAsync(PackageImageList region);
        Task<PackageImageList> DeletePackageImagesAsync(Guid id);
    }
}
