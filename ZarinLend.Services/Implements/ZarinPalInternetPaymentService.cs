using Common;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.Payment;
using Core.Entities.Business.RequestFacility;
using Core.Entities.Business.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Services.Model.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class ZarinPalInternetPaymentService : IZarinPalInternetPaymentService, IScopedDependency
    {
        private readonly ILogger<ZarinPalInternetPaymentService> logger;
        private readonly IBaseRepository<ZarinPalInternetPayment> zarinPalInternetPaymentRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletTransactionRepository walletTransactionRepository;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly IRequestFacilityRepository requestFacilityRepository;
        private readonly IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmetRepository;
        private readonly IBaseRepository<GlobalSetting> globalSettingRepository;
        private readonly IUserRepository userRepository;
        private readonly IBaseRepository<PaymentInfo> paymentInfoRepository;
        public static readonly string MerchantId = "cf01410e-a6a0-43d2-97b7-3c5dc81199f7";
        private readonly string ZarinPalRequestUrl = "https://api.zarinpal.com/pg/v4/payment/request.json?";
        public static readonly string ZarinPalVerifyUrl = "https://api.zarinpal.com/pg/v4/payment/verify.json?";
        public static readonly string GatewayUrl = "https://www.zarinpal.com/pg/StartPay/";
        private const int SuccessCode = 100;
        private const int VerifiedCode = 101;
        public ZarinPalInternetPaymentService(ILogger<ZarinPalInternetPaymentService> logger,
                                              IBaseRepository<ZarinPalInternetPayment> zarinPalInternetPaymentRepository,
                                              ITransactionRepository transactionRepository,
                                              IWalletTransactionRepository walletTransactionRepository,
                                              IRequestFacilityService requestFacilityService,
                                              IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
                                              IRequestFacilityRepository requestFacilityRepository,
                                              IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmetRepository,
                                              IBaseRepository<GlobalSetting> globalSettingRepository,
                                              IUserRepository userRepository,
                                              IBaseRepository<PaymentInfo> paymentInfoRepository)
        {
            this.logger = logger;
            this.zarinPalInternetPaymentRepository = zarinPalInternetPaymentRepository;
            this.transactionRepository = transactionRepository;
            this.walletTransactionRepository = walletTransactionRepository;
            this.requestFacilityService = requestFacilityService;
            //this.walletTransactionRepository = walletTransactionRepository;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.requestFacilityRepository = requestFacilityRepository;
            this.requestFacilityInstallmetRepository = requestFacilityInstallmetRepository;
            this.globalSettingRepository = globalSettingRepository;
            this.userRepository = userRepository;
            this.paymentInfoRepository = paymentInfoRepository;
        }

        /// <summary>
        /// initilize 'Zarinpal Internet payment' and get 'authority'
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestFacilityId"></param>
        /// <param name="amount"></param>
        /// <param name="paymentType"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="mobile"></param>
        /// <param name="description"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> InitilizeInternetPayment(Guid userId, int? requestFacilityId, long amount, PaymentType paymentType, string callbackUrl,
            string mobile, string description, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    #region Insert Into DB
                    var internetPayment = new ZarinPalInternetPayment()
                    {
                        UserId = userId,
                        Amount = amount,
                        Description = description,
                        PaymentType = paymentType,
                        Merchant_Id = MerchantId,
                        PaymentReason = new PaymentReason()
                        {
                            RequestFacilityId = requestFacilityId,
                        }
                    };
                    await zarinPalInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
                    callbackUrl += $"?ZarinPalInternetPaymentId={internetPayment.Id}";
                    #endregion

                    string Reqparameters = $"merchant_id={MerchantId}&amount={amount}&callback_url={callbackUrl}&description={description}&mobile={mobile}";

                    HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(ZarinPalRequestUrl + Reqparameters, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject responseObject = JObject.Parse(responseBody);
                    string errors = responseObject["errors"].ToString();
                    string data = responseObject["data"].ToString();

                    if (data != "[]")
                    {
                        var authority = responseObject["data"]["authority"].ToString();
                        internetPayment.Authority = authority;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Authority),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        return authority;
                    }
                    else
                    {
                        internetPayment.IsSuccess = false;
                        internetPayment.Errors = errors;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Errors),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        logger.LogError(errors);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// initilize 'Zarinpal Internet payment' and get 'authority'
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestFacilityGuarantorId"></param>
        /// <param name="requestFacilityId"></param>
        /// <param name="amount"></param>
        /// <param name="paymentType"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="mobile"></param>
        /// <param name="description"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> InitilizeInternetPaymentByGuarantor(Guid userId, int requestFacilityGuarantorId, int requestFacilityId, long amount, PaymentType paymentType, string callbackUrl,
            string mobile, string description, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    #region Insert Into DB
                    var internetPayment = new ZarinPalInternetPayment()
                    {
                        UserId = userId,
                        Amount = amount,
                        Description = description,
                        PaymentType = paymentType,
                        Merchant_Id = MerchantId,
                        IpgType = IpgType.ZarinpalIPG,
                        PaymentReason = new PaymentReason()
                        {
                            RequestFacilityId = requestFacilityId,
                            RequestFacilityGuarantorId = requestFacilityGuarantorId
                        }
                    };
                    await zarinPalInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
                    callbackUrl += $"?ZarinPalInternetPaymentId={internetPayment.Id}";
                    #endregion

                    string Reqparameters = $"merchant_id={MerchantId}&amount={amount}&callback_url={callbackUrl}&description={description}&mobile={mobile}";

                    HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(ZarinPalRequestUrl + Reqparameters, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject responseObject = JObject.Parse(responseBody);
                    string errors = responseObject["errors"].ToString();
                    string data = responseObject["data"].ToString();

                    if (data != "[]")
                    {
                        var authority = responseObject["data"]["authority"].ToString();
                        internetPayment.Authority = authority;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Authority),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        return authority;
                    }
                    else
                    {
                        internetPayment.IsSuccess = false;
                        internetPayment.Errors = errors;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Errors),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        logger.LogError(errors);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return null;
        }

        public async Task<string> InitilizeInternetPaymentBySeller(Guid userId, Guid paymentInfoId, long amount, string mobile, string description, string callbackUrl, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    #region Insert Into DB
                    var internetPayment = new ZarinPalInternetPayment()
                    {
                        UserId = userId,
                        Amount = amount,
                        Description = description,
                        PaymentType = PaymentType.PayForBuyByBuyer,
                        Merchant_Id = MerchantId,
                        IpgType = IpgType.ZarinpalIPG,
                        PaymentReason = new PaymentReason()
                        {
                            PaymentInfoId = paymentInfoId,
                        }
                    };
                    await zarinPalInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
                    callbackUrl += $"?ZarinPalInternetPaymentId={internetPayment.Id}";
                    #endregion

                    string Reqparameters = $"merchant_id={MerchantId}&amount={amount}&callback_url={callbackUrl}&description={description}&mobile={mobile}";

                    HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(ZarinPalRequestUrl + Reqparameters, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject responseObject = JObject.Parse(responseBody);
                    string errors = responseObject["errors"].ToString();
                    string data = responseObject["data"].ToString();

                    if (data != "[]")
                    {
                        var authority = responseObject["data"]["authority"].ToString();
                        internetPayment.Authority = authority;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Authority),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        return authority;
                    }
                    else
                    {
                        internetPayment.IsSuccess = false;
                        internetPayment.Errors = errors;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Errors),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        logger.LogError(errors);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// initilize 'Zarinpal Internet payment' and get 'authority'
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestFacilityId"></param>
        /// <param name="requestFacilityInstallmentId"></param>
        /// <param name="amount"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="mobile"></param>
        /// <param name="description"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> InitilizeInternetPayment(Guid userId, int requestFacilityId, int requestFacilityInstallmentId, long amount, string callbackUrl, string mobile,
            string description, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    #region Insert Into DB
                    var internetPayment = new ZarinPalInternetPayment()
                    {
                        UserId = userId,
                        Amount = amount,
                        Description = description,
                        PaymentType = PaymentType.PayInstallment,
                        Merchant_Id = MerchantId,
                        IpgType = IpgType.ZarinpalIPG,
                        PaymentReason = new PaymentReason()
                        {
                            RequestFacilityId = requestFacilityId,
                            RequestFacilityInstallmentId = requestFacilityInstallmentId
                        }
                    };
                    await zarinPalInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
                    callbackUrl += $"?ZarinPalInternetPaymentId={internetPayment.Id}";
                    #endregion

                    string Reqparameters = $"merchant_id={MerchantId}&amount={amount}&callback_url={callbackUrl}&description={description}&mobile={mobile}";

                    HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(ZarinPalRequestUrl + Reqparameters, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject responseObject = JObject.Parse(responseBody);
                    string errors = responseObject["errors"].ToString();
                    string data = responseObject["data"].ToString();

                    if (data != "[]")
                    {
                        var authority = responseObject["data"]["authority"].ToString();
                        internetPayment.Authority = authority;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Authority),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        return authority;
                    }
                    else
                    {
                        internetPayment.IsSuccess = false;
                        internetPayment.Errors = errors;
                        await zarinPalInternetPaymentRepository.UpdateCustomPropertiesAsync(internetPayment, cancellationToken, true,
                            nameof(ZarinPalInternetPayment.Errors),
                            nameof(ZarinPalInternetPayment.UpdateDate));

                        logger.LogError(errors);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return null;
        }

        public async Task<InternetPaymentResponseModel> ValidatePaymentResponse(HttpContext context, CancellationToken cancellationToken)
        {
            string status, authority;
            long zarinPalInternetPaymentId;
            if (context.Request.Query["Authority"].Count == 0 ||
                context.Request.Query["ZarinPalInternetPaymentId"].Count == 0 ||
                context.Request.Query["status"].Count == 0)
                return new InternetPaymentResponseModel()
                {
                    Message = "خطایی نامشخص رخ داده است",
                    IsSuccess = false
                };

            authority = context.Request.Query["Authority"]!;
            status = context.Request.Query["Status"]!;
            zarinPalInternetPaymentId = Convert.ToInt32(context.Request.Query["ZarinPalInternetPaymentId"]);

            var internetPayment = await zarinPalInternetPaymentRepository.TableNoTracking
                .Include(p => p.PaymentReason)
                .Where(p => p.Id == zarinPalInternetPaymentId && p.Authority == authority)
                .FirstOrDefaultAsync(cancellationToken);

            if (internetPayment != null)
            {
                if (internetPayment.IsSuccess.HasValue && internetPayment.IsSuccess.Value)
                {
                    return new InternetPaymentResponseModel()
                    {
                        Amount = internetPayment.Amount,
                        Authority = internetPayment.Authority,
                        Card_Pan = internetPayment.Card_Pan,
                        Message = internetPayment.Message,
                        Ref_Id = internetPayment.Ref_Id,
                        Code = internetPayment.Code,
                        RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                        RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                        IsSuccess = true
                    };
                }
                using (HttpClient client = new HttpClient())
                {
                    string Reqparameters = $"merchant_id={MerchantId}&amount={internetPayment.Amount}&authority={authority}";
                    HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(ZarinPalVerifyUrl + Reqparameters, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject responseObject = JObject.Parse(responseBody);
                    string dataString = responseObject["data"]!.ToString();
                    string errorsString = responseObject["errors"]!.ToString();

                    if (dataString != "[]" && dataString != "{}")
                    {
                        var code = Convert.ToInt32(responseObject["data"]!["code"]);
                        if (code == SuccessCode || code == VerifiedCode)
                        {
                            internetPayment.Ref_Id =  responseObject["data"]!["ref_id"]!.ToString();
                            internetPayment.Card_Hash = responseObject["data"]!["card_hash"]!.ToString();
                            internetPayment.Card_Pan = responseObject["data"]!["card_pan"]!.ToString();
                            internetPayment.Fee = Convert.ToInt32(responseObject["data"]!["fee"]);
                            internetPayment.Fee_Type = responseObject["data"]!["fee_type"]!.ToString();
                            internetPayment.Message = responseObject["data"]!["message"]!.ToString();
                            internetPayment.IsSuccess = true;
                            internetPayment.Code = code;
                            await zarinPalInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken, false);

                            if (internetPayment.PaymentType == PaymentType.PayInstallment && internetPayment.PaymentReason.RequestFacilityInstallmentId.HasValue)
                            {
                                var installment = await requestFacilityInstallmetRepository
                                            .TableNoTracking
                                            .Where(p => p.Id == internetPayment.PaymentReason.RequestFacilityInstallmentId.Value)
                                            .Select(p => new
                                            {
                                                p.RequestFacility.GlobalSetting.FacilityInterest,
                                                p.StartForPayment,
                                                p.DueDate,
                                                p.Amount
                                            })
                                            .FirstOrDefaultAsync(cancellationToken);
                                await requestFacilityInstallmetRepository.UpdateCustomPropertiesAsync(new RequestFacilityInstallment()
                                {
                                    Id = internetPayment.PaymentReason.RequestFacilityInstallmentId.Value,
                                    Paid = true,
                                    RealPayDate = DateTime.Now,
                                    PenaltyDays = installment.StartForPayment.Value.Date > installment.DueDate
                                        ? (int)Math.Ceiling((installment.StartForPayment.Value - installment.DueDate).TotalDays)
                                        : 0,
                                    PenaltyAmount = installment.StartForPayment.Value.Date > installment.DueDate
                                        ? (installment.Amount * ((int)installment.FacilityInterest + 6) * (int)Math.Ceiling((installment.StartForPayment.Value - installment.DueDate).TotalDays)) / 36500
                                        : 0,
                                    RealPayAmount = installment.StartForPayment.Value.Date > installment.DueDate
                                        ? installment.Amount + (installment.Amount * ((int)installment.FacilityInterest + 6) * (int)Math.Ceiling((installment.StartForPayment.Value - installment.DueDate).TotalDays)) / 36500
                                        : installment.Amount,
                                }, cancellationToken, false,
                                nameof(RequestFacilityInstallment.Paid),
                                nameof(RequestFacilityInstallment.RealPayDate),
                                nameof(RequestFacilityInstallment.PenaltyAmount),
                                nameof(RequestFacilityInstallment.PenaltyDays),
                                nameof(RequestFacilityInstallment.RealPayAmount));
                            }

                            if (internetPayment.PaymentType != PaymentType.PayForBuyByBuyer)
                            {
                                await transactionRepository.AddAsync(new Transaction()
                                {
                                    Amount = internetPayment.Amount,
                                    CreatorId = internetPayment.UserId,
                                    UserId = internetPayment.UserId,
                                    Description = internetPayment.Description,
                                    TransactionType = TransactionTypeEnum.Deposit,
                                    TransactionReason = new TransactionReason()
                                    {
                                        RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                                        RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                                        PaymentId = internetPayment.Id,
                                    }
                                }, cancellationToken);
                            }
                            else
                            {
                                var activeGlobalSetting = await globalSettingRepository.GetColumnValueAsync(p => p.IsActive,
                                                                                                                     p => new { p.Id, FeePercentage = p.FinancialInstitutionFacilityFee + p.LendTechFacilityFee },
                                                                                                                     cancellationToken);
                                var shopAmount = internetPayment.Amount - Convert.ToInt64(activeGlobalSetting.FeePercentage * internetPayment.Amount / 100);
                                var system_account_user_Id = userRepository.TableNoTracking.Where(p => p.UserName == "system_admin").Select(p => p.Id).FirstOrDefault();
                                var paymentInfo = await paymentInfoRepository.TableNoTracking
                                    .Where(p => p.Id == internetPayment.PaymentReason.PaymentInfoId.Value)
                                    .Select(p => new
                                    {
                                        p.OrganizationId,
                                        p.SellerId,
                                        ShopName = p.Seller.Person.Organization.Name,
                                        p.BuyerId,
                                        p.Buyer.Person.NationalCode,
                                        p.Buyer.Person.FName,
                                        p.Buyer.Person.LName
                                    })
                                    .FirstOrDefaultAsync(cancellationToken);

                                await paymentInfoRepository.UpdateCustomPropertiesAsync(new PaymentInfo()
                                {
                                    Id = internetPayment.PaymentReason.PaymentInfoId.Value,
                                    IsActive = false,
                                    IsUsed = true
                                }, cancellationToken, false,
                                nameof(PaymentInfo.IsActive),
                                nameof(PaymentInfo.IsUsed));

                                await walletTransactionRepository.AddRangeAsync(new List<WalletTransaction>()
                                {
                                    new WalletTransaction(){
                                        WalletTransactionType = WalletTransactionTypeEnum.Withdraw,
                                        Amount=-internetPayment.Amount,
                                        CreatorId=system_account_user_Id,
                                        UserId = paymentInfo.BuyerId,
                                        Description =$"برداشت بابت خرید از فروشگاه '{paymentInfo.ShopName}'"
                                    },
                                    new WalletTransaction(){
                                        WalletTransactionType = WalletTransactionTypeEnum.Deposit,
                                        Amount=shopAmount,
                                        CreatorId=system_account_user_Id,
                                        UserId = paymentInfo.SellerId,
                                        OrganizationId = paymentInfo.OrganizationId,
                                        Description =$"واریز بابت خرید از مشتری '{paymentInfo.FName} {paymentInfo.LName}({paymentInfo.NationalCode})'",
                                        Invoices = new List<Invoice>()
                                        {
                                            new Invoice()
                                            {
                                                GlobalSettingId = activeGlobalSetting.Id,
                                                Status = InvoiceStatus.Register,
                                                OrganizationId =paymentInfo.OrganizationId,
                                                SellerId = paymentInfo.SellerId,
                                                Amount = shopAmount,
                                                CreatorId=system_account_user_Id,
                                                BuyerId=paymentInfo.BuyerId,
                                                Number = string.Empty,
                                                Description =$"ایجاد اتوماتیک فاکتور - مشتری : {paymentInfo.FName} {paymentInfo.LName}({paymentInfo.NationalCode})"
                                            }
                                        }
                                    },
                                }, cancellationToken, saveNow: true);
                            }
                            return new InternetPaymentResponseModel()
                            {
                                Amount = internetPayment.Amount,
                                Authority = internetPayment.Authority,
                                Card_Pan = internetPayment.Card_Pan,
                                Message = internetPayment.Message,
                                Ref_Id = internetPayment.Ref_Id,
                                Code = internetPayment.Code,
                                RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                                RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                                IsSuccess = true
                            };
                        }
                    }
                    else if (errorsString != "[]" && errorsString !="{}")
                    {
                        internetPayment.IsSuccess = false;
                        internetPayment.Code = Convert.ToInt32(responseObject["errors"]["code"]);
                        internetPayment.Errors = responseObject["errors"]["message"].ToString();
                        await zarinPalInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken);
                        return new InternetPaymentResponseModel()
                        {
                            Message = $"کد خطا : {responseObject["errors"]["code"]} - پیغام خطا : {responseObject["errors"]["message"]}",
                            Amount = internetPayment.Amount,
                            RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                            RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                            Authority = authority,
                            IsSuccess = false
                        };
                    }
                }
            }

            return new InternetPaymentResponseModel()
            {
                Message = "خطایی نامشخص رخ داده است",
                Amount = internetPayment.Amount,
                IsSuccess = false
            };
        }

        public async Task ValidationStatuslessPayments(Guid userId, CancellationToken cancellationToken)
        {
            var statuslessPayments = await zarinPalInternetPaymentRepository
                .TableNoTracking
                .Include(p => p.PaymentReason)
                .Where(p => p.UserId.Equals(userId) && p.IsSuccess == null && p.Authority != null)
                .ToListAsync(cancellationToken);

            foreach (var internetPayment in statuslessPayments)
            {
                using (HttpClient client = new HttpClient())
                {
                    string Reqparameters = $"merchant_id={MerchantId}&amount={internetPayment.Amount}&authority={internetPayment.Authority}";
                    HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(ZarinPalVerifyUrl + Reqparameters, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject responseObject = JObject.Parse(responseBody);
                    string dataString = responseObject["data"]!.ToString();
                    string errorsString = responseObject["errors"]!.ToString();

                    if (dataString != "[]" && dataString != "{}")
                    {
                        var code = Convert.ToInt32(responseObject["data"]!["code"]);
                        if (code == SuccessCode || code == VerifiedCode)
                        {
                            internetPayment.Ref_Id =  responseObject["data"]!["ref_id"]!.ToString();
                            internetPayment.Card_Hash = responseObject["data"]!["card_hash"]!.ToString();
                            internetPayment.Card_Pan = responseObject["data"]!["card_pan"]!.ToString();
                            internetPayment.Fee = Convert.ToInt32(responseObject["data"]!["fee"]);
                            internetPayment.Fee_Type = responseObject["data"]!["fee_type"]!.ToString();
                            internetPayment.Message = responseObject["data"]!["message"]!.ToString();
                            internetPayment.IsSuccess = true;
                            internetPayment.Code = code;
                            await zarinPalInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken, false);

                            if (internetPayment.PaymentType == PaymentType.PayInstallment && internetPayment.PaymentReason.RequestFacilityInstallmentId.HasValue)
                            {
                                var installment = await requestFacilityInstallmetRepository
                                            .TableNoTracking
                                            .Where(p => p.Id == internetPayment.PaymentReason.RequestFacilityInstallmentId.Value)
                                            .Select(p => new
                                            {
                                                p.RequestFacility.GlobalSetting.FacilityInterest,
                                                p.StartForPayment,
                                                p.DueDate,
                                                p.Amount
                                            })
                                            .FirstOrDefaultAsync(cancellationToken);
                                await requestFacilityInstallmetRepository.UpdateCustomPropertiesAsync(new RequestFacilityInstallment()
                                {
                                    Id = internetPayment.PaymentReason.RequestFacilityInstallmentId.Value,
                                    Paid = true,
                                    RealPayDate = DateTime.Now,
                                    PenaltyDays = installment!.StartForPayment!.Value.Date > installment.DueDate
                                        ? (int)Math.Ceiling((installment.StartForPayment.Value - installment.DueDate).TotalDays)
                                        : 0,
                                    PenaltyAmount = installment.StartForPayment.Value.Date > installment.DueDate
                                        ? (installment.Amount * ((int)installment.FacilityInterest + 6) * (int)Math.Ceiling((installment.StartForPayment.Value - installment.DueDate).TotalDays)) / 36500
                                        : 0,
                                    RealPayAmount = installment.StartForPayment.Value.Date > installment.DueDate
                                        ? installment.Amount + (installment.Amount * ((int)installment.FacilityInterest + 6) * (int)Math.Ceiling((installment.StartForPayment.Value - installment.DueDate).TotalDays)) / 36500
                                        : installment.Amount,
                                }, cancellationToken, false,
                                nameof(RequestFacilityInstallment.Paid),
                                nameof(RequestFacilityInstallment.RealPayDate),
                                nameof(RequestFacilityInstallment.PenaltyAmount),
                                nameof(RequestFacilityInstallment.PenaltyDays),
                                nameof(RequestFacilityInstallment.RealPayAmount));
                            }

                            if (internetPayment.PaymentType != PaymentType.PayForBuyByBuyer)
                            {
                                await transactionRepository.AddAsync(new Transaction()
                                {
                                    Amount = internetPayment.Amount,
                                    CreatorId = internetPayment.UserId,
                                    UserId = internetPayment.UserId,
                                    Description = internetPayment.Description,
                                    TransactionType = TransactionTypeEnum.Deposit,
                                    TransactionReason = new TransactionReason()
                                    {
                                        RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                                        RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                                        PaymentId = internetPayment.Id
                                    }
                                }, cancellationToken);
                            }
                            else
                            {
                                var activeGlobalSetting = await globalSettingRepository.GetColumnValueAsync(p => p.IsActive,
                                                                                                                     p => new { p.Id, FeePercentage = p.FinancialInstitutionFacilityFee + p.LendTechFacilityFee },
                                                                                                                     cancellationToken);
                                var shopAmount = internetPayment.Amount - Convert.ToInt64(activeGlobalSetting.FeePercentage * internetPayment.Amount / 100);
                                var system_account_user_Id = userRepository.TableNoTracking.Where(p => p.UserName == "system_admin").Select(p => p.Id).FirstOrDefault();
                                var paymentInfo = await paymentInfoRepository.TableNoTracking
                                    .Where(p => p.Id == internetPayment.PaymentReason.PaymentInfoId.Value)
                                    .Select(p => new
                                    {
                                        p.OrganizationId,
                                        p.SellerId,
                                        ShopName = p.Seller.Person.Organization.Name,
                                        p.BuyerId,
                                        p.Buyer.Person.NationalCode,
                                        p.Buyer.Person.FName,
                                        p.Buyer.Person.LName
                                    })
                                    .FirstOrDefaultAsync(cancellationToken);

                                await paymentInfoRepository.UpdateCustomPropertiesAsync(new PaymentInfo()
                                {
                                    Id = internetPayment.PaymentReason.PaymentInfoId.Value,
                                    IsActive = false,
                                    IsUsed = true
                                }, cancellationToken, false,
                                nameof(PaymentInfo.IsActive),
                                nameof(PaymentInfo.IsUsed));

                                await walletTransactionRepository.AddRangeAsync(new List<WalletTransaction>()
                                {
                                    new WalletTransaction(){
                                        WalletTransactionType = WalletTransactionTypeEnum.Withdraw,
                                        Amount=-internetPayment.Amount,
                                        CreatorId=system_account_user_Id,
                                        UserId = paymentInfo.BuyerId,
                                        Description =$"برداشت بابت خرید از فروشگاه '{paymentInfo.ShopName}'"
                                    },
                                    new WalletTransaction(){
                                        WalletTransactionType = WalletTransactionTypeEnum.Deposit,
                                        Amount=shopAmount,
                                        CreatorId=system_account_user_Id,
                                        UserId = paymentInfo.SellerId,
                                        OrganizationId = paymentInfo.OrganizationId,
                                        Description =$"واریز بابت خرید از مشتری '{paymentInfo.FName} {paymentInfo.LName}({paymentInfo.NationalCode})'",
                                        Invoices = new List<Invoice>()
                                        {
                                            new Invoice()
                                            {
                                                GlobalSettingId = activeGlobalSetting.Id,
                                                Status = InvoiceStatus.Register,
                                                OrganizationId =paymentInfo.OrganizationId,
                                                SellerId = paymentInfo.SellerId,
                                                Amount = shopAmount,
                                                CreatorId=system_account_user_Id,
                                                BuyerId=paymentInfo.BuyerId,
                                                Number = string.Empty,
                                                Description =$"ایجاد اتوماتیک فاکتور - مشتری : {paymentInfo.FName} {paymentInfo.LName}({paymentInfo.NationalCode})"
                                            }
                                        }
                                    },
                                }, cancellationToken, saveNow: true);
                            }
                        }
                    }
                    else if (errorsString != "[]" && errorsString != "{}")
                    {
                        internetPayment.IsSuccess = false;
                        internetPayment.Code = Convert.ToInt32(responseObject["errors"]["code"]);
                        internetPayment.Errors = responseObject["errors"]["message"].ToString();
                        await zarinPalInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken);
                    }
                }
            }
        }

        public async Task ApprovedPaymentVerifyShahkarAndSamatServiceStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            if (await requestFacilityService
               .CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService,
               cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService,
                    StatusEnum.Approved, userId, userId, null, cancellationToken: cancellationToken);
            }
        }

        public async Task ApprovedVerifyShahkarAndSamatServiceStep(Guid userId, Guid creatorId, int requestFacilityId, CancellationToken cancellationToken)
        {
            if (await requestFacilityService
               .CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.Buyer }, WorkFlowFormEnum.VerifyShahkarAndSamatService,
               cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.VerifyShahkarAndSamatService,
                    StatusEnum.Approved, userId, creatorId, null, cancellationToken: cancellationToken);
            }
        }

        public async Task ApprovedPaymentSalesCommissioStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            if (await requestFacilityService
               .CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.PaySalesCommission,
               cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.PaySalesCommission,
                    StatusEnum.Approved, userId, userId, "تایید-پرداخت کارمزد عملیات", cancellationToken: cancellationToken);

                #region Add amount Request Facility to Wallet
                var amount = await requestFacilityRepository.GetColumnValueAsync<long>(p => p.Id.Equals(requestFacilityId), cancellationToken, nameof(RequestFacility.Amount));
                await transactionRepository.AddAsync(new Transaction()
                {
                    Amount = amount,
                    UserId = userId,
                    CreatorId = userId,                   
                    TransactionType = TransactionTypeEnum.Deposit,
                    Description = "پرداخت کارمزد عملیات",
                    TransactionReason = new TransactionReason()
                    {
                        RequestFacilityId = requestFacilityId
                    }
                }, cancellationToken);
                #endregion
            }
        }

        //public async Task<bool> ExistSuccessfulPayment(int requestFacilityId, PaymentType paymentType, CancellationToken cancellationToken)
        //{
        //    return await zarinPalInternetPaymentRepository
        //                .TableNoTracking
        //                .AnyAsync(p => p.RequestFacilityId == requestFacilityId &&
        //                         p.PaymentType == paymentType &&
        //                         p.IsSuccess == true &&
        //                         p.Code == SuccessCode,
        //                         cancellationToken);
        //}

        //public async Task<bool> ExistSuccessfulPayment(Guid userId, PaymentType paymentType, CancellationToken cancellationToken)
        //{
        //    return await zarinPalInternetPaymentRepository
        //                .TableNoTracking
        //                .AnyAsync(p => p.UserId == userId &&
        //                         p.PaymentType == paymentType &&
        //                         p.IsSuccess == true &&
        //                         p.Code == SuccessCode,
        //                         cancellationToken);
        //}
    }
}
