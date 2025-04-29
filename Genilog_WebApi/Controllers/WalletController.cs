using AutoMapper;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.WalletModel;
using Genilog_WebApi.Repository.LogisticsRepo;
using Genilog_WebApi.Repository.NotificationRepo;
using Genilog_WebApi.Repository.WalletRepo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController(IHostEnvironment _env, IMapper mapper, IRidersRepository ridersRepository,
        IWalletRepository walletRepository) : ControllerBase
    {
        private readonly IHostEnvironment _env = _env;
        private readonly IMapper mapper = mapper;
        private readonly IRidersRepository ridersRepository = ridersRepository;
        private readonly IWalletRepository walletRepository = walletRepository;


        [HttpGet("payout-transfer")]
        [Authorize(Roles = "Station_User,Rider,Super_Admin,Admin")]
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
        [Authorize(Roles = "Station_User,Rider,Super_Admin,Admin")]
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
        [Authorize(Roles = "Rider,Super_Admin,Admin")]
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
            
            
            var contacts = await walletRepository.DeletePayoutAsync(id);
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

        [HttpPut("payout-transfer/{id:guid}")]
        [Authorize(Roles = "Super_Admin,Admin")]
        public async Task<IActionResult> UpdatePayoutAsync([FromRoute] Guid id, AddPayout request)
        {
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
                var theUser = await ridersRepository.GetAsync(stationId);
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
                // convert back to dto
                var userDto = new ResponseModel()
                {
                    Message = "Updated Successfully",
                    Status = true,

                };
                return Ok(userDto);
            }
        }

        //paystack
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializePayment([FromBody] PaymentRequest paymentRequest)
        {
            var url = "https://api.paystack.co/transaction/initialize";
            var data = new
            {
                email = paymentRequest.Email,
                amount = paymentRequest.Amount * 100,  // Amount in Kobo (100 kobo = 1 Naira)
                callback_url = $"{Cls_Keys.ServerURL}/api/Wallet/verify", // The URL to redirect after payment
                channels = new[] { "card", "bank", "ussd", "mobile_money", "bank_transfer" },
                metadata = new
                {
                    cancel_action = $"{Cls_Keys.ServerURL}/api/paystack-redirect?status=cancelled"
                }
            };

            using var httpClient = new HttpClient();

            StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.PaystackSecretKey);
            using var response = await httpClient.PostAsync($"{url}", content);
            string apiResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(apiResponse);
                return Ok(paystackResponse);
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
            }

        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyPayment([FromQuery] string trxref, [FromQuery] string reference)
        {
            var url = $"https://api.paystack.co/transaction/verify/{reference}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.PaystackSecretKey);


            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.PaystackSecretKey);
            using var response = await httpClient.GetAsync($"{url}");
            string apiResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paystackResponse = new PaymentSucessData()
                {
                    Status = true,
                    Message = "Payment Made Sucessfully",
                    AccessCode = trxref,
                    Reference = reference,
                    PaymentStatus=""
                };
                return Ok(apiResponse);
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
            }
        }

        //flutterwave
        [HttpPost("initialize-flutterwave")]
        public async Task<IActionResult> InitializeFlutterwavePayment([FromBody] PaymentRequest paymentRequest)
        {
            var url = "https://api.flutterwave.com/v3/payments";
            var data = new
            {
                tx_ref = CreateRandomTokenSix(),
                amount = paymentRequest.Amount,  // Amount in Kobo (100 kobo = 1 Naira)
                customer = new
                {
                    email = paymentRequest.Email,
                    name = paymentRequest.FullName
                },
                currency = "NGN",
                redirect_url = $"{Cls_Keys.ServerURL}/api/Wallet/verify-flutterwave", // The URL to redirect after payment
                customizations = new
                {
                    title = "My App Payment",
                    description = "Payment for items in cart"
                }
            };

            using var httpClient = new HttpClient();

            StringContent content = new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.FlutterwaveSecretKey);
            using var response = await httpClient.PostAsync($"{url}", content);
            string apiResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paystackResponse = JsonConvert.DeserializeObject<FlutterwaveResponse>(apiResponse);
                return Ok(paystackResponse);
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
            }

        }

        [HttpGet("verify-flutterwave")]
        public async Task<IActionResult> VerifyFlutterwavePayment([FromQuery] string status, [FromQuery] string tx_ref, [FromQuery] string transaction_id)
        {
            var url = $"https://api.flutterwave.com/v3/transactions/{transaction_id}/verify";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.FlutterwaveSecretKey);


            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Cls_Keys.FlutterwaveSecretKey);
            using var response = await httpClient.GetAsync($"{url}");
            string apiResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var paystackResponse = new PaymentSucessData()
                {
                    Status = true,
                    Message = "Payment Made Sucessfully",
                    AccessCode = transaction_id,
                    Reference = tx_ref,
                    PaymentStatus=status
                };
                if (paystackResponse.PaymentStatus == "successful")
                {
                    // TODO: Mark payment as successful in DB
                    return Redirect($"{Cls_Keys.ServerURL}/api/flutterwave-redirect?status={paystackResponse.PaymentStatus}");
                }
                else
                {
                    return Redirect($"{Cls_Keys.ServerURL}/api/flutterwave-redirect?status={paystackResponse.PaymentStatus}");
                }
            }
            else
            {
                var error = new ErrorModel()
                {
                    Message = $"{apiResponse}",
                    Status = true
                };
                return BadRequest(error);
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

        private static string CreateRandomTokenSix()
        {
            char[] charArr = "ABCDEFGHIJKLMNOPQLSTUVWXYZ0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new();
            for (int i = 0; i < 6; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos)!.ToString()!)) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }

        #endregion
    }
}
