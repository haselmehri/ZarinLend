using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.Promissory;

namespace Services
{
    public class RequestFacilityPromissoryService(ILogger<RequestFacilityPromissoryService> logger,
                                                  IRequestFacilityRepository requestFacilityRepository,
                                                  IBaseRepository<RequestFacilityPromissory> requestFacilityPromissoryRepository,
                                                  IFinnotechService finnotechService,
                                                  IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
                                                  IOptionsSnapshot<SiteSettings> siteSettings) : IRequestFacilityPromissoryService, IScopedDependency
    {
        private readonly IOptionsSnapshot<SiteSettings> siteSettings = siteSettings;

        //public async Task<List<DocumentModel>> GetRequestFacilityWarantyDocument(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        //{
        //    return await requestFacilityWarrantyRepository.SelectByAsync(p => p.RequestFacilityId.Equals(requestFacilityId) &&
        //     p.RequestFacility.BuyerId.Equals(userId) &&
        //     !p.IsDeleted,
        //       p => new DocumentModel()
        //       {
        //           Id = p.Id,
        //           CreatedDate = p.CreatedDate,
        //           FilePath = p.FilePath,
        //           Version = p.Version,
        //           FileType = p.FileType,
        //           DocumentType = p.DocumentType,
        //           DocumentNumber = p.DocumentNumber,
        //       }, cancellationToken);
        //}

        private async Task<RequestFacilityPromissoryModel?> getRequestFacilityPromissory(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            return await requestFacilityPromissoryRepository.TableNoTracking
                .Where(p => p.RequestFacilityId.Equals(requestFacilityId) && p.RequestFacility.BuyerId.Equals(userId) && p.IsActive)
                .Select(p => new RequestFacilityPromissoryModel()
                {
                    Id = p.Id,
                    RequestFacilityId = requestFacilityId,
                    IssuerName = p.IssuerName,
                    IssuerMobile = p.IssuerMobile,
                    IssuerIdCode = p.IssuerIdCode,
                    IssuerIban = p.IssuerIban,
                    IssuerPostalCode = p.IssuerPostalCode,
                    ReceiverIdCode = p.ReceiverIdCode,
                    ReceiverName = p.ReceiverName,
                    ReceiverMobile = p.ReceiverMobile,
                    PaymentPlace = p.PaymentPlace,
                    Description = p.Description,
                    BirthDate = p.BirthDate,
                    IssuerAddress = p.IssuerAddress,
                    Amount = p.Amount,
                    TrackId = p.TrackId,
                    PromissoryId = p.PromissoryId,
                    RequestId = p.RequestId,
                    SigningStatus = p.SigningStatus,
                    SigningTrackId = p.SigningTrackId,
                    UnSignedPdf = p.UnSignedPdf,
                    MultiSignedPdf = p.MultiSignedPdf,
                    CreateDate = p.CreatedDate,
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<RequestFacilityPromissoryModel?> GetFinalizePromissory(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            return await requestFacilityPromissoryRepository.TableNoTracking
               .Where(p => p.RequestFacilityId.Equals(requestFacilityId) &&
                           p.RequestFacility.BuyerId.Equals(userId) &&
                           !string.IsNullOrEmpty(p.SigningTrackId) &&
                           p.UnSignedPdf != null &&
                           p.IsActive)
               .Select(p => new RequestFacilityPromissoryModel()
               {
                   Id = p.Id,
                   RequestFacilityId = requestFacilityId,
                   ReceiverIdCode = p.ReceiverIdCode,
                   ReceiverName = p.ReceiverName,
                   Amount = p.Amount,
                   TrackId = p.TrackId,
                   PromissoryId = p.PromissoryId,
                   RequestId = p.RequestId,
                   SigningStatus = p.SigningStatus,
                   SigningTrackId = p.SigningTrackId,
                   UnSignedPdf = p.UnSignedPdf,
                   MultiSignedPdf = p.MultiSignedPdf,
                   IssuerName = p.IssuerName,
                   IssuerIdCode = p.IssuerIdCode,
                   BirthDate = p.BirthDate,
                   IssuerIban = p.IssuerIban,
                   IssuerAddress = p.IssuerAddress,
                   IssuerMobile = p.IssuerMobile
               })
               .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<RequestFacilityPromissoryModel> GetRequestFacilityPromissory(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {          
            var requestFacilityPromissory = await getRequestFacilityPromissory(userId, requestFacilityId, cancellationToken);
            if (requestFacilityPromissory != null && (DateTime.Now - requestFacilityPromissory.CreateDate).Minutes >= 1410)
            {
                var requestId = requestFacilityPromissory.RequestId;//await finnotechService.PromissoryPublishRequestInquiry(requestFacilityPromissory.IssuerIdCode, userId, cancellationToken);
                if (!string.IsNullOrEmpty(requestId))
                { 
                    await finnotechService.PromissoryDelete(requestId,userId, cancellationToken);
                    await requestFacilityPromissoryRepository.UpdateCustomPropertiesAsync(new RequestFacilityPromissory()
                    {
                        Id = requestFacilityPromissory.Id,
                        IsActive = false
                    },
                    cancellationToken,
                    saveNow: true,
                    nameof(RequestFacilityPromissory.IsActive));
                }
            }
            requestFacilityPromissory = await getRequestFacilityPromissory(userId, requestFacilityId, cancellationToken);
            if (requestFacilityPromissory == null)
            {
                var requestFacilityDetailForPromissory = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) &&
                    p.BuyerId.Equals(userId),
                    p => new
                    {
                        RequestFacilityId = p.Id,
                        UserId = p.BuyerId,
                        IssuerName = $"{p.Buyer.Person.FName} {p.Buyer.Person.LName}",
                        IssuerIdCode = p.Buyer.Person.NationalCode,
                        IssuerMobile = p.Buyer.Person.Mobile,
                        IssuerBirthDate = p.Buyer.Person.BirthDate,
                        IssuerIban = p.Buyer.UserBankAccounts.Where(p => p.IsConfirm && !p.IsDeleted).Select(p => p.IBAN).FirstOrDefault()!,
                        IssuerPostalCode = p.Buyer.Person.PostalCode,
                        IssuerAddress = p.Buyer.Person.Address,
                        LeasingTitle = p.Organization.Name,
                        LeasingNationalId = p.Organization.NationalId,
                        p.FacilityNumber,
                        p.Amount,
                        p.FacilityType.MonthCount,
                        p.GlobalSetting.WarantyPercentage,
                        p.GlobalSetting.FacilityInterest
                    }, cancellationToken))
                    .FirstOrDefault()!;

                var trackId = Guid.NewGuid().ToString();
                var promissoryRequest = new PromissoryPublishRequestModel()
                {
                    IssuerName = requestFacilityDetailForPromissory.IssuerName,
                    IssuerIdCode = requestFacilityDetailForPromissory.IssuerIdCode,
                    IssuerMobile = requestFacilityDetailForPromissory.IssuerMobile,
                    IssuerAddress = requestFacilityDetailForPromissory.IssuerAddress!,
                    IssuerPostalCode = requestFacilityDetailForPromissory.IssuerPostalCode!,
                    Description = $"شماره تسهیلات : {requestFacilityDetailForPromissory.FacilityNumber} / مبلغ تسهیلات : {requestFacilityDetailForPromissory.Amount:N0} ريال / برج الهیه شعبه 7000",
                    IssuerIban = requestFacilityDetailForPromissory.IssuerIban,
                    BirthDate = requestFacilityDetailForPromissory.IssuerBirthDate.GregorianToShamsi(),
                    Amount = Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(requestFacilityDetailForPromissory.Amount,
                                                                                                            requestFacilityDetailForPromissory.MonthCount,
                                                                                                            requestFacilityDetailForPromissory.FacilityInterest,
                                                                                                            requestFacilityDetailForPromissory.WarantyPercentage))).ToString()
                };
                var promissoryPublishRequestResult = await finnotechService.PromissoryPublishRequest(promissoryRequest, trackId, userId, cancellationToken);

                if (promissoryPublishRequestResult.IsSuccess && promissoryPublishRequestResult.Result != null)
                {
                    await requestFacilityPromissoryRepository.AddAsync(new RequestFacilityPromissory()
                    {
                        RequestFacilityId = requestFacilityId,
                        IsActive = true,
                        IssuerIdType = promissoryRequest.IssuerIdType,
                        IssuerMobile = promissoryRequest.IssuerMobile,
                        IssuerName = promissoryRequest.IssuerName,
                        IssuerIdCode = promissoryRequest.IssuerIdCode,
                        IssuerIban = promissoryRequest.IssuerIban,
                        IssuerAddress = promissoryRequest.IssuerAddress,
                        IssuerPostalCode = promissoryRequest.IssuerPostalCode,
                        BirthDate = promissoryRequest.BirthDate,
                        PaymentPlace = promissoryRequest.PaymentPlace,
                        ReceiverBirthDate = promissoryRequest.ReceiverBirthDate,
                        ReceiverIdCode = promissoryRequest.ReceiverIdCode,
                        ReceiverIdType = promissoryRequest.ReceiverIdType,
                        ReceiverMobile = promissoryRequest.ReceiverMobile,
                        ReceiverName = promissoryRequest.ReceiverName,
                        Transferable = promissoryRequest.Transferable,
                        TrackId = trackId,
                        UnSignedPdf = promissoryPublishRequestResult.Result.UnSignedPdf,
                        PromissoryId = promissoryPublishRequestResult.Result.PromissoryId,
                        RequestId = promissoryPublishRequestResult.Result.RequestId,
                        Description = promissoryRequest.Description,
                        Amount = Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(requestFacilityDetailForPromissory.Amount,
                                                                                                            requestFacilityDetailForPromissory.MonthCount,
                                                                                                            requestFacilityDetailForPromissory.FacilityInterest,
                                                                                                            requestFacilityDetailForPromissory.WarantyPercentage))),
                    }, cancellationToken);

                    return (await getRequestFacilityPromissory(userId, requestFacilityId, cancellationToken))!;
                }

                if (promissoryPublishRequestResult.Error != null)
                    throw new AppException($"{promissoryPublishRequestResult.Error.Code}-{promissoryPublishRequestResult.Error.Message}");
                else
                    throw new AppException($"خطا در صدور سفته");
            }

