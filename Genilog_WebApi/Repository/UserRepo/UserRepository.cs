using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.UsersDataModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.UserRepo
{
    public class UserRepository(Genilog_Data_Context maap_Context) : IUserRepository
    {
        private readonly Genilog_Data_Context maap_Context = maap_Context;

        public async Task<UsersDataModelTable> AddAsync(UsersDataModelTable subTable)
        {

            //  subTable.Id = Guid.NewGuid();
            await maap_Context.AddAsync(subTable);
            await maap_Context.SaveChangesAsync();
            return subTable;
        }

#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        public async Task<UsersDataModelTable?> DeleteAsync(Guid id)
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        {
            var subTable = await maap_Context.UsersDataModelTables!
                .FirstOrDefaultAsync(x => x.Id == id);
            if (subTable == null)
            {
                return null;
            }
            else
            {
                // Delete Region
                maap_Context.UsersDataModelTables!.Remove(subTable);
                await maap_Context.SaveChangesAsync();
                return subTable;
            }

        }

        public async Task<IEnumerable<UsersDataModelTable>> GetAllAsync()
        {
            return await maap_Context.UsersDataModelTables!.
            Include(x => x.DeliveryAddresses).
            OrderBy(x => x.CreatedAt)
            .ToListAsync(); ;
        }

        public async Task<UsersDataModelTable> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await maap_Context.UsersDataModelTables!
           .Include(x => x.DeliveryAddresses)
            .FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        public async Task<UsersDataModelTable?> UpdateAsync(Guid id, UsersDataModelTable user)
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        {
            var existinguser = await maap_Context.UsersDataModelTables!.FirstOrDefaultAsync(x => x.Id == id);

            if (existinguser == null)
            {
                return null;
            }
            else
            {
                existinguser.Sex = user.Sex;
                existinguser.FirstName = user.FirstName;
                existinguser.LastName = user.LastName;
                existinguser.ProfilePicture = user.ProfilePicture;
                existinguser.PhoneNo = user.PhoneNo;
                existinguser.Address = user.Address;
                existinguser.Locality = user.Locality;
                existinguser.State = user.State;
                existinguser.PostCodes = user.PostCodes;
                existinguser.Latitude = user.Latitude;
                existinguser.Longitude = user.Longitude;
                existinguser.UserStatus = user.UserStatus!;
                existinguser.LastSeenAt = user.LastSeenAt;
                existinguser.LastLoginAt = user.LastLoginAt;
                existinguser.MoneyBoxBalance = user.MoneyBoxBalance;
                existinguser.BankName = user.BankName;
                existinguser.AccountNumber = user.AccountNumber;
                existinguser.AccountName = user.AccountName;
                await maap_Context.SaveChangesAsync();
                return existinguser;
            }
        }
       
        // Delivery Address
        public async Task<IEnumerable<DeliveryAddress>> GetAllDeliveryAsync()
        {
            return await maap_Context.DeliveryAddresses!.
            OrderBy(x => x.CreatedAt)
            .ToListAsync(); ;
        }
        public async Task<DeliveryAddress> GetAddressAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await maap_Context.DeliveryAddresses!
            .FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<DeliveryAddress> AddDeliveryAddressAsync(DeliveryAddress deliveryAddresses)
        {
            deliveryAddresses.Id = Guid.NewGuid();
            await maap_Context.AddAsync(deliveryAddresses);
            await maap_Context.SaveChangesAsync();
            return deliveryAddresses;
        }

        public async Task<DeliveryAddress> UpdateDeliveryAddressAsync(Guid id, DeliveryAddress deliveryAddress)
        {
            var existingDeliveryAddress = await maap_Context.DeliveryAddresses!.FirstOrDefaultAsync(x => x.Id == id);
            if (existingDeliveryAddress == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                existingDeliveryAddress.Address = deliveryAddress.Address;
                existingDeliveryAddress.AddressPostCodes = deliveryAddress.AddressPostCodes;
                existingDeliveryAddress.HouseNo = deliveryAddress.HouseNo;
                existingDeliveryAddress.Locality = deliveryAddress.Locality;
                existingDeliveryAddress.Latitude = deliveryAddress.Latitude;
                existingDeliveryAddress.Longitude = deliveryAddress.Longitude;
                await maap_Context.SaveChangesAsync();
                return existingDeliveryAddress;
            }
        }

        public async Task<DeliveryAddress> DeleteDeliveryAddressAsync(Guid id)
        {
            var deliveryAddresses = await maap_Context.DeliveryAddresses!.FirstOrDefaultAsync(x => x.Id == id);
            if (deliveryAddresses == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                maap_Context.DeliveryAddresses!.Remove(deliveryAddresses);
                await maap_Context.SaveChangesAsync();
                return deliveryAddresses;
            }
        }
    }
}
