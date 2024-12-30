using AutoMapper;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.WalletModel;
using Genilog_WebApi.Repository.LogisticsRepo;
using Genilog_WebApi.Repository.WalletRepo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController(IHostEnvironment _env, IMapper mapper, ILogisticsRepository logisticsRepository,
        IWalletRepository walletRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IMapper mapper = mapper;
        private readonly ILogisticsRepository logisticsRepository = logisticsRepository;
        private readonly IWalletRepository walletRepository = walletRepository;
        readonly string keyPath = Path.Combine(_env.ContentRootPath, "Key\\ginilog-e3c8a-firebase-adminsdk-28ax3-07783858d2json");


        [HttpGet("payout-transfer")]
        [Authorize(Roles = "Station_User,Rider_User,Super_Admin,Admin")]
        public async Task<IActionResult> GetAllRiderPayoutAsync([FromQuery] FilterLocationData data)
        {
            var events = await walletRepository.GetAllPayoutAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality) && !string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality)&&(x.PaymentTo!.Contains(data.AnyItem)||
                x.TransactionReference!.Contains(data.AnyItem) || x.Name!.Contains(data.AnyItem)));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                return Ok(userDto);
            }
           else if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality) )
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.State) &&  !string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && (x.PaymentTo!.Contains(data.AnyItem) ||
                x.TransactionReference!.Contains(data.AnyItem) || x.Name!.Contains(data.AnyItem)));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.State))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.Locality) && !string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality) && (x.PaymentTo!.Contains(data.AnyItem) ||
                x.TransactionReference!.Contains(data.AnyItem) || x.Name!.Contains(data.AnyItem)));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else if (!string.IsNullOrEmpty(data.AnyItem))
            {
                allPosts = allPosts.Where(x =>  x.PaymentTo!.Contains(data.AnyItem) ||
                x.Name!.Contains(data.AnyItem)||
                x.UserId.ToString() == data.AnyItem);
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                return Ok(userDto);
            }
            else
            {
                var userDto = mapper.Map<List<PayoutDataModelDto>>(events);
                return Ok(userDto);
            }
        }

        [HttpGet("payout-statistic")]
        [Authorize(Roles = "Station_User,Rider_User,Super_Admin,Admin")]
        public async Task<IActionResult> GetAllAvailableGasOrderAsync([FromQuery] FilterAllData data)
        {
            var events = await walletRepository.GetAllPayoutAsync();
            events = [.. events.OrderByDescending(x => x.CreatedAt)];
            var allPosts = events.AsQueryable();

            if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider").ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TodaysAmountPayout=todaysAmountPayout,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality) && data.Days != null)
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => ((today - x.CreatedAt).TotalDays < data.Days)).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.State) && !string.IsNullOrEmpty(data.Locality) && data.StartDate != null && data.EndDate != null)
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State) && x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => x.CreatedAt >= data.StartDate && x.CreatedAt <= data.EndDate).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.State))
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);
                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.State) && data.Days != null)
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => ((today - x.CreatedAt).TotalDays < data.Days)).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.State) && data.StartDate != null && data.EndDate != null)
            {
                allPosts = allPosts.Where(x => x.State!.Contains(data.State));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => x.CreatedAt >= data.StartDate && x.CreatedAt <= data.EndDate).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.Locality))
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);
                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.Locality) && data.Days != null)
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => ((today - x.CreatedAt).TotalDays < data.Days)).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.Locality) && data.StartDate != null && data.EndDate != null)
            {
                allPosts = allPosts.Where(x => x.Locality!.Contains(data.Locality));
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => x.CreatedAt >= data.StartDate && x.CreatedAt <= data.EndDate).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (data.Days != null)
            {
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);
                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => ((today - x.CreatedAt).TotalDays < data.Days)).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (data.StartDate != null && data.EndDate != null)
            {
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => x.CreatedAt >= data.StartDate && x.CreatedAt <= data.EndDate).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else if (!string.IsNullOrEmpty(data.UserId))
            {
                var userDto = mapper.Map<List<PayoutDataModelDto>>(allPosts);

                var todaysData = userDto;
                var today = DateTime.UtcNow;
                userDto = userDto.Where(x => x.UserId.ToString() == data.UserId).ToList();

                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }

            else
            {
                var userDto = mapper.Map<List<PayoutDataModelDto>>(events);
                var totalAmountPayout = userDto.Sum(x => x.Amount);
                var todaysPayout = userDto.Where(x => x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todaysAmountPayout = todaysPayout.Sum(x => x.Amount);
                var totalRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" 
                ).ToList();
                var totalRiderAmountPayout = totalRiderPayout.Sum(x => x.Amount);
                var totalLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" 
                 ).ToList();
                var totalLogisticslAmountPayout = totalLogisticsPayout.Sum(x => x.Amount);
                var todaysRiderPayout = userDto.Where(x => x.PaymentTo=="Rider" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayRiderAmountPayout = todaysRiderPayout.Sum(x => x.Amount);
                var todaysLogisticsPayout = userDto.Where(x => x.PaymentTo == "Logistics" && x.CreatedAt.ToString("MM/dd/yyyy") == DateTime.UtcNow.ToString("MM/dd/yyyy")).ToList();
                var todayLogisticsAmountPayout = todaysLogisticsPayout.Sum(x => x.Amount);
                
                var dataModel = new PayoutStatistics()
                {
                    
                    TotalPayout = userDto.Count,
                    TotalAmountPayout = totalAmountPayout,
                    TodaysPayout = todaysPayout.Count,
                    TodaysAmountPayout = todaysAmountPayout,
                    TotalRiderPayout = totalRiderPayout.Count,
                    TotalRiderAmountPayout = totalRiderAmountPayout,
                    TotalLogisticsPayout = totalLogisticsPayout.Count,
                    TotalLogisticslAmountPayout = totalLogisticslAmountPayout,
                    TodaysRiderPayout = todaysRiderPayout.Count,
                    TodayRiderAmountPayout = todayRiderAmountPayout,
                    TodaysLogisticsPayout = todaysLogisticsPayout.Count,
                    TodayLogisticsAmountPayout = todayLogisticsAmountPayout,
                    
                };
                return Ok(dataModel);
            }
        }

        [HttpGet]
        [Route("payout-transfer/{id:guid}")]
        [Authorize(Roles = "Station_User,Rider_User,Super_Admin,Admin")]
        public async Task<IActionResult> GetPayoutAsync([FromRoute] Guid id)
        {
            var contacts = await walletRepository.GetPayoutAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                var contactsDto = mapper.Map<PayoutDataModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpDelete]
        [Route("payout-transfer/{id:guid}")]
        [Authorize(Roles = "Super_Admin")]
        public async Task<IActionResult> DeletePayoutAsync([FromRoute] Guid id)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
            var contacts = await walletRepository.DeletePayoutAsync(id);
            if (contacts == null)
            {
                return BadRequest("Id Does not Exist");
            }
            else
            {
                DocumentReference usrRef = firestoreDb!.Collection("PayoutCollections").Document(contacts.Id.ToString());
                await usrRef.DeleteAsync();
                var contactsDto = mapper.Map<PayoutDataModelDto>(contacts);
                return Ok(contactsDto);
            }
        }

        [HttpPut("payout-transfer/{id:guid}")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public async Task<IActionResult> UpdatePayoutAsync([FromRoute] Guid id, AddPayout request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);

            var user = new PayoutDataModel()
            {
                PayOutStatus = true,
                TransactionReference = request.TransactionReference,
                PayOutDateAt = DateTime.UtcNow,
            };

            // Update detials to repository
            user = await walletRepository.UpdatePayoutAsync(id, user);

            // check the null value
            if (user == null)
            {
                return BadRequest("Lab Does not Exist");
            }
            // convert back to dto
            else
            {
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DocumentReference usrRef = firestoreDb!.Collection("PayoutCollections").Document(user.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"PayOutStatus",user.PayOutStatus!},
                    {"TransactionReference",user.TransactionReference! },
                    {"PayOutDateAt",timeStamp},
                };
                await usrRef.UpdateAsync(user3);

             

                // Update detials to repository
            

                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true,

                };
                return Ok(userDto);
            }
        }

      
        [HttpPost("payout-gas-station-transfer")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public async Task<IActionResult> AddLogisticsPayoutAsync([FromHeader] Guid stationId, [FromBody] AddPayout request)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            var firestoreDb = FirestoreDb.Create(Cls_Keys.ProjectId);
            var check = ValidatePayout(request);

            if (!check)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var date = DateTime.UtcNow.ToString("ddd,MMM d,yyyy");
                var timeStamp = Timestamp.GetCurrentTimestamp();
                DateTime localTime = DateTime.Now;
                TimeZoneInfo nigeriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
                DateTime nigeriaTime = TimeZoneInfo.ConvertTimeFromUtc(localTime.ToUniversalTime(), nigeriaTimeZone);
                var theUser = await logisticsRepository.GetAsync(stationId);
                var contacts = new PayoutDataModel()
                {
                    UserId = theUser.Id,
                    Name = $"{theUser.CompanyName}",
                    Remark = "Payout",
                    Amount = request.Amount,
                    PayOutStatus = true,
                    PaymentTo="Logistics",
                    TransactionReference = request.TransactionReference,
                    PayOutDateAt = nigeriaTime,
                    CreatedAt = nigeriaTime,
                    Address = theUser.Address,
                    Locality = theUser.Locality,
                    State = theUser.State,
                    PostCodes = theUser.PostCodes,
                    Latitude = theUser.Latitude,
                    Longitude = theUser.Longitude,
                };
                // Pass detials to repository
                contacts = await walletRepository.AddPayoutAsync(contacts);
                DocumentReference usrRef = firestoreDb!.Collection("PayoutCollections").Document(contacts.Id.ToString());
                Dictionary<string, object> user3 = new()
                {
                    {"Id",contacts.Id.ToString()},
                    {"UserId",contacts.UserId.ToString()!},
                    {"Name",contacts.Name!},
                    {"Remark",contacts.Remark!},
                    {"Amount",contacts.Amount!},
                    {"PayOutStatus",contacts.PayOutStatus!},
                    {"PaymentTo",contacts.PaymentTo! },
                    {"TransactionReference",contacts.TransactionReference! },
                    {"PayOutDateAt",timeStamp},
                    {"Address",contacts.Address!},
                    {"Locality",contacts.Locality! },
                    {"State",contacts.State! },
                    {"PostCodes",contacts.PostCodes! },
                    {"Latitude",contacts.Latitude! },
                    {"Longitude",contacts.Longitude! },
                    {"DatePublished",date!},
                    {"Timestamp",timeStamp!}
                };
                await usrRef.SetAsync(user3);
                // convert back to dto
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true,

                };
                return Ok(userDto);
            }
        }


        #region private methods

        private bool ValidatePayout(AddPayout request)
        {
            if (request == null)
            {
                ModelState.AddModelError(nameof(request), $"Add  Data Is Required");
                return false;
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;

        }

        #endregion
    }
}
