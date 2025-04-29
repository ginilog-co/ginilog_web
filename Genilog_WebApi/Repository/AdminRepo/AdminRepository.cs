using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AdminsModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.AdminRepo
{
    public class AdminRepository(Genilog_Data_Context maap_Context, IWebHostEnvironment environment) : IAdminRepository
    {
        private readonly Genilog_Data_Context maap_Context = maap_Context;
        private readonly IWebHostEnvironment environment = environment;

        public async Task<AdminModelTable> AddAsync(AdminModelTable users)
        {
            await maap_Context.AdminModelTables!.AddAsync(users);
            await maap_Context.SaveChangesAsync();
            return users;

        }

        public async Task<AdminModelTable> DeleteAsync(Guid id)
        {
            var users = await maap_Context.AdminModelTables!.FirstOrDefaultAsync(x => x.Id == id);
            if (users == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                maap_Context.AdminModelTables!.Remove(users);
                await maap_Context.SaveChangesAsync();
                return users;
            }
        }

        public async Task<IEnumerable<AdminModelTable>> GetAllAsync()
        {
            return await maap_Context.AdminModelTables!.ToListAsync();
        }

        public async Task<AdminModelTable> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await maap_Context.AdminModelTables!.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<AdminModelTable> UpdateAsync(Guid id, AdminModelTable user)
        {
            var existinguser = await maap_Context.AdminModelTables!.FirstOrDefaultAsync(x => x.Id == id);

            if (existinguser == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                existinguser.SurName = user.SurName;
                existinguser.FirstName = user.FirstName;
                existinguser.PhoneNo = user.PhoneNo;
                existinguser.StaffCode = user.StaffCode;
                existinguser.State = user.State;
                existinguser.Locality = user.Locality;
                existinguser.Branch = user.Branch;
                existinguser.Address = user.Address;
                // Upload Image
                existinguser.ImagePath = user.ImagePath;
                await maap_Context.SaveChangesAsync();
                return existinguser;
            }
        }

        // ADVERT LINE
        public async Task<AdvertHolderModel> AddAdvertAsync(AdvertHolderModel users)
        {
            users.Id=Guid.NewGuid();
            await maap_Context.AdvertHolderModels!.AddAsync(users);
            await maap_Context.SaveChangesAsync();
            return users;

        }

        public async Task<AdvertHolderModel> DeleteAdvertAsync(Guid id)
        {
            var users = await maap_Context.AdvertHolderModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (users == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                maap_Context.AdvertHolderModels!.Remove(users);
                await maap_Context.SaveChangesAsync();
                return users;
            }
        }

        public async Task<IEnumerable<AdvertHolderModel>> GetAllAdvertAsync()
        {
            return await maap_Context.AdvertHolderModels!.ToListAsync();
        }

        public async Task<AdvertHolderModel> GetAdvertAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await maap_Context.AdvertHolderModels!.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<AdvertHolderModel> UpdateAdvertAsync(Guid id, AdvertHolderModel user)
        {
            var existinguser = await maap_Context.AdvertHolderModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (existinguser == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                existinguser.AdvertImage = user.AdvertImage;
                existinguser.AdvertName = user.AdvertName;
                existinguser.AdvertType = user.AdvertType;
                existinguser.AdvertItemDescription = user.AdvertItemDescription;
                existinguser.AdvertItemCost = user.AdvertItemCost;
                existinguser.AdvertDays4 = user.AdvertDays4;
                await maap_Context.SaveChangesAsync();
                return existinguser;
            }
        }
    }
}
