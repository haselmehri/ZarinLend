using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Services
{
    public class SignContractService(ILogger<SignContractService> logger,
                                     IOptionsSnapshot<SiteSettings> siteSettings,
                                     IRequestFacilityRepository requestFacilityRepository,
                                     IRequestFacilityService requestFacilityService,
                                     IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
                                     IAyandehSignService ayandehSignService,
                                     INeoZarinService neoZarinService,
                                     IUserRepository userRepository,
                                     IWebHostEnvironment webHostEnvironment) : ISignContractService, IScopedDependency
    {
        private readonly SiteSettings siteSettings = siteSettings.Value;

        public async Task<Tuple<bool, string>> SignContractByNeoZarin(Guid userId, int requestFacilityId, byte[] contract, string baseUrl, CancellationToken cancellationToken = default)
        {
            var facilityInfo = await requestFacilityRepository.TableNoTracking
                .Where(p => p.Id == requestFacilityId)
                .Select(p => new
                {
                    //OrganizationName = p.Organization.Name,
                    p.Amount,
                    p.FacilityNumber,
                    p.FacilityType.MonthCount,
                    p.ContractFileName
                })
                .FirstOrDefaultAsync();

            #region Sign Contract & Save Contract file
            var contractFileName = !string.IsNullOrEmpty(facilityInfo.ContractFileName) ? Path.GetFileNameWithoutExtension(facilityInfo.ContractFileName) : $"{Guid.NewGuid()}";
            string path = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{contractFileName}");
            ////System.IO.File.WriteAllBytes(path, pdf.Data);

            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await stream.WriteAsync(contract, 0, contract.Length);
                await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                {
                    Id = requestFacilityId,
                    ContractFileName = $"{contractFileName}.pdf"
                }, cancellationToken, true, nameof(RequestFacility.ContractFileName));
            }
            var mobile = await userRepository.TableNoTracking.Where(p => p.Id == userId).Select(p => p.Person.Mobile).FirstOrDefaultAsync(cancellationToken);
            var callBackUrl = $"{baseUrl}/File/ContractSignatureCallback/{requestFacilityId}/{contractFileName}";
            var fileUrl = $"{baseUrl}/File/ContractForSignature/{requestFacilityId}/{contractFileName}.pdf";
            var resultRequestSign = await neoZarinService.SignContract(requestFacilityId, userId, callBackUrl, fileUrl, mobile,
                trackId: contractFileName,
                description: $"امضاء قرارداد تسهیلات-مبلغ : {facilityInfo.Amount:N0}-شماره تسهیلات : {facilityInfo.FacilityNumber}",
                cancellationToken);

            #endregion Sign Contract & Save Contract file

            var currentStepIsSignContractByUser = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.SignContractByUser, cancellationToken);
            if (resultRequestSign && currentStepIsSignContractByUser)
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                  WorkFlowFormEnum.SignContractByUser,
                  StatusEnum.Approved,
                  opratorId: userId,
                  "درخواست امضای قرارداد توسط اپلیکیشن نئوزرین",
                  cancellationToken);

            return new Tuple<bool, string>(resultRequestSign, callBackUrl);
        }
        public async Task<bool> SignContractByAyandehSign(Guid userId, int requestFacilityId, byte[] contract, CancellationToken cancellationToken = default)
        {
            var facilityInfo = await requestFacilityRepository.TableNoTracking
                .Where(p => p.Id == requestFacilityId)
                .Select(p => new
                {
                    p.Amount,
                    p.FacilityNumber,
                    p.FacilityType.MonthCount,
                    p.ContractFileName
                })
                .FirstOrDefaultAsync();

            #region Sign Contract & Save Contract file
            var contractFileName = !string.IsNullOrEmpty(facilityInfo!.ContractFileName) ? Path.GetFileNameWithoutExtension(facilityInfo.ContractFileName) : $"{Guid.NewGuid()}";
            string contractPath = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{contractFileName}.pdf");
            ////System.IO.File.WriteAllBytes(path, pdf.Data);

            using (FileStream stream = new FileStream(contractPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await stream.WriteAsync(contract, 0, contract.Length);
                await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                {
                    Id = requestFacilityId,
                    ContractFileName = $"{contractFileName}.pdf"
                }, cancellationToken, true, nameof(RequestFacility.ContractFileName));
            }
            var personInfo = await userRepository.TableNoTracking.Where(p => p.Id == userId)
                .Select(p => new { p.Person.Mobile, p.Person.NationalCode }).FirstOrDefaultAsync(cancellationToken);

            byte[] pdfBytes = File.ReadAllBytes(contractPath);
            string pdfBase64 = Convert.ToBase64String(pdfBytes);
            var pdfToSign = HttpUtility.UrlEncode(pdfBase64);
            var callBackUrl = $"{siteSettings.SigningSettings.AyandehSignCallback}/file/AyandehSignPostCallback/{requestFacilityId}";

            var signingToken = await ayandehSignService.GetSigningToken(requestFacilityId,
                                                                        userId,
                                                                        pdfToSign,
                                                                        personInfo!.NationalCode,
                                                                        personInfo.Mobile,
                                                                        title: "امضاء قرارداد دریافت تسهیلات",
                                                                        hint: $"امضاء قرارداد تسهیلات-مبلغ : {facilityInfo.Amount:N0}-شماره تسهیلات : {facilityInfo.FacilityNumber}",
                                                                        callBackUrl,
                                                                        cancellationToken);

            if (!signingToken.IsSuccess)
                throw new AppException(signingToken.ErrorMessage);

            var currentStepIsSignContractByUser = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.SignContractByUser, cancellationToken);

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacilityId,
                AyandehSignSigningToken = signingToken.SigningToken
            }, cancellationToken, !currentStepIsSignContractByUser, nameof(RequestFacility.AyandehSignSigningToken));


            #endregion Sign Contract & Save Contract file

            if (currentStepIsSignContractByUser)
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                  WorkFlowFormEnum.SignContractByUser,
                  StatusEnum.Approved,
                  opratorId: userId,
                  "درخواست امضای قرارداد توسط اپلیکیشن آینده ساین",
                  cancellationToken);

            return !string.IsNullOrEmpty(signingToken.SigningToken);
        }

        public async Task<bool> SignContractByAyandehSign(Guid bankManagerUserId, int leasingId, int requestFacilityId, bool justSignContract, CancellationToken cancellationToken = default)
        {
            if (!justSignContract)
                if (!await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
                                                                                                 leasingId,
                                                                                                 new List<RoleEnum> { RoleEnum.AdminBankLeasing },
                                                                                                 WorkFlowFormEnum.AdminBankLeasingSignature,
                                                                                                 cancellationToken))
                    throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'امضای قرارداد/یا در انتظار امضای قرارداد' نمی باشد");

            var facilityInfo = await requestFacilityRepository.TableNoTracking
                .Where(p => p.Id == requestFacilityId)
                .Select(p => new
                {
                    p.Amount,
                    p.FacilityNumber,
                    p.FacilityType.MonthCount,
                    p.SignedContractByUserFileName
                })
                .FirstOrDefaultAsync();

            #region Sign Contract & Save Contract file

            var signedContractFileName = await requestFacilityService.GetSignedContractByUserFileName(requestFacilityId, cancellationToken);
            string filePath = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{signedContractFileName}");
            var pdfToSign = string.Empty;
            if (File.Exists(filePath))
            {
                byte[] pdfBytes = File.ReadAllBytes(filePath);
                string pdfBase64 = Convert.ToBase64String(pdfBytes);
                pdfToSign = HttpUtility.UrlEncode(pdfBase64);
            }
            else
                throw new AppException("فایل قرارداد یافت نشد!");

            var personInfo = await userRepository.TableNoTracking
                .Where(p => p.Id == bankManagerUserId && p.Person.OrganizationId == leasingId && p.UserRoles.Any(x => x.Role.Name == RoleEnum.AdminBankLeasing.ToString()))
                .Select(p => new
                {
                    p.Person.Mobile,
                    p.Person.NationalCode
                }).FirstOrDefaultAsync(cancellationToken);

            if (personInfo == null)
                throw new AppException("اطلاعات امضاء کننده یافت نشد!");

            var callBackUrl = $"{siteSettings.SigningSettings.AyandehSignCallback}/file/BankAdminAyandehSignPostCallback/{requestFacilityId}/{leasingId}";
            var signingToken = await ayandehSignService.GetSigningToken(requestFacilityId,
                                                                        bankManagerUserId,
                                                                        pdfToSign,
                                                                        personInfo!.NationalCode,
                                                                        personInfo.Mobile,
                                                                        title: "امضاء قرارداد دریافت تسهیلات توسط مدیر بانک",
                                                                        hint: $"امضاء قرارداد تسهیلات مدیر بانک-مبلغ : {facilityInfo!.Amount:N0}-شماره تسهیلات : {facilityInfo.FacilityNumber}",
                                                                        callBackUrl,
                                                                        cancellationToken);

            if (!signingToken.IsSuccess)
                throw new AppException(signingToken.ErrorMessage);

            logger.LogInformation($"SSS : {signingToken}");
            //var currentStepIsSignContractByUser = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(bankManagerUserId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
            //    WorkFlowFormEnum.SignContractByUser, cancellationToken);

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacilityId,
                AyandehSignSigningTokenForAdminBank = signingToken.SigningToken
            },
            cancellationToken,
            saveNow: true,
            nameof(RequestFacility.AyandehSignSigningTokenForAdminBank));


            #endregion Sign Contract & Save Contract file

            //if (!string.IsNullOrEmpty(signingToken))
            //    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
            //      WorkFlowFormEnum.AdminBankLeasingSignature,
            //      StatusEnum.Approved,
            //      opratorId: bankManagerUserId,
            //      "درخواست امضای قرارداد توسط اپلیکیشن آینده ساین(مدیر شعبه)",
            //      cancellationToken);

            return !string.IsNullOrEmpty(signingToken.SigningToken);
        }
    }
}
