using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.Payment;
using Core.Entities.Business.RequestFacility;
using Core.Entities.Business.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class SamanInternetPaymentService : ISamanInternetPaymentService, IScopedDependency
    {
        private const string TERMINAL_ID = "13614110";
        private const string TERMINAL_PASS = "1460234";
        private const int SuccessStatus = 2;
        private const int SuccessVerifyCode = 0;
        private const string SuccessState = "OK";
        public const string IPG_URL = "https://sep.shaparak.ir/onlinepg/onlinepg";
        public const string GET_TOKEN_URL = "https://sep.shaparak.ir/onlinepg/sendtoken";
        private const string VERIFY_TRANSACTION_URL = "https://sep.shaparak.ir/verifyTxnRandomSessionkey/ipg/VerifyTransaction";
        private const string REVERSE_TRANSACTION_URL = "https://sep.shaparak.ir/verifyTxnRandomSessionkey/ipg/ReverseTransaction";
        private readonly ILogger<SamanInternetPaymentService> logger;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IBaseRepository<SamanInternetPayment> samanInternetPaymentRepository;
        private readonly IRequestFacilityRepository requestFacilityRepository;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletTransactionRepository walletTransactionRepository;
        private readonly IBaseRepository<PaymentInfo> paymentInfoRepository;
        private readonly IUserRepository userRepository;
        private readonly IBaseRepository<GlobalSetting> globalSettingRepository;
        private readonly IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmetRepository;

        public SamanInternetPaymentService(ILogger<SamanInternetPaymentService> logger,
                                           IRequestFacilityService requestFacilityService,
                                           IBaseRepository<SamanInternetPayment> samanInternetPaymentRepository,
                                           IRequestFacilityRepository requestFacilityRepository,
                                           IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
                                           ITransactionRepository transactionRepository,
                                           IWalletTransactionRepository walletTransactionRepository,
                                           IBaseRepository<PaymentInfo> paymentInfoRepository,
                                           IUserRepository userRepository,
                                           IBaseRepository<GlobalSetting> globalSettingRepository,
                                           IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmetRepository)
        {
            this.logger = logger;
            this.requestFacilityService = requestFacilityService;
            this.samanInternetPaymentRepository = samanInternetPaymentRepository;
            this.requestFacilityRepository = requestFacilityRepository;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.transactionRepository = transactionRepository;
            this.walletTransactionRepository = walletTransactionRepository;
            this.paymentInfoRepository = paymentInfoRepository;
            this.userRepository = userRepository;
            this.globalSettingRepository = globalSettingRepository;
            this.requestFacilityInstallmetRepository = requestFacilityInstallmetRepository;
        }

        public async Task<long> GetTotalWithdrawBaseCard(string cardNumber,Guid userId,CancellationToken cancellationToken)
        {
            return await samanInternetPaymentRepository.TableNoTracking
                                                       .Where(p => p.IsSuccess == true && p.CardNumberForPayment == cardNumber && p.UserId == userId)
                                                       .SumAsync(p => p.Amount, cancellationToken);
        }
        public async Task<PagingDto<SamanIntenetBankPaymentExportModel>> SearchForExport(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var entities = samanInternetPaymentRepository.TableNoTracking;

            ApplyFilter(ref entities, filter);

            var query = entities
                         .OrderByDescending(p => p.UpdateDate)
                         .ThenByDescending(p => p.CreatedDate);

            var filterList = await query
                .Select(p => new SamanIntenetBankPaymentExportModel
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentType = p.PaymentType,
                    Payer = $"{p.User.Person.FName} {p.User.Person.LName}",
                    PayerNationalCode = p.User.Person.NationalCode,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    IsSuccess = p.IsSuccess,
                    MaskedPan = p.TransactionDetail_MaskedPan,
                    RefNum = p.RefNum,
                    //RequestFacilityGuarantorId = p.RequestFacilityGuarantorId,
                    //RequestFacilityId = p.RequestFacilityId,
                    //RequestFacilityInstallmentId = p.RequestFacilityInstallmentId,
                    ResNum = p.ResNum,
                    RRN = p.RRN,
                    State = p.State,
                    Status = p.Status,
                    TraceNo = p.TraceNo,
                    StraceDate = p.TransactionDetail_StraceDate,
                    //UserId = p.UserId,
                    //ValidationFee = p.RequestFacility.GlobalSetting.ValidationFee,
                    FinancialInstitutionFacilityFee = p.PaymentReason.RequestFacility.GlobalSetting.FinancialInstitutionFacilityFee,
                    LendTechFacilityFee = p.PaymentReason.RequestFacility.GlobalSetting.LendTechFacilityFee
                })
                .ToListAsync(cancellationToken);

            return new PagingDto<SamanIntenetBankPaymentExportModel>()
            {
                Data = filterList
            };
        }
        public async Task<PagingDto<SamanIntenetBankPaymentModel>> Search(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var entities = samanInternetPaymentRepository.TableNoTracking;

            ApplyFilter(ref entities, filter);

            var query = entities
                        .OrderByDescending(p => p.UpdateDate)
                        .ThenByDescending(p => p.CreatedDate)
                        .Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize);

            var filterList = await query
                .Select(p => new SamanIntenetBankPaymentModel
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentType = p.PaymentType,
                    Payer = $"{p.User.Person.FName} {p.User.Person.LName}",
                    PayerNationalCode = p.User.Person.NationalCode,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    StraceDate = p.TransactionDetail_StraceDate,
                    IsSuccess = p.IsSuccess,
                    MaskedPan = p.TransactionDetail_MaskedPan,
                    RequestFacilityGuarantorId = p.PaymentReason.RequestFacilityGuarantorId,
                    RequestFacilityId = p.PaymentReason.RequestFacilityId,
                    RequestFacilityInstallmentId = p.PaymentReason.RequestFacilityInstallmentId,
                    ResNum = p.ResNum,
                    UserId = p.UserId,
                    ValidationFee = p.PaymentReason.RequestFacility != null ? p.PaymentReason.RequestFacility.GlobalSetting.ValidationFee : 0,
                    FinancialInstitutionFacilityFee = p.PaymentReason.RequestFacility != null ? p.PaymentReason.RequestFacility.GlobalSetting.FinancialInstitutionFacilityFee : 0,
                    LendTechFacilityFee = p.PaymentReason.RequestFacility != null ? p.PaymentReason.RequestFacility.GlobalSetting.LendTechFacilityFee : 0
                })
                .ToListAsync(cancellationToken);

            var totalRowCounts = await query.CountAsync();

            return new PagingDto<SamanIntenetBankPaymentModel>()
            {
                CurrentPage = filter.Page,
                Data = filterList,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };
        }

        private IQueryable<SamanInternetPayment> ApplyFilter(ref IQueryable<SamanInternetPayment> list, PagingFilterDto filter)
        {
            if (filter != null && filter.FilterList != null)
            {
                foreach (var item in filter.FilterList)
                {
                    switch (item.PropertyName)
                    {
                        case "PaymentType":
                            {
                                PaymentType propertyValue = (PaymentType)item.PropertyValue;
                                if (propertyValue == PaymentType.PayValidationFeeByGuarantor || propertyValue == PaymentType.PayValidationFee)
                                    list = list.Where(p => (p.PaymentType == PaymentType.PayValidationFee || p.PaymentType == PaymentType.PayValidationFeeByGuarantor));
                                else
                                    list = list.Where(p => p.PaymentType == propertyValue);
                                break;
                            }
                        case "ResNum":
                            {
                                string resNum = item.PropertyValue;
                                list = list.Where(p => p.ResNum == resNum);
                                break;
                            }
                        case "FName":
                            {
                                string propertyValue = item.PropertyValue;
                                propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                                if (!string.IsNullOrEmpty(propertyValue))
                                    list = list.Where(p => p.User.Person.FName.Replace(" ", string.Empty).Contains(propertyValue));
                                break;
                            }
                        case "LName":
                            {
                                string propertyValue = item.PropertyValue;
                                propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                                if (!string.IsNullOrEmpty(propertyValue))
                                    list = list.Where(p => p.User.Person.LName.Replace(" ", string.Empty).Contains(propertyValue));
                                break;
                            }
                        case "NationalCode":
                            {
                                string propertyValue = item.PropertyValue;
                                list = list.Where(p => p.User.Person.NationalCode.Contains(propertyValue));
                                break;
                            }
                        case "StartDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                list = list.Where(p => (p.CreatedDate.Date >= propertyValue.Date || (p.UpdateDate.HasValue && p.UpdateDate.Value.Date >= propertyValue.Date)));
                                break;
                            }
                        case "EndDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                list = list.Where(p => (p.CreatedDate.Date <= propertyValue.Date || (p.UpdateDate.HasValue && p.UpdateDate.Value.Date <= propertyValue.Date)));
                                break;
                            }
                        case "IsSuccess":
                            {
                                bool? propertyValue = item.PropertyValue;
                                list = list.Where(p => p.IsSuccess == propertyValue);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            return list;
        }
        public async Task<string> InitilizeInternetPayment(Guid userId, int? requestFacilityId, long amount, PaymentType paymentType, string description,
            CancellationToken cancellationToken)
        {
            var internetPayment = new SamanInternetPayment()
            {
                UserId = userId,
                Amount = amount,
                Description = description,
                PaymentType = paymentType,
                TerminalId = TERMINAL_ID,
                IpgType = IpgType.SamanIPG,
                //ResNum = DateTimeHelper.GregorianToShamsi(DateTime.Now, _separator: string.Empty, showTime: true).Replace("-", string.Empty).Replace(":", string.Empty),
                ResNum = $"{DateTimeHelper.GregorianToShamsi(DateTime.Now, _separator: string.Empty, showTime: true).Replace("-", string.Empty).Replace(":", string.Empty)}{new Random().NextInt64(1000000000000, 9999999999999)}",
                PaymentReason = new PaymentReason()
                {
                    RequestFacilityId = requestFacilityId,
                }
            };
            await samanInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
            return internetPayment.ResNum.ToString();
        }

        public async Task<string> InitilizeInternetPayment(Guid userId, int requestFacilityId, int requestFacilityInstallmentId, long amount, string description, CancellationToken cancellationToken)
        {
            var internetPayment = new SamanInternetPayment()
            {
                UserId = userId,
                Amount = amount,
                Description = description,
                PaymentType = PaymentType.PayInstallment,
                TerminalId = TERMINAL_ID,
                IpgType = IpgType.SamanIPG,
                //ResNum = DateTimeHelper.GregorianToShamsi(DateTime.Now, _separator: string.Empty, showTime: true).Replace("-", string.Empty).Replace(":", string.Empty),
                ResNum = $"{DateTimeHelper.GregorianToShamsi(DateTime.Now, _separator: string.Empty, showTime: true).Replace("-", string.Empty).Replace(":", string.Empty)}{new Random().NextInt64(1000000000000, 9999999999999)}",
                PaymentReason = new PaymentReason()
                {
                    RequestFacilityId = requestFacilityId,
                    RequestFacilityInstallmentId = requestFacilityInstallmentId
                }

            };
            await samanInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
            return internetPayment.ResNum.ToString();
        }
        public async Task<string> InitilizeInternetPaymentByGuarantor(Guid userId, int requestFacilityGuarantorId, int requestFacilityId, long amount, PaymentType paymentType, string description,
            CancellationToken cancellationToken)
        {
            var internetPayment = new SamanInternetPayment()
            {
                UserId = userId,
                Amount = amount,
                Description = description,
                PaymentType = paymentType,
                TerminalId = TERMINAL_ID,
                IpgType = IpgType.SamanIPG,
                //ResNum = DateTimeHelper.GregorianToShamsi(DateTime.Now, _separator: string.Empty, showTime: true).Replace("-", string.Empty).Replace(":", string.Empty),
                ResNum = $"{DateTimeHelper.GregorianToShamsi(DateTime.Now, _separator: string.Empty, showTime: true).Replace("-", string.Empty).Replace(":", string.Empty)}{new Random().NextInt64(1000000000000, 9999999999999)}",
                PaymentReason = new PaymentReason()
                {
                    RequestFacilityId = requestFacilityId,
                    RequestFacilityGuarantorId = requestFacilityGuarantorId
                }
            };
            await samanInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
            return internetPayment.ResNum.ToString();
        }

        public async Task<string> InitilizeInternetPaymentBySeller(Guid userId, Guid paymentInfoId, long amount, string cardNumber, string description, CancellationToken cancellationToken)
        {
            var internetPayment = new SamanInternetPayment()
            {
                UserId = userId,
                Amount = amount,
                Description = description,
                PaymentType = PaymentType.PayForBuyByBuyer,
                TerminalId = TERMINAL_ID,
                CardNumberForPayment = cardNumber,
                IpgType = IpgType.SamanIPG,
                HashCardNumberForPayment = cardNumber.GetHash(HashMethod.SHA256),
                ResNum = $"{DateTimeHelper.GregorianToShamsi(DateTime.Now, _separator: string.Empty, showTime: true).Replace("-", string.Empty).Replace(":", string.Empty)}{new Random().NextInt64(1000000000000, 9999999999999)}",
                PaymentReason = new PaymentReason()
                {
                    PaymentInfoId = paymentInfoId
                }
            };
            await samanInternetPaymentRepository.AddAsync(internetPayment, cancellationToken);
            return internetPayment.ResNum.ToString();
        }

        public async Task<GetTokenResultModel> GetToken(string resNum, long amount, string mobile, string redirectUrl, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = JsonConvert.SerializeObject(
                        new
                        {
                            action = "token",
                            TerminalId = TERMINAL_ID,
                            Amount = amount,
                            resNum = resNum,
                            ResNum1 = "12345",
                            ResNum2 = "23456",
                            ResNum3 = "34567",
                            ResNum4 = "45678",
                            RedirectUrl = redirectUrl,
                            CellNumber = mobile,
                            TokenExpiryInMin = 360
                        });
                    var body = new StringContent(content, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(IPG_URL, body, cancellationToken);
                    if (response != null)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        if (responseBody == null) return null;

                        JObject resultObject = JObject.Parse(responseBody);
                        if (resultObject == null) return null;

                        return JsonConvert.DeserializeObject<GetTokenResultModel>(resultObject.ToString());
                    }

                    return null;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return null;
            }
        }
        public async Task<GetTokenResultModel> GetToken(string resNum, long amount, string mobile, string redirectUrl, string defaultCardNumber, CancellationToken cancellationToken)
        {
            try
            {
                var defaultPanKey = await RegisterCard(defaultCardNumber, null, cancellationToken);
                if (defaultPanKey == null) throw new AppException("'defaultPanKey' is null. return result from 'RegisterCard()' is null!");
                using (var client = new HttpClient())
                {
                    var content = JsonConvert.SerializeObject(
                        new
                        {
                            action = "token",
                            TerminalId = TERMINAL_ID,
                            Amount = amount,
                            resNum = resNum,
                            ResNum1 = "12345",
                            ResNum2 = "23456",
                            ResNum3 = "34567",
                            ResNum4 = "45678",
                            RedirectUrl = redirectUrl,
                            CellNumber = mobile,
                            TokenExpiryInMin = 360,
                            DefaultPanKey = defaultPanKey
                        });
                    var body = new StringContent(content, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(IPG_URL, body, cancellationToken);
                    if (response != null)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        if (responseBody == null) return null;

                        JObject resultObject = JObject.Parse(responseBody);
                        if (resultObject == null) return null;

                        return JsonConvert.DeserializeObject<GetTokenResultModel>(resultObject.ToString());
                    }

                    return null;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return null;
            }
        }

        public async Task<string> RegisterCard(string cardNumber, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetIdnToken(cancellationToken);
            if (token == null) throw new AppException("return result from 'GetIdnToken()' is null!");
            var responseBody = string.Empty;
            //long logId = 0;
            //var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://cardmanagementapi.sep.ir/api/v1/register-card";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var content = JsonConvert.SerializeObject(
                        new
                        {
                            Pan = cardNumber,
                            terminalNumber = TERMINAL_ID
                        });
                    var body = new StringContent(content, Encoding.UTF8, "application/json");

                    //logId = await finotechLogRepository
                    //    .AddLog(new FinotechLog()
                    //    {
                    //        RequestFacilityId = requestFacilityId,
                    //        Url = requesturl,
                    //        ServiceName = FinotechServiceName.ChargeCard.ToString(),
                    //        TrackId = trackId,
                    //        Body = content,
                    //        MethodType = MethodType.Post,
                    //        OpratorId = creator
                    //    });
                    HttpResponseMessage response = await client.PostAsync(requesturl, body, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        //if (logId > 0)
                        //{
                        //    await finotechLogRepository.UpdateLog(logId, responseBody);
                        //    logUpdateSuccessed = true;
                        //}

                        JObject resultObject = JObject.Parse(responseBody);
                        if (resultObject != null &&
                            resultObject.ContainsKey("data") &&
                            ((JObject)resultObject["data"]).ContainsKey("panUniqueKey") &&
                            resultObject["data"]["panUniqueKey"] != null)
                        {
                            logger.LogWarning($"RegisterCard : CardNumber :{cardNumber} , panUniqueKey : {resultObject["data"]["panUniqueKey"]}!");
                            return resultObject["data"]["panUniqueKey"].ToString();
                        }
                        else
                        {
                            logger.LogWarning($"RegisterCard : cardNumber :{cardNumber}. unexpected error!");
                        }
                    }
                    return null;
                }
            }
            catch (Exception exp)
            {
                //if (logId > 0 && !logUpdateSuccessed)
                //    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return null;
            }
        }

        /// <summary>
        /// get token for using reguster card(register-card)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> GetIdnToken(CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent("grant_type=password&username=HiradZarin&password=@HiradZarin123456&scope=Sep.OnlinePg.CardManagement", null, "application/x-www-form-urlencoded");
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic c2VwX3Jlc3RfYXBpX2NsaWVudGlkOnNlY3JldA==");

                    HttpResponseMessage response = await client.PostAsync("https://idn.seppay.ir/connect/token", content, cancellationToken);
                    if (response != null)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        if (responseBody == null) return null;

                        JObject resultObject = JObject.Parse(responseBody);
                        if (resultObject != null && resultObject.ContainsKey("access_token") && resultObject["access_token"] != null) return resultObject["access_token"].ToString();
                    }

                    return null;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return null;
            }
        }

        private async Task<VerifyTransactionModel> VerifyTransaction(string terminalId, string refNumber, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(
                    new
                    {
                        RefNum = refNumber,
                        TerminalNumber = terminalId,
                    });
                var body = new StringContent(content, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(VERIFY_TRANSACTION_URL, body, cancellationToken);
                if (response != null)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (responseBody == null) return null;

                    JObject resultObject = JObject.Parse(responseBody);
                    if (resultObject == null) return null;

                    return JsonConvert.DeserializeObject<VerifyTransactionModel>(resultObject.ToString());
                }

                return null;
            }
        }

        private async Task<VerifyTransactionModel> ReverseTransaction(string terminalId, string refNumber, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(
                    new
                    {
                        RefNum = refNumber,
                        TerminalNumber = terminalId,
                    });
                var body = new StringContent(content, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(REVERSE_TRANSACTION_URL, body, cancellationToken);
                if (response != null)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (responseBody == null) return null;

                    JObject resultObject = JObject.Parse(responseBody);
                    if (resultObject == null) return null;

                    return JsonConvert.DeserializeObject<VerifyTransactionModel>(resultObject.ToString());
                }

                return null;
            }
        }

        public async Task<SamanInternetPaymentCallBackModel> ValidatePaymentResponse(HttpContext context, CancellationToken cancellationToken)
        {
            string resNum, refNum, state;
            int status;
            if (context.Request.Query["ResNum"].Count == 0 ||
                context.Request.Query["RefNum"].Count == 0 ||
                context.Request.Query["TerminalId"].Count == 0 ||
                context.Request.Query["Status"].Count == 0 ||
                context.Request.Query["State"].Count == 0)
                return new SamanInternetPaymentCallBackModel()
                {
                    Message = "خطایی نامشخص رخ داده است",
                    IsSuccess = false
                };

            resNum = context.Request.Query["ResNum"];
            refNum = context.Request.Query["RefNum"];
            state = context.Request.Query["State"];
            status = Convert.ToInt32(context.Request.Query["Status"]);
            //var resNumBaseGUID = new Guid(resNum);

            var internetPayment = await samanInternetPaymentRepository.TableNoTracking
                .Include(p => p.PaymentReason)
                .Where(p => p.ResNum == resNum)
                .FirstOrDefaultAsync(cancellationToken);

            if (internetPayment != null)
            {
                if (internetPayment.IsSuccess.HasValue && internetPayment.IsSuccess.Value)
                {
                    return new SamanInternetPaymentCallBackModel()
                    {
                        Amount = internetPayment.Amount,
                        RefNum = internetPayment.RefNum,
                        SecurePan = internetPayment.SecurePan,
                        Message = internetPayment.State,
                        Rrn = internetPayment.RRN,
                        Status = internetPayment.Status.Value,
                        RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                        RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                        IsSuccess = true
                    };
                }

                internetPayment.Status = status;
                internetPayment.State = state;
                internetPayment.RefNum = refNum;
                internetPayment.TraceNo = context.Request.Query["TraceNo"];
                internetPayment.MId = context.Request.Query["MId"];
                internetPayment.RRN = context.Request.Query["RRN"];
                internetPayment.SecurePan = context.Request.Query["SecurePan"];
                internetPayment.HashedCardNumber = context.Request.Query["HashedCardNumber"];
                internetPayment.ReturnAmount = Convert.ToInt64(context.Request.Query["Amount"]);
                internetPayment.Wage = !string.IsNullOrEmpty(context.Request.Query["Wage"]) ? Convert.ToInt32(context.Request.Query["Wage"]) : null;
                if (state.ToUpper() != SuccessState)
                {
                    internetPayment.IsSuccess = false;

                    await samanInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken);
                    return new SamanInternetPaymentCallBackModel()
                    {
                        Message = $"تراکنش با خطا مواجه شد - {state}",
                        Amount = internetPayment.Amount,
                        RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                        RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                        IsSuccess = false
                    };
                }
                else
                {
                    if (!string.IsNullOrEmpty(internetPayment.HashCardNumberForPayment) &&
                        !internetPayment.HashCardNumberForPayment.Equals(internetPayment.HashedCardNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        var verifyResult = await ReverseTransaction(context.Request.Query["TerminalId"].ToString(), refNum, cancellationToken);
                        internetPayment.MethodName = "ReverseTransaction";
                        internetPayment.IsSuccess = false;
                        #region Reverse Transaction
                        if (verifyResult != null)
                        {
                            internetPayment.IsSuccess = false;
                            internetPayment.ResultCode = verifyResult.ResultCode;
                            internetPayment.ResultDescription = $"{verifyResult.ResultDescription}-تراکنش باید با شماره کارت {internetPayment.CardNumberForPayment} انجام شود";
                            if (verifyResult.TransactionDetail != null)
                            {
                                internetPayment.TransactionDetail_AffectiveAmount = verifyResult.TransactionDetail.AffectiveAmount;
                                internetPayment.TransactionDetail_HashedPan = verifyResult.TransactionDetail.HashedPan;
                                internetPayment.TransactionDetail_MaskedPan = verifyResult.TransactionDetail.MaskedPan;
                                internetPayment.TransactionDetail_OrginalAmount = verifyResult.TransactionDetail.OrginalAmount;
                                internetPayment.TransactionDetail_RefNum = verifyResult.TransactionDetail.RefNum;
                                internetPayment.TransactionDetail_RRN = verifyResult.TransactionDetail.RRN;
                                internetPayment.TransactionDetail_StraceDate = verifyResult.TransactionDetail.StraceDate;
                                internetPayment.TransactionDetail_StraceNo = verifyResult.TransactionDetail.StraceNo;
                                internetPayment.TransactionDetail_TerminalNumber = verifyResult.TransactionDetail.TerminalNumber;
                            }
                            //await samanInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken, false);
                            //return new SamanInternetPaymentCallBackModel()
                            //{
                            //    Message = $"تراکنش با خطا مواجه شد - {verifyResult.ResultDescription}(کد خطا : {verifyResult.ResultCode})",
                            //    Amount = internetPayment.Amount,
                            //    TraceNo = internetPayment.TraceNo,
                            //    RefNum = internetPayment.RefNum,
                            //    SecurePan = internetPayment.SecurePan,
                            //    Rrn = internetPayment.RRN,
                            //    RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                            //    RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                            //    IsSuccess = false
                            //};
                        }
                        //else
                        //{
                        internetPayment.IsSuccess = false;
                        await samanInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken);
                        return new SamanInternetPaymentCallBackModel()
                        {
                            Message = $"-تراکنش باید با شماره کارت {internetPayment.CardNumberForPayment} انجام شود - تراکنش با خطا مواجه شد - {state}",
                            Amount = internetPayment.Amount,
                            RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                            RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                            TraceNo = internetPayment.TraceNo,
                            RefNum = internetPayment.RefNum,
                            SecurePan = internetPayment.SecurePan,
                            Rrn = internetPayment.RRN,
                            IsSuccess = false
                        };
                        //}
                        #endregion Reverse Transaction
                    }
                    else
                    {
                        var verifyResult = await VerifyTransaction(context.Request.Query["TerminalId"].ToString(), refNum, cancellationToken);
                        internetPayment.MethodName = "VerifyTransaction";
                        #region Verify Transaction
                        if (verifyResult != null)
                        {
                            if (verifyResult.Success &&
                                verifyResult.ResultCode == SuccessVerifyCode &&
                                verifyResult.TransactionDetail.AffectiveAmount == verifyResult.TransactionDetail.OrginalAmount)
                            {
                                internetPayment.IsSuccess = verifyResult.Success;
                                internetPayment.ResultCode = verifyResult.ResultCode;
                                internetPayment.ResultDescription = verifyResult.ResultDescription;
                                internetPayment.TransactionDetail_AffectiveAmount = verifyResult.TransactionDetail.AffectiveAmount;
                                internetPayment.TransactionDetail_HashedPan = verifyResult.TransactionDetail.HashedPan;
                                internetPayment.TransactionDetail_MaskedPan = verifyResult.TransactionDetail.MaskedPan;
                                internetPayment.TransactionDetail_OrginalAmount = verifyResult.TransactionDetail.OrginalAmount;
                                internetPayment.TransactionDetail_RefNum = verifyResult.TransactionDetail.RefNum;
                                internetPayment.TransactionDetail_RRN = verifyResult.TransactionDetail.RRN;
                                internetPayment.TransactionDetail_StraceDate = verifyResult.TransactionDetail.StraceDate;
                                internetPayment.TransactionDetail_StraceNo = verifyResult.TransactionDetail.StraceNo;
                                internetPayment.TransactionDetail_TerminalNumber = verifyResult.TransactionDetail.TerminalNumber;

                                await samanInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken, false);

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
                                            PaymentId = internetPayment.Id,
                                            RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                                            RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                                            RequestFacilityInstallmentId = internetPayment.PaymentReason.RequestFacilityInstallmentId,

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
                                            new WalletTransaction()
                                            {
                                                WalletTransactionType = WalletTransactionTypeEnum.Withdraw,
                                                Amount=-internetPayment.Amount,
                                                CreatorId=system_account_user_Id,
                                                UserId = paymentInfo.BuyerId,
                                                Description =$"برداشت بابت خرید از فروشگاه '{paymentInfo.ShopName}'"
                                            },
                                            new WalletTransaction()
                                            {
                                                WalletTransactionType = WalletTransactionTypeEnum.Deposit,
                                                Amount= shopAmount,
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
                                            }
                                        }, cancellationToken, saveNow: true);
                                }
                                return new SamanInternetPaymentCallBackModel()
                                {
                                    Amount = internetPayment.Amount,
                                    RefNum = internetPayment.TransactionDetail_RefNum,
                                    SecurePan = internetPayment.TransactionDetail_MaskedPan,
                                    Message = internetPayment.ResultDescription,
                                    TraceNo = internetPayment.TraceNo,
                                    Rrn = internetPayment.TransactionDetail_RRN,
                                    RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                                    RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                                    IsSuccess = true
                                };
                            }
                            else
                            {
                                internetPayment.IsSuccess = false;
                                internetPayment.ResultCode = verifyResult.ResultCode;
                                internetPayment.ResultDescription = verifyResult.ResultDescription;
                                if (verifyResult.TransactionDetail != null)
                                {
                                    internetPayment.TransactionDetail_AffectiveAmount = verifyResult.TransactionDetail.AffectiveAmount;
                                    internetPayment.TransactionDetail_HashedPan = verifyResult.TransactionDetail.HashedPan;
                                    internetPayment.TransactionDetail_MaskedPan = verifyResult.TransactionDetail.MaskedPan;
                                    internetPayment.TransactionDetail_OrginalAmount = verifyResult.TransactionDetail.OrginalAmount;
                                    internetPayment.TransactionDetail_RefNum = verifyResult.TransactionDetail.RefNum;
                                    internetPayment.TransactionDetail_RRN = verifyResult.TransactionDetail.RRN;
                                    internetPayment.TransactionDetail_StraceDate = verifyResult.TransactionDetail.StraceDate;
                                    internetPayment.TransactionDetail_StraceNo = verifyResult.TransactionDetail.StraceNo;
                                    internetPayment.TransactionDetail_TerminalNumber = verifyResult.TransactionDetail.TerminalNumber;
                                }
                                await samanInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken, false);
                                return new SamanInternetPaymentCallBackModel()
                                {
                                    Message = $"تراکنش با خطا مواجه شد - {verifyResult.ResultDescription}(کد خطا : {verifyResult.ResultCode})",
                                    Amount = internetPayment.Amount,
                                    TraceNo = internetPayment.TraceNo,
                                    RefNum = internetPayment.RefNum,
                                    SecurePan = internetPayment.SecurePan,
                                    Rrn = internetPayment.RRN,
                                    RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                                    RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                                    IsSuccess = false
                                };
                            }
                        }
                        else
                        {
                            internetPayment.IsSuccess = false;
                            await samanInternetPaymentRepository.UpdateAsync(internetPayment, cancellationToken);
                            return new SamanInternetPaymentCallBackModel()
                            {
                                Message = $"تراکنش با خطا مواجه شد - {state}",
                                Amount = internetPayment.Amount,
                                RequestFacilityId = internetPayment.PaymentReason.RequestFacilityId,
                                RequestFacilityGuarantorId = internetPayment.PaymentReason.RequestFacilityGuarantorId,
                                TraceNo = internetPayment.TraceNo,
                                RefNum = internetPayment.RefNum,
                                SecurePan = internetPayment.SecurePan,
                                Rrn = internetPayment.RRN,
                                IsSuccess = false
                            };
                        }
                        #endregion Verify Transaction
                    }
                }
            }

            return new SamanInternetPaymentCallBackModel()
            {
                Message = "خطایی نامشخص رخ داده است",
                Amount = internetPayment.Amount,
                IsSuccess = false
            };
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
        //    return await samanInternetPaymentRepository
        //                .TableNoTracking
        //                .AnyAsync(p => p.RequestFacilityId == requestFacilityId &&
        //                         p.PaymentType == paymentType &&
        //                         p.IsSuccess == true &&
        //                         p.State == SuccessState,
        //                         cancellationToken);
        //}

        //public async Task<bool> ExistSuccessfulPayment(Guid userId, PaymentType paymentType, CancellationToken cancellationToken)
        //{
        //    return await samanInternetPaymentRepository
        //                .TableNoTracking
        //                .AnyAsync(p => p.UserId == userId &&
        //                               p.PaymentType == paymentType &&
        //                               p.IsSuccess == true &&
        //                               p.State == SuccessState,
        //                               cancellationToken);
        //}
    }
}
