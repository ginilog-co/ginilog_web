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
    }
}
