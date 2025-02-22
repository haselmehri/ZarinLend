using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class SamatService : ISamatService, IScopedDependency
    {
        private readonly ILogger<SamatService> logger;
        private readonly IBaseRepository<SamatFacilityHeader> samatFacilityHeaderRepository;
        private readonly IBaseRepository<SamatBackChequeHeader> samatBackChequeHeaderRepository;
        private readonly IUserRepository userRepository;

        public SamatService(ILogger<SamatService> logger,
                            IBaseRepository<SamatFacilityHeader> samatFacilityHeaderRepository, 
                            IBaseRepository<SamatBackChequeHeader> samatBackChequeHeaderRepository,
                            IUserRepository userRepository)
        {
            this.logger = logger;
            this.samatFacilityHeaderRepository = samatFacilityHeaderRepository;
            this.samatBackChequeHeaderRepository = samatBackChequeHeaderRepository;
            this.userRepository = userRepository;
        }

        public async Task<bool> InquiryDone(int requestFacilityId, CancellationToken cancellationToken = default)
        {
            return await samatFacilityHeaderRepository.TableNoTracking.AnyAsync(p => p.RequestFacilityId.Equals(requestFacilityId), cancellationToken) &&
                   await samatBackChequeHeaderRepository.TableNoTracking.AnyAsync(p => p.RequestFacilityId.Equals(requestFacilityId), cancellationToken);
        }

        #region Facility
        public async Task<SamatFacilityHeaderModel> GetUserFacilitiesFromDB(int requestFacilityId, CancellationToken cancellationToken)
        {
            return await samatFacilityHeaderRepository
                .TableNoTracking
                .Where(p => p.RequestFacilityId.Equals(requestFacilityId))
                .OrderByDescending(p => p.CreatedDate)
                .Take(1)
                .Select(p => new SamatFacilityHeaderModel()
                {
                    RequestFacilityId = p.RequestFacilityId,
                    Name = p.Name,
                    FacilityDebtTotalAmount = p.FacilityDebtTotalAmount,
                    FacilityDeferredTotalAmount = p.FacilityDeferredTotalAmount,
                    FacilityPastExpiredTotalAmount = p.FacilityPastExpiredTotalAmount,
                    FacilitySuspiciousTotalAmount = p.FacilitySuspiciousTotalAmount,
                    FacilityTotalAmount = p.FacilityTotalAmount,
                    CreatedDate = p.CreatedDate,
                    CreatorName = $"{p.Creator.Person.FName} {p.Creator.Person.LName}",
                    FacilityList = p.SamatFacilityDetails.Select(x => new SamatFacilityDetailModel()
                    {
                        BranchDescription = x.BranchDescription,
                        FacilityAmountOrginal = x.FacilityAmountOrginal,
                        FacilityBankCode = x.FacilityBankCode,
                        FacilityBenefitAmount = x.FacilityBenefitAmount,
                        FacilityBranch = x.FacilityBranch,
                        FacilityBranchCode = x.FacilityBranchCode,
                        FacilityDebtorTotalAmount = x.FacilityDebtorTotalAmount,
                        FacilityDeferredAmount = x.FacilityDeferredAmount,
                        FacilityPastExpiredAmount = x.FacilityPastExpiredAmount,
                        FacilitySuspiciousAmount = x.FacilitySuspiciousAmount,
                        FacilityEndDate = x.FacilityEndDate,
                        FacilitySetDate = x.FacilitySetDate
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<JObject> FacilityInquiry(string nationalCode, CancellationToken cancellationToken)
        {
            var token = await RefreshToken();
            if (token == null) throw new AppException("return result from 'RefreshToken()' is null!");

            if (string.IsNullOrEmpty(nationalCode)) throw new AppException("'nationalCode' is null!");

            using (var client = new HttpClient())
            {
                string requesturl = $"https://apibeta.finnotech.ir/credit/v2/clients/zarinpal1/users/{nationalCode}/facilityInquiry";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                //HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject resultObject = JObject.Parse(responseBody);
                return resultObject;
            }
        }

        public async Task<bool> GetUserFacilitiesFromCentralBank(int requestFacilityId, Guid requestFacilityUserId, Guid creatorId, CancellationToken cancellationToken)
        {
            var nationalCode = await userRepository.TableNoTracking.Where(p => p.Id.Equals(requestFacilityUserId)).Select(p => p.Person.NationalCode).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(nationalCode)) throw new AppException("'nationalCode' is null!");

            try
            {
                var resultObject = await FacilityInquiry(nationalCode, cancellationToken);

                if (resultObject.ContainsKey("status") && resultObject["status"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                {
                    if ((resultObject["result"] as JObject).ContainsKey("facilityList") &&
                        resultObject["result"]["facilityList"].GetType() == typeof(JArray) &&
                        (resultObject["result"]["facilityList"] as JArray).Count > 0)
                    {
                        var facilityListModel = JsonConvert.DeserializeObject<List<SamatFacilityDetailModel>>(resultObject["result"]["facilityList"].ToString());
                        var samatFacilityResultModel = JsonConvert.DeserializeObject<SamatFacilityHeaderModel>(resultObject["result"].ToString());
                        samatFacilityResultModel.FacilityList = facilityListModel;

                        #region First Delete before Facility Data from DB & new Facility Data Insert To DB
                        //await DeletePreviousFacilitiesFromDB(userId);

                        await samatFacilityHeaderRepository.AddAsync(new SamatFacilityHeader()
                        {
                            RequestFacilityId = requestFacilityId,
                            CreatorId = creatorId,
                            FacilityDebtTotalAmount = samatFacilityResultModel.FacilityDebtTotalAmount,
                            FacilityDeferredTotalAmount = samatFacilityResultModel.FacilityDeferredTotalAmount,
                            FacilityPastExpiredTotalAmount = samatFacilityResultModel.FacilityPastExpiredTotalAmount,
                            FacilitySuspiciousTotalAmount = samatFacilityResultModel.FacilitySuspiciousTotalAmount,
                            FacilityTotalAmount = samatFacilityResultModel.FacilityTotalAmount,
                            Name = samatFacilityResultModel.Name,
                            SamatFacilityDetails = facilityListModel.Select(p => new SamatFacilityDetail()
                            {
                                BranchDescription = p.BranchDescription,
                                FacilityAmountOrginal = p.FacilityAmountOrginal,
                                FacilityBankCode = p.FacilityBankCode,
                                FacilityBenefitAmount = p.FacilityBenefitAmount,
                                FacilityBranch = p.FacilityBranch,
                                FacilityBranchCode = p.FacilityBranchCode,
                                FacilityDebtorTotalAmount = p.FacilityDebtorTotalAmount,
                                FacilityDeferredAmount = p.FacilityDeferredAmount,
                                FacilityPastExpiredAmount = p.FacilityPastExpiredAmount,
                                FacilitySuspiciousAmount = p.FacilitySuspiciousAmount,
                                FacilityEndDate = p.FacilityEndDate,
                                FacilitySetDate = p.FacilitySetDate,
                            }).ToList()
                        }, cancellationToken);
                        #endregion
                    }
                    else
                    {
                        #region First Delete before Facility Data from DB & log Insert To DB
                        //await DeletePreviousFacilitiesFromDB(userId);

                        //"message": "هیچ تسهیلات و تعهدی یافت نشد"
                        await samatFacilityHeaderRepository.AddAsync(new SamatFacilityHeader()
                        {
                            RequestFacilityId = requestFacilityId,
                            CreatorId = creatorId,
                            FacilityDebtTotalAmount = 0,
                            FacilityDeferredTotalAmount = 0,
                            FacilityPastExpiredTotalAmount = 0,
                            FacilitySuspiciousTotalAmount = 0,
                            FacilityTotalAmount = 0,
                            Name = string.Empty
                        }, cancellationToken);
                        #endregion
                    }

                    return true;
                }
                else
                {
                    if (resultObject.ContainsKey("error") && (resultObject["error"] as JObject).ContainsKey("code") && (resultObject["error"] as JObject).ContainsKey("message"))
                        logger.LogWarning($"status : {resultObject["status"]} , code :{(resultObject["error"] as JObject)["code"]} , message :{(resultObject["error"] as JObject)["message"]} ");
                    else
                        logger.LogWarning($"GetUserFacilityFromCentralBank : requestFacilityId : {requestFacilityId}, userId :{requestFacilityUserId}. unexpected error!");
                    //return BadRequest("کد ملی صحیح نمی باشد!");

                    return false;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return false;
            }

            //async Task DeletePreviousFacilitiesFromDB(Guid userId)
            //{
            //    var samatFacilityHeader = await samatFacilityHeaderRepository.TableNoTracking
            //                 .Where(p => p.UserId.Equals(userId))
            //                 .Select(p => new
            //                 {
            //                     SamatFacilityHeader = p,
            //                     p.SamatFacilityDetails,
            //                 })
            //                 .FirstOrDefaultAsync();

            //    if (samatFacilityHeader != null)
            //    {
            //        if (samatFacilityHeader.SamatFacilityDetails != null && samatFacilityHeader.SamatFacilityDetails.Any())
            //            await samatFacilityDetailRepository.DeleteRangeAsync(samatFacilityHeader.SamatFacilityDetails, cancellationToken, false);

            //        await samatFacilityHeaderRepository.DeleteAsync(samatFacilityHeader.SamatFacilityHeader, cancellationToken, false);
            //    }
            //}
        }
        #endregion

        #region Back Cheque 
        public async Task<SamatBackChequeHeaderModel> GetUserBackChequesFromDB(int requestFacilityId, CancellationToken cancellationToken)
        {
            return await samatBackChequeHeaderRepository
                .TableNoTracking
                .Where(p => p.RequestFacilityId.Equals(requestFacilityId))
                .OrderByDescending(p => p.CreatedDate)
                .Take(1)
                .Select(p => new SamatBackChequeHeaderModel()
                {
                    RequestFacilityId = p.RequestFacilityId,
                    Name = p.Name,
                    CreatorName = $"{p.Creator.Person.FName} {p.Creator.Person.LName}",
                    CreatedDate = p.CreatedDate,
                    BackChequeList = p.SamatBackChequeDetails.Select(x => new SamatBackChequeDetailModel()
                    {
                        BranchDescription = x.BranchDescription,
                        AccountNumber = x.AccountNumber,
                        Amount = x.Amount,
                        BackDate = x.BackDate,
                        BankCode = x.BankCode,
                        BranchCode = x.BranchCode,
                        Date = x.Date,
                        Number = x.Number
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<JObject> BackCheques(string nationalCode, CancellationToken cancellationToken)
        {
            var token = await RefreshToken();
            if (token == null) throw new AppException("return result from 'RefreshToken()' is null!");

            if (string.IsNullOrEmpty(nationalCode)) throw new AppException("'nationalCode' is null!");

            using (var client = new HttpClient())
            {
                string requesturl = $"https://apibeta.finnotech.ir/credit/v2/clients/zarinpal1/users/{nationalCode}/backCheques";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                //HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                string responseBody = await response.Content.ReadAsStringAsync();

                return JObject.Parse(responseBody);
            }
        }

        public async Task<bool> GetUserBackChequesFromCentralBank(int requestFacilityId, Guid requestFacilityUserId, Guid creatorId, CancellationToken cancellationToken)
        {
            var nationalCode = await userRepository.TableNoTracking.Where(p => p.Id.Equals(requestFacilityUserId)).Select(p => p.Person.NationalCode).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(nationalCode)) throw new AppException("'nationalCode' is null!");

            try
            {
                JObject resultObject = await BackCheques(nationalCode, cancellationToken);

                if (resultObject.ContainsKey("status") && resultObject["status"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                {
                    if ((resultObject["result"] as JObject).ContainsKey("chequeList") &&
                        resultObject["result"]["chequeList"].GetType() == typeof(JArray) &&
                        (resultObject["result"]["chequeList"] as JArray).Count > 0)
                    {
                        var fullName = resultObject["name"].ToString();
                        var backChequeList = JsonConvert.DeserializeObject<List<SamatBackChequeDetailModel>>(resultObject["result"]["chequeList"].ToString());

                        #region First Delete before Back Cheque Data from DB & new Back Cheque Data Insert To DB
                        //await DeletePreviousBackChequesFromDB(userId);

                        await samatBackChequeHeaderRepository.AddAsync(new SamatBackChequeHeader()
                        {
                            RequestFacilityId = requestFacilityId,
                            CreatorId = creatorId,
                            Name = fullName,
                            SamatBackChequeDetails = backChequeList.Select(p => new SamatBackChequeDetail()
                            {
                                BranchDescription = p.BranchDescription,
                                AccountNumber = p.AccountNumber,
                                Amount = p.Amount,
                                BackDate = p.BackDate,
                                BankCode = p.BankCode,
                                BranchCode = p.BranchCode,
                                Date = p.Date,
                                Number = p.Number
                            }).ToList()
                        }, cancellationToken);
                        #endregion
                    }
                    else
                    {
                        #region First Delete before Back Cheque Data from DB & log Insert To DB
                        //await DeletePreviousBackChequesFromDB(userId);

                        //"message": "هیچ تسهیلات و تعهدی یافت نشد"
                        await samatBackChequeHeaderRepository.AddAsync(new SamatBackChequeHeader()
                        {
                            RequestFacilityId = requestFacilityId,
                            CreatorId = creatorId
                        }, cancellationToken);
                        #endregion
                    }
                    return true;
                }
                else
                {
                    if (resultObject.ContainsKey("error") && (resultObject as JObject).ContainsKey("code") && (resultObject as JObject).ContainsKey("message"))
                        logger.LogWarning($"status : {resultObject["status"]} , code :{(resultObject["error"] as JObject)["code"]} , message :{(resultObject["error"] as JObject)["message"]} ");
                    else
                        logger.LogWarning($"GetUserBackChequesFromCentralBank : requestFacilityId : {requestFacilityId}, userId :{requestFacilityUserId}. unexpected error!");
                    //return BadRequest("کد ملی صحیح نمی باشد!");

                    return false;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return false;
            }

            //async Task DeletePreviousBackChequesFromDB(Guid userId)
            //{
            //    var samatBackChequeHeader = await samatBackChequeHeaderRepository.TableNoTracking
            //                 .Where(p => p.UserId.Equals(userId))
            //                 .Select(p => new
            //                 {
            //                     SamatBackChequeHeader = p,
            //                     p.SamatBackChequeDetails,
            //                 })
            //                 .FirstOrDefaultAsync();

            //    if (samatBackChequeHeader != null)
            //    {
            //        if (samatBackChequeHeader.SamatBackChequeDetails != null && samatBackChequeHeader.SamatBackChequeDetails.Any())
            //            await samatBackChequeDetailRepository.DeleteRangeAsync(samatBackChequeHeader.SamatBackChequeDetails, cancellationToken, false);

            //        await samatBackChequeHeaderRepository.DeleteAsync(samatBackChequeHeader.SamatBackChequeHeader, cancellationToken, false);
            //    }
            //}
        }
        #endregion
        private async Task<string> RefreshToken()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = "https://apibeta.finnotech.ir/dev/v2/oauth2/token";
                    var grant_type = "refresh_token";
                    var token_type = "CLIENT-CREDENTIAL";
                    var refresh_token = "NT5Mu6kuCql6tgScar5D9nBjImjRUTJxGUONFOh1ud2j1hNRlvNTyOdk7akMJpKgEgnxhn8Hy4MMnBQwxLjvHUzZ05nivBEDdFESAMduVMNVRaP2eUOy5fuOKP3sXUB5mkv03cNTRyCwX5EppjyzsZogrqV77R0Hi6SYUS11ERT6xWAOSWCUGTCfeAeMuL4Onm0tNLMrrgj1n0i8uMVOTLBEygyepnyQ35CyljqHzXsFi707JUVYmlhx7oqmCaT7";

                    var body = JsonConvert.SerializeObject(new { grant_type = grant_type, token_type = token_type, refresh_token = refresh_token });
                    client.DefaultRequestHeaders.Add("Authorization", "Basic emFyaW5wYWwxOjYzNWViYmJhNjdjODU1MGQ0MGM3");
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("status") && resultObject["status"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if ((resultObject["result"] as JObject).ContainsKey("value"))
                            return (resultObject["result"] as JObject)["value"].ToString();
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
