using AutoMapper;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.AdminsModel;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Repository.GeneralRepo;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.AdminRepo
{
    public class AdminRepository(Genilog_Data_Context maap_Context, IMapper mapper) : IAdminRepository
    {
        private readonly Genilog_Data_Context maap_Context = maap_Context;
        private readonly IMapper mapper = mapper;

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
            return await maap_Context.AdminModelTables!.AsNoTracking().ToListAsync();
        }

        public async Task<PageModel<AdminModelTableDto>> GetAllAdminAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = maap_Context.AdminModelTables!
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .AsQueryable();


            // 2. Apply filters
            if (!string.IsNullOrWhiteSpace(filter.State))
            {
                string any = filter.State;
                query = query.Where(x =>
                    EF.Functions.Like(x.State, $"%{any}%") 
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.Locality))
            {
                string any = filter.Locality;
                query = query.Where(x =>
                    EF.Functions.Like(x.Locality, $"%{any}%")
                );
            }
              

            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                string any = filter.UserId;
                query = query.Where(x =>
                    EF.Functions.Like(x.ManagerId.ToString(), $"%{any}%") 
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterTypes))
            {
                string any = filter.FilterTypes;
                query = query.Where(x =>
                    EF.Functions.Like(x.AdminType, $"%{any}%") ||
                     x.CompanyType != null && x.CompanyType.Any(d =>
                     d.Contains(any, StringComparison.CurrentCultureIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.FirstName, $"%{any}%") ||
                    EF.Functions.Like(x.SurName, $"%{any}%") ||
                    EF.Functions.Like(x.Email, $"%{any}%") ||
                    EF.Functions.Like(x.PhoneNo, $"%{any}%") ||
                    EF.Functions.Like(x.Sex, $"%{any}%") ||
                    EF.Functions.Like(x.Branch, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyName, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyUserName, $"%{any}%")
                );
            }
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                var from = filter.StartDate.Value.Date;
                var to = filter.EndDate.Value.Date;

                // swap if reversed
                if (from > to)
                    (from, to) = (to, from);



                var fromDate = from;
                var toDate = to.AddDays(1);

                query = query.Where(s => s.CreatedAt >= fromDate && s.CreatedAt < toDate);

                // query = query.Where(s => s.CreatedAt.Date >= from && s.CreatedAt.Date <= to);
            }

            // 3. Pagination on the entity query
            var pageSize = Math.Clamp(filter.PageSize ?? 20, 1, 20);
            var page = filter.Page is null or <= 0 ? 1 : filter.Page.Value;
            var pagedData = await query.ToPagedAsync(page, pageSize);


            // 4. Map to DTO after retrieving paged data
            var userDto = mapper.Map<List<AdminModelTableDto>>(pagedData.Data);
            userDto = [.. userDto.OrderByDescending(o => o.CreatedAt)];


            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<AdminModelTableDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize
            );
        }

        public async Task<AdminModelTable> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await maap_Context.AdminModelTables!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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
                existinguser.CompanyName = user.CompanyName;
                existinguser.CompanyUserName = user.CompanyUserName;
                existinguser.CompanyType = user.CompanyType;
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
            return await maap_Context.AdvertHolderModels!.AsNoTracking().ToListAsync();
        }

        public async Task<AdvertHolderModel> GetAdvertAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await maap_Context.AdvertHolderModels!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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
                existinguser.TransRef = user.TransRef;
                existinguser.TransStatus = user.TransStatus;
                existinguser.ExpiredAt = user.ExpiredAt;
                await maap_Context.SaveChangesAsync();
                return existinguser;
            }
        }

        // COMPANY Apply
        public async Task<CompanyApplyDataModel> AddCompanyApplyAsync(CompanyApplyDataModel users)
        {
            users.Id = Guid.NewGuid();
            await maap_Context.CompanyApplyDataModels!.AddAsync(users);
            await maap_Context.SaveChangesAsync();
            return users;
        }

        public async Task<CompanyApplyDataModel> DeleteCompanyApplyAsync(Guid id)
        {
            var users = await maap_Context.CompanyApplyDataModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (users == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                maap_Context.CompanyApplyDataModels!.Remove(users);
                await maap_Context.SaveChangesAsync();
                return users;
            }
        }

        public async Task<IEnumerable<CompanyApplyDataModel>> GetAllCompanyApplyAsync()
        {
            return await maap_Context.CompanyApplyDataModels!.AsNoTracking().ToListAsync();
        }

        public async Task<PageModel<CompanyApplyDataModelDto>> GetAllPaginatedCompanyApplyAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = maap_Context.CompanyApplyDataModels!
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FilterTypes))
            {
                string any = filter.FilterTypes;
                query = query.Where(x =>
                 
                     x.CompanyType != null && x.CompanyType.Any(d =>
                     d.Contains(any, StringComparison.CurrentCultureIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.FirstName, $"%{any}%") ||
                    EF.Functions.Like(x.SurName, $"%{any}%") ||
                    EF.Functions.Like(x.Email, $"%{any}%") ||
                    EF.Functions.Like(x.PhoneNo, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyName, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyUserName, $"%{any}%")
                );
            }
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                var from = filter.StartDate.Value.Date;
                var to = filter.EndDate.Value.Date;

                // swap if reversed
                if (from > to)
                    (from, to) = (to, from);



                var fromDate = from;
                var toDate = to.AddDays(1);

                query = query.Where(s => s.CreatedAt >= fromDate && s.CreatedAt < toDate);

                // query = query.Where(s => s.CreatedAt.Date >= from && s.CreatedAt.Date <= to);
            }

            // 3. Pagination on the entity query
            var pageSize = Math.Clamp(filter.PageSize ?? 20, 1, 20);
            var page = filter.Page is null or <= 0 ? 1 : filter.Page.Value;
            var pagedData = await query.ToPagedAsync(page, pageSize);


            // 4. Map to DTO after retrieving paged data
            var userDto = mapper.Map<List<CompanyApplyDataModelDto>>(pagedData.Data);
            userDto = [.. userDto.OrderByDescending(o => o.CreatedAt)];


            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<CompanyApplyDataModelDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize
            );
        }

        public async Task<CompanyApplyDataModel> GetCompanyApplyAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await maap_Context.CompanyApplyDataModels!.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<CompanyApplyDataModel> UpdateCompanyApplyAsync(Guid id, CompanyApplyDataModel user)
        {
            var existinguser = await maap_Context.CompanyApplyDataModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (existinguser == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                existinguser.Email = user.Email;
                existinguser.SurName = user.SurName;
                existinguser.FirstName = user.FirstName;
                existinguser.PhoneNo = user.PhoneNo;
                existinguser.CompanyName = user.CompanyName;
                existinguser.CompanyUserName = user.CompanyUserName;
                existinguser.CompanyAddress = user.CompanyAddress;
                existinguser.CompanyType = user.CompanyType;
                await maap_Context.SaveChangesAsync();
                return existinguser;

            }
        }
    }
}
