using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class SmsService : ISmsService, IScopedDependency
    {
        private readonly ILogger<SmsService> logger;
        private readonly IBaseRepository<SmsVerification> smsVerificatioRepository;
        private readonly IUserRepository userRepository;
        private readonly string domain;
        private readonly string API_KEY = "7A374A6947715644585A35324D45562F792F49334233767473526C6659624D4E7A354251472F37344F73773D";

        public SmsService(ILogger<SmsService> logger, IBaseRepository<SmsVerification> smsVerificatioRepository, IUserRepository userRepository, IOptions<SiteSettings> siteSettings)
        {
            this.logger = logger;
            this.smsVerificatioRepository = smsVerificatioRepository;
            this.userRepository = userRepository;
            domain = siteSettings.Value.Domain;
        }

        public async Task<bool> Send(string mobile, string message, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl =
                        $"https://api.kavenegar.com/v1/{API_KEY}/sms/send.json?receptor={mobile}&message={WebUtility.UrlEncode($"{message}")}";

                    //string requesturl =
                    //    $"https://api.kavenegar.com/v1/{API_KEY}/verify/lookup.json?receptor={mobile}&token={otp}&token2={domain}&template=SendOtp";

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("return") &&
                        JObject.Parse(resultObject["return"]!.ToString()).ContainsKey("status") &&
                        JObject.Parse(resultObject["return"]!.ToString())["status"]!.ToString().Equals("200", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (resultObject.ContainsKey("entries") &&
                            resultObject["entries"]!.GetType() == typeof(JArray) &&
                            (resultObject["entries"] as JArray)!.Count > 0)
                        {
                            return true;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (resultObject.ContainsKey("return") && resultObject.ContainsKey("status") && resultObject.ContainsKey("message"))
                            logger.LogWarning($"error! , status :{resultObject["status"]} , message :{resultObject["message"]} ");

                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }

            throw new AppException("خطا در ارسال رمز پیامک!");
            //return false;
        }
        public async Task<long> SendPaymentLink(string mobile, string paymentInfoId, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    paymentInfoId = paymentInfoId.Replace("/", "____");
                    var url = $"https://{domain}/pay/{paymentInfoId}";
                    var message = $"جهت تکمیل فرآیند خرید روی لینک زیر کلیک کنید تا به صفحه پرداخت هدایت شوید :{Environment.NewLine}{url}";
                    string requesturl =
                        $"https://api.kavenegar.com/v1/{API_KEY}/sms/send.json?receptor={mobile}&message={WebUtility.UrlEncode(message)}";

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("return") &&
                        JObject.Parse(resultObject["return"].ToString()).ContainsKey("status") &&
                        JObject.Parse(resultObject["return"].ToString())["status"].ToString().Equals("200", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (resultObject.ContainsKey("entries") &&
                            resultObject["entries"].GetType() == typeof(JArray) &&
                            (resultObject["entries"] as JArray).Count > 0)
                        {
                            var result = JsonConvert.DeserializeObject<SmsVerificationModel>((resultObject["entries"] as JArray)[0].ToString());

                            return result.MessageId;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (resultObject.ContainsKey("return") && resultObject.ContainsKey("status") && resultObject.ContainsKey("message"))
                            logger.LogWarning($"error! , status :{resultObject["status"]} , message :{resultObject["message"]} ");

                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }

            throw new AppException("خطا در ارسال لینک پرداخت به مشتری!");
        }

        public async Task<long?> SendVerificationSmsToTransferCreditAmount(Guid sellerId, long amount, [NotNull] string mobile, [NotNull] string shopName,
            string verficationCode, CancellationToken cancellationToken)
        {
            //return Convert.ToInt32(verficationCode) + new Random().Next(1, 100000);
            if (string.IsNullOrEmpty(mobile)) throw new AppException("'mobile' is null!");

            try
            {
                using (var client = new HttpClient())
                {
                    var seprateAmount = amount.ToString("N0");
                    //shopName = shopName.Trim().Replace(" ", "-");                

                    //string requesturl =
                    //    $"https://api.kavenegar.com/v1/{API_KEY}/verify/lookup.json?receptor={mobile}&token={seprateAmount}&token2={shopName}&token3={randomVerficationCode}&template=VerifyBetweenSellerAndBuyer";
                    var message = $"کاربر گرامی شما قصد دارید مبلغ {seprateAmount} ريال از اعتبار خود را در فروشگاه {shopName} استفاده کنید{Environment.NewLine}در صورت تایید کد {verficationCode} را به فروشنده اعلام کنید{Environment.NewLine}زرین لند";
                    string requesturl =
                        $"https://api.kavenegar.com/v1/{API_KEY}/sms/send.json?receptor={mobile}&message={WebUtility.UrlEncode(message)}";

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogInformation(responseBody);

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("return") &&
                        JObject.Parse(resultObject["return"].ToString()).ContainsKey("status") &&
                        JObject.Parse(resultObject["return"].ToString())["status"].ToString().Equals("200", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (resultObject.ContainsKey("entries") &&
                            resultObject["entries"].GetType() == typeof(JArray) &&
                            (resultObject["entries"] as JArray).Count > 0)
                        {
                            var result = JsonConvert.DeserializeObject<SmsVerificationModel>((resultObject["entries"] as JArray)[0].ToString());

                            #region Insert To DB
                            //await smsVerificatioRepository.AddAsync(new SmsVerification()
                            //{
                            //    ReceptorId = buyerId,
                            //    SenderId = sellerId,
                            //    Cost = result.Cost,
                            //    ExpireTime = DateTime.Now.AddMinutes(10),
                            //    Message = result.Message,
                            //    MessageId = result.MessageId,
                            //    MessageTemplate = "VerifyBetweenSellerAndBuyer",
                            //    Receptor = result.Receptor,
                            //    Sender = result.Sender,
                            //    Status = result.Status,
                            //    StatusText = result.StatusText,
                            //    Token = result.Token,
                            //    Token2 = result.Token2,
                            //    Token3 = result.Token3,
                            //    Amount = amount,
                            //    VerficationCode = randomVerficationCode,
                            //    ReturnCode = Convert.ToInt32(JObject.Parse(resultObject["return"].ToString())["status"]),
                            //    ReturnMessage = JObject.Parse(resultObject["return"].ToString())["message"].ToString()
                            //}, cancellationToken);
                            #endregion

                            return result.MessageId;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (resultObject.ContainsKey("return") && resultObject.ContainsKey("status") && resultObject.ContainsKey("message"))
                            logger.LogWarning($"error! , status :{resultObject["status"]} , message :{resultObject["message"]} ");
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }

            return null;
        }
    }
}