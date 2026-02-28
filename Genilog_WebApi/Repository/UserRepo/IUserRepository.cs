using Genilog_WebApi.Model.UsersDataModel;

namespace Genilog_WebApi.Repository.UserRepo
{
    public interface IUserRepository
    {
        Task<IEnumerable<UsersDataModelTable>> GetAllAsync();
        Task<UsersDataModelTable> GetAsync(Guid id);
        Task<UsersDataModelTable> AddAsync(UsersDataModelTable region);
        Task<UsersDataModelTable> DeleteAsync(Guid id);
        Task<UsersDataModelTable> UpdateAsync(Guid id, UsersDataModelTable region);

        // Delivery Address
        Task<IEnumerable<DeliveryAddress>> GetAllDeliveryAsync();
        Task<DeliveryAddress> GetAddressAsync(Guid id);
        Task<DeliveryAddress> AddDeliveryAddressAsync(DeliveryAddress deliveryAddress);
        Task<DeliveryAddress> UpdateDeliveryAddressAsync(Guid id, DeliveryAddress deliveryAddress);
        Task<DeliveryAddress> DeleteDeliveryAddressAsync(Guid id);
    }
}