            return requestFacilityPromissory!;
        }

        public async Task SendSignRequest(int requestFacilityId, Guid userId, CancellationToken cancellationToken)
        {
            var requestFacilityPromissory = await GetRequestFacilityPromissory(userId, requestFacilityId, cancellationToken);
            if (requestFacilityPromissory == null)
                throw new AppException("اطلاعات سفته یافت نشد!قبل از ارسال درخواست امضای سفته باید درخواست صدور سفته فراخوانی شود");

            var promissorySignRequest = new PromissorySignRequestModel()
            {
                IdCode = requestFacilityPromissory.IssuerIdCode,
                RegistrationId = requestFacilityPromissory.TrackId,
                CallbackUrl = $"{siteSettings.Value.SigningSettings.AyandehSignCallback}/RequestFacilityPromissory/AyandehSignPostCallbackForPromissory/{requestFacilityId}"
            };
            var result = await finnotechService.PromissorySignRequest(promissorySignRequest, userId, cancellationToken);
            if (result.IsSuccess)
            {
                await requestFacilityPromissoryRepository.UpdateCustomPropertiesAsync(new RequestFacilityPromissory()
                {
                    Id = requestFacilityPromissory.Id,
                    Application = promissorySignRequest.Application,
                    CallbackUrl = promissorySignRequest.CallbackUrl,
                    LetSignerDownload = promissorySignRequest.LetSignerDownload,
                    State = promissorySignRequest.State,
                    SigningTrackId = result.Result!.SigningTrackId,
                },
                cancellationToken,
                saveNow: true,
                nameof(RequestFacilityPromissory.Application),
                nameof(RequestFacilityPromissory.CallbackUrl),
                nameof(RequestFacilityPromissory.LetSignerDownload),
                nameof(RequestFacilityPromissory.State),
                nameof(RequestFacilityPromissory.SigningTrackId));

                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                                                                                             WorkFlowFormEnum.SignPromissoryByUser,
                                                                                             StatusEnum.Approved,
                                                                                             opratorId: userId,
                                                                                             "ارسال درخواست امضای سفته به اپلیکیشن آینده ساین",
                                                                                             cancellationToken);

                return;
            }

            throw new AppException($"{result.Error!.Message}-{result.Error.Code}");

        }

        public async Task PromissoryFinalize(int requestFacilityId, Guid userId, CancellationToken cancellationToken)
        {
            var requestFacilityPromissory = await getRequestFacilityPromissory(userId, requestFacilityId, cancellationToken);
            if (requestFacilityPromissory == null)
                throw new AppException("اطلاعات سفته یافت نشد!");

            if (string.IsNullOrEmpty(requestFacilityPromissory.SigningTrackId))
                throw new AppException("قبل از نهایی کردن سفته باید اقدام به امضای آن نمایید در صورتی که نوتیفیکیشن امضای سفته را در اپلیکیشن آینده ساین دریافت نکرده اید رو دکمه 'ارسال مجدد درخواست امضای سفته' کلیک کنید!");

            var result = await finnotechService.PromissoryFinalize(requestFacilityPromissory.TrackId, userId, cancellationToken);
            if (result.IsSuccess)
            {
                await requestFacilityPromissoryRepository.UpdateCustomPropertiesAsync(new RequestFacilityPromissory()
                {
                    Id = requestFacilityPromissory.Id,
                    MultiSignedPdf = result.Result!.MultiSignedPdf,
                },
                cancellationToken,
                saveNow: true,
                nameof(RequestFacilityPromissory.MultiSignedPdf));

                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                                                                                             WorkFlowFormEnum.WaitingSignPromissoryByUser,
                                                                                             StatusEnum.Approved,
                                                                                             opratorId: userId,
                                                                                             "نهایی کردن سفته بعد از امضای سفته توسط کابر",
                                                                                             cancellationToken);

                return;
            }

            throw new AppException($"{result.Error!.Message}-{result.Error.Code}");

        }
    }
}
