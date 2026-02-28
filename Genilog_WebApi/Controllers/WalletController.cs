using AutoMapper;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.WalletModel;
using Genilog_WebApi.Repository.LogisticsRepo;
using Genilog_WebApi.Repository.WalletRepo;
using Microsoft.AspNetCore.Mvc;
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
