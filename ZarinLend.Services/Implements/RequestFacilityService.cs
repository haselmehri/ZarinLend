using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Core.Entities.Business.Transaction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Common.LocalizationResource;
using ZarinLend.Services.Model;
using static Stimulsoft.Report.StiRecentConnections;

namespace Services
{
    public class RequestFacilityService : IRequestFacilityService, IScopedDependency
    {
        private readonly ILogger<RequestFacilityService> logger;
        private readonly IRequestFacilityRepository requestFacilityRepository;
        private readonly IBaseRepository<FacilityType> facilityTypeRepository;
        private readonly IBaseRepository<Organization> organizattionRepository;
        private readonly IWorkFlowStepRepository workFlowStepRepository;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly ISamatService samatService;
        private readonly IBaseRepository<UsagePlace> usagePlaceRepository;
        private readonly IBaseRepository<ApplicantValidationResult> applicantValidationResultRepository;
        private readonly IUserRepository userRepository;
        private readonly IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmentRepository;
        private readonly IFinnotechService finnotechService;
        private readonly IBaseRepository<RequestFacilityError> requestFacilityErrorRepository;
        private readonly IGlobalSettingService globalSettingService;
        private readonly IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository;
        private readonly IBaseRepository<IranCreditScoring> iranCreditScoringRepository;
        private readonly IWalletTransactionRepository walletTransactionRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IBaseRepository<RequestFacilityWorkFlowStepWorkFlowStepRejectionReason> requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository;
        private readonly IBaseRepository<WorkFlowStepRejectionReason> workFlowStepRejectionReasonRepository;

        public RequestFacilityService(ILogger<RequestFacilityService> logger,
            IRequestFacilityRepository requestFacilityRepository,
            IWorkFlowStepRepository workFlowStepRepository,
            IBaseRepository<FacilityType> facilityTypeRepository,
            IBaseRepository<Organization> organizattionRepository,
            IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
            ISamatService samatService,
            IBaseRepository<UsagePlace> usagePlaceRepository,
            IBaseRepository<ApplicantValidationResult> applicantValidationResultRepository,
            IUserRepository userRepository,
            IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmentRepository,
            IFinnotechService finnotechService,
            IBaseRepository<RequestFacilityError> requestFacilityErrorRepository,
            IGlobalSettingService globalSettingService,
            IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository,
            IBaseRepository<IranCreditScoring> iranCreditScoringRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IWebHostEnvironment webHostEnvironment,
            IBaseRepository<RequestFacilityWorkFlowStepWorkFlowStepRejectionReason> requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository,
            IBaseRepository<WorkFlowStepRejectionReason> workFlowStepRejectionReasonRepository)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            this.logger = logger;
            this.requestFacilityRepository = requestFacilityRepository;
            this.facilityTypeRepository = facilityTypeRepository;
            this.organizattionRepository = organizattionRepository;
            this.workFlowStepRepository = workFlowStepRepository;
            this.samatService = samatService;
            this.usagePlaceRepository = usagePlaceRepository;
            this.applicantValidationResultRepository = applicantValidationResultRepository;
            this.userRepository = userRepository;
            this.requestFacilityInstallmentRepository = requestFacilityInstallmentRepository;
            this.finnotechService = finnotechService;
            this.requestFacilityErrorRepository = requestFacilityErrorRepository;
            this.globalSettingService = globalSettingService;
            this.iranCreditScoringResultRuleRepository = iranCreditScoringResultRuleRepository;
            this.iranCreditScoringRepository = iranCreditScoringRepository;
            this.walletTransactionRepository = walletTransactionRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository = requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository;
            this.workFlowStepRejectionReasonRepository = workFlowStepRejectionReasonRepository;
        }

        #region Apis for External usings
        public virtual async Task<bool> MoveToVerifyStepFromUploadDocuments(int requestFacilityId, Guid userId, CancellationToken cancellationToken)
        {
            var requestIsInSpecialStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService, cancellationToken);
            if (requestIsInSpecialStep)
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService,
                     StatusEnum.Approved, userId, userId, "تایید اتوماتیک این مرحله-پرداخت خارج از سیستم", cancellationToken: cancellationToken);
                requestIsInSpecialStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.Buyer },
                    WorkFlowFormEnum.VerifyShahkarAndSamatService, cancellationToken);
                if (requestIsInSpecialStep)
                {
                    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.VerifyShahkarAndSamatService,
                        StatusEnum.Approved, userId, userId, null, cancellationToken: cancellationToken);

                    return true;
                }
                else
                {
                    logger.LogError($"درخواست فوق درمرحله اعتبارسنجی سمات قرار ندارد. RequestFacilityId : {requestFacilityId}");
                    throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
                }
            }
            else
            {
                logger.LogError($"درخواست فوق درمرحله پرداخت کارمزد اعتبارسنجی قرار ندارد. RequestFacilityId : {requestFacilityId}");
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
            }
        }

        #endregion Apis for External usings

        public async Task<bool?> GetValidationMustBeRepeated(int id, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.TableNoTracking
                                                  .Where(p => p.Id == id)
                                                  .Select(p => p.ValidationMustBeRepeated)
                                                  .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task AssignRequestFacilityToUser(AssignRequestFacilityToUserModel model, CancellationToken cancellationToken)
        {
            Assert.NotEmpty(model.RequestFacilityIds, nameof(model.RequestFacilityIds));
            foreach (int requestFacilityId in model.RequestFacilityIds)
            {
                await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                {
                    Id = requestFacilityId,
                    OperatorId = model.OperatorId,
                }, cancellationToken,
                false,
                nameof(RequestFacility.OperatorId));
                logger.LogInformation($"Assign RequestFacilty(ID : {requestFacilityId}) to User(ID : '{model.OperatorId}') by User(ID : '{model.CreatorId};)");
            }

            await requestFacilityRepository.SaveChangesAsync(cancellationToken);
        }

        #region singning by bank admin
        public async Task<string> SaveSignedContractByBankAndMoveOnWorkFlow(int requestFacilityId, int leasingId, Guid opratorId, byte[] signContract, string digest, string certificate,
            string signature, CancellationToken cancellationToken = default)
        {
            #region Sign Contract & Save Contract file
            var contractFileName = Guid.NewGuid().ToString();
            string path = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{contractFileName}.pdf");
            ////System.IO.File.WriteAllBytes(path, pdf.Data);

            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await stream.WriteAsync(signContract, 0, signContract.Length);
            }

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacilityId,
                SignedContractByBankFileName = $"{contractFileName}.pdf",
                Digest = digest,
                Signature = signature,
                Certificate = certificate,
                SigningMethod = SigningMethod.HardwareToken,
            }, cancellationToken,
            false,
            nameof(RequestFacility.SignedContractByBankFileName),
            nameof(RequestFacility.Signature),
            nameof(RequestFacility.Digest),
            nameof(RequestFacility.SigningMethod),
            nameof(RequestFacility.Certificate)
            );
            #endregion Sign Contract & Save Contract file

            var status = await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, leasingId, new List<RoleEnum> { RoleEnum.AdminBankLeasing }, WorkFlowFormEnum.AdminBankLeasingSignature, cancellationToken);
            if (status)
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                  WorkFlowFormEnum.AdminBankLeasingSignature,
                  StatusEnum.Approved,
                  opratorId: opratorId,
                  "امضاء قرارداد توسط مدیر بانک",
                  cancellationToken,
                  autoSave: false);
            }
            await requestFacilityRepository.SaveChangesAsync(cancellationToken);

            return $"{contractFileName}.pdf";
        }

        public async Task<bool> ApprovedAdminLeasingWithoutSignContract(RequestStatusModel model, CancellationToken cancellationToken = default)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId, model.LeasingId.Value, new List<RoleEnum> { RoleEnum.AdminBankLeasing },
                WorkFlowFormEnum.AdminBankLeasingSignature, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
                  WorkFlowFormEnum.AdminBankLeasingSignature,
                  StatusEnum.Approved,
                  opratorId: model.OpratorId,
                  statusDescription: !string.IsNullOrEmpty(model.StatusDescription) ? model.StatusDescription : "تایید قرارداد بدون امضاء قرارداد توسط مدیر بانک",
                  cancellationToken);

                return true;
            }

            throw new AppException("تسهیلات فوق در مرحله تایید مدیر و امضاء قرارداد توسط مدیر نهاد مالی قرار ندارد!");
        }

        public async Task<bool> ReturnToCorrection(RequestStatusModel model, CancellationToken cancellationToken = default)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId, model.LeasingId.Value, new List<RoleEnum> { RoleEnum.AdminBankLeasing },
                WorkFlowFormEnum.AdminBankLeasingSignature, cancellationToken))
            {
                var statusDescription = model.Status == StatusEnum.Approved
                    ? !string.IsNullOrEmpty(model.StatusDescription) ? model.StatusDescription : "تایید قرارداد بدون امضاء قرارداد توسط مدیر بانک"
                    : model.StatusDescription;

                var currentStep = await requestFacilityWorkFlowStepRepository
                            .GetByConditionAsync(p => p.RequestFacilityId == model.RequestFacilityId
                            && !p.StatusId.HasValue
                            && p.WorkFlowStep.WorkFlowFormId.Equals(WorkFlowFormEnum.AdminBankLeasingSignature),
                            cancellationToken);

                foreach (var id in model.WorkFlowStepRejectionReasonIds)
                {
                    requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository.Add(new()
                    {
                        RequestFacilityWorkFlowStepId = currentStep.Id,
                        WorkFlowStepRejectionReasonId = id
                    }, false);
                }

                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
                  WorkFlowFormEnum.AdminBankLeasingSignature,
                  StatusEnum.ReturnToCorrection,
                  opratorId: model.OpratorId,
                  statusDescription: statusDescription + " - " + model.RejectionReasonDescription,
                  cancellationToken);

                return true;
            }

            throw new AppException("تسهیلات فوق در مرحله تایید مدیر و امضاء قرارداد توسط مدیر نهاد مالی قرار ندارد!");
        }

        public async Task SaveSignedContractByAdminAndMoveOnWorkFlow(int requestFacilityId, int leasingId, byte[] signContract, CancellationToken cancellationToken = default)
        {
            #region Sign Contract & Save Contract file
            var contractFileName = Guid.NewGuid().ToString();
            string path = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{contractFileName}.pdf");
            ////System.IO.File.WriteAllBytes(path, pdf.Data);

            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await stream.WriteAsync(signContract, 0, signContract.Length);
            }

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacilityId,
                SigningMethod = SigningMethod.MobileApp,
                SignedContractByBankFileName = $"{contractFileName}.pdf"
            },
            cancellationToken,
            saveNow: false,
            nameof(RequestFacility.SignedContractByBankFileName),
            nameof(RequestFacility.SigningMethod));
            #endregion Sign Contract & Save Contract file

            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
                                                                       leasingId,
                                                                       new List<RoleEnum> { RoleEnum.AdminBankLeasing },
                                                                       WorkFlowFormEnum.AdminBankLeasingSignature,
                                                                       cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                  WorkFlowFormEnum.AdminBankLeasingSignature,
                  StatusEnum.Approved,
                  opratorId: await userRepository.TableNoTracking.Where(p => p.UserName == "system_admin").Select(p => p.Id).SingleAsync(),
                  "تایید اتوماتیک بعد از بعد از امضای قرارداد توسط مدیر بانک با اپلیکیشن آینده ساین",
                  cancellationToken,
                  autoSave: false);
            }

            await requestFacilityErrorRepository.SaveChangesAsync(cancellationToken);
        }

        #endregion singning by bank admin

        public async Task SaveSignedContractAndMoveOnWorkFlow(int requestFacilityId, byte[] signContract, CancellationToken cancellationToken = default)
        {
            #region Sign Contract & Save Contract file
            var contractFileName = Guid.NewGuid().ToString();
            string path = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{contractFileName}.pdf");
            ////System.IO.File.WriteAllBytes(path, pdf.Data);

            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await stream.WriteAsync(signContract, 0, signContract.Length);
            }

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacilityId,
                SignedContractByUserFileName = $"{contractFileName}.pdf"
            }, cancellationToken, false, nameof(RequestFacility.SignedContractByUserFileName));
            #endregion Sign Contract & Save Contract file

            if (await userRepository.TableNoTracking.Where(p => p.UserName == "system_admin")
              .Select(p => p.Id).AnyAsync())
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                  WorkFlowFormEnum.WaitingToSignContractByUser,
                  StatusEnum.Approved,
                  opratorId: (await userRepository.TableNoTracking.Where(p => p.UserName == "system_admin")
                  .Select(p => p.Id).SingleAsync()),
                  "تایید اتوماتیک بعد از بعد از امضای قرارداد",
                  cancellationToken);
            }
        }
        public async Task TestSaveSignedContractAndMoveOnWorkFlow(int requestFacilityId, CancellationToken cancellationToken = default)
        {
            #region Sign Contract & Save Contract file
            var contractFileName = await requestFacilityRepository
                .GetColumnValueAsync<string>(p => p.Id == requestFacilityId, cancellationToken, nameof(RequestFacility.ContractFileName));
            //string path = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{contractFileName}.pdf");

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacilityId,
                SignedContractByUserFileName = $"{contractFileName}"
            }, cancellationToken, false, nameof(RequestFacility.SignedContractByUserFileName));
            #endregion Sign Contract & Save Contract file

            if (await userRepository.TableNoTracking.Where(p => p.UserName == "system_admin")
              .Select(p => p.Id).AnyAsync())
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                  WorkFlowFormEnum.WaitingToSignContractByUser,
                  StatusEnum.Approved,
                  opratorId: (await userRepository.TableNoTracking.Where(p => p.UserName == "system_admin")
                  .Select(p => p.Id).SingleAsync()),
                  "تایید اتوماتیک بعد از بعد از امضای قرارداد",
                  cancellationToken);
            }

        }
        public async Task<string> GetSignedContractByUserFileName(int id, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.GetColumnValueAsync<string>(p => p.Id == id, cancellationToken, nameof(RequestFacility.SignedContractByUserFileName));
        }
        public async Task<string> GetAyandehSigningToken(int id, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.GetColumnValueAsync<string>(p => p.Id == id, cancellationToken, nameof(RequestFacility.AyandehSignSigningToken));
        }

        public async Task<string> GetAyandehSignSigningTokenForAdminBank(int id, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.GetColumnValueAsync<string>(p => p.Id == id, cancellationToken, nameof(RequestFacility.AyandehSignSigningTokenForAdminBank));
        }
        public async Task<bool> UserHasOpenRequest(Guid userId, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.TableNoTracking
                        .AnyAsync(p => p.BuyerId.Equals(userId) &&
                                       !p.CancelByUser &&
                                       p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue),
                                       cancellationToken);
        }

        public async Task<FindOpenRequestFacilityResultModel?> FindOpenRequestFacility(string nationalCode, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.TableNoTracking
                        .Where(p => p.Buyer.Person.NationalCode.Equals(nationalCode) &&
                                   !p.CancelByUser &&
                                    p.GuarantorIsRequired &&
                                    !p.RequestFacilityGuarantors.Any(p => !p.CancelByUser &&
                                                                         !p.RequestFacilityGuarantorWorkFlowSteps.Any(x => !x.StatusId.HasValue)) &&
                                    p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                                                            !x.StatusId.HasValue &&
                                                                            (x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.VerifyZarrinLend ||
                                                                            x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.RegisterIdentityInfo ||
                                                                            x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.UploadIdentityDocuments)))
                        .Select(p => new FindOpenRequestFacilityResultModel
                        {
                            Id = p.Id,
                            BuyerId = p.BuyerId,
                            RequesterId = p.BuyerId,
                            FName = p.Buyer.Person.FName,
                            LName = p.Buyer.Person.LName,
                            FatherName = p.Buyer.Person.FatherName,
                            NationalCode = p.Buyer.Person.NationalCode,
                            Amount = p.Amount,
                            MonthCount = p.FacilityType.MonthCount,
                            MonthCountTitle = p.FacilityType.MonthCountTitle,
                            FinancialInstitutionFacilityFee = p.GlobalSetting.FinancialInstitutionFacilityFee,
                            LendTechFacilityFee = p.GlobalSetting.LendTechFacilityFee,
                            WarantyPercentage = p.GlobalSetting.WarantyPercentage,
                            FacilityInterest = p.GlobalSetting.FacilityInterest,
                            ValidationFee = p.GlobalSetting.ValidationFee,
                            CreateDate = p.CreatedDate
                        })
                        .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsOpenRequestFacility(int requestFacilityId, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.TableNoTracking
                        .AnyAsync(p => p.Id.Equals(requestFacilityId) &&
                                      !p.CancelByUser &&
                                       p.GuarantorIsRequired &&
                                      !p.RequestFacilityGuarantors.Any(p => !p.CancelByUser &&
                                                                            !p.RequestFacilityGuarantorWorkFlowSteps.Any(x => !x.StatusId.HasValue)) &&
                                       p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                                                              !x.StatusId.HasValue &&
                                                                              (x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.VerifyZarrinLend ||
                                                                               x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.RegisterIdentityInfo ||
                                                                               x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.UploadIdentityDocuments)), cancellationToken);
        }

        public async Task<long> GetUserSumAmountApprovedFacilities(Guid userId, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.TableNoTracking
                        .Where(p => p.BuyerId.Equals(userId) &&
                                       !p.CancelByUser &&
                                       !p.DoneFacility &&
                                       p.RequestFacilityWorkFlowSteps
                                            .Any(x => x.WorkFlowStep.IsApproveFinalStep &&
                                                x.WorkFlowStep.IsLastStep &&
                                                x.StatusId.HasValue &&
                                                x.StatusId.Value == (short)StatusEnum.Approved))
                        .SumAsync(p => p.Amount, cancellationToken);
        }
        public async Task<bool> VerifyCheckByZarinLend(RequestStatusModel statusModel, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin },
                 WorkFlowFormEnum.VerifyCheckByZarinLend, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.VerifyCheckByZarinLend,
                statusModel.Status, opratorId: statusModel.OpratorId, statusModel.StatusDescription, cancellationToken);

                return true;
            }
            return false;
        }

        public async Task<bool> SetGuarantorIsRequired(RequestGuarantorIsRequiredModel statusModel, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin },
                 WorkFlowFormEnum.VerifyZarrinLend, cancellationToken))
            {
                await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                {
                    Id = statusModel.RequestFacilityId,
                    GuarantorIsRequired = statusModel.GuarantorIsRequired
                }, cancellationToken, true, nameof(RequestFacility.GuarantorIsRequired), nameof(RequestFacility.UpdateDate));

                return true;
            }
            return false;
        }
        public async Task<bool> CheckGuarantorIsRequired(int requestFacilityId, CancellationToken cancellationToken)
        {
            return await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.Id == requestFacilityId && p.GuarantorIsRequired, cancellationToken);
        }

        public async Task UpdateAwaitingIntroductionGuarantor(int requestFacilityId, bool awaitingIntroductionGuarantor, bool saveNow, CancellationToken cancellationToken)
        {
            if (!await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.Id == requestFacilityId, cancellationToken))
                throw new AppException(ApiResultStatusCode.NotFound, $"تسهیلات مورد نظر با شناسه({requestFacilityId}) فوق یافت نشد!");

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacilityId,
                AwaitingIntroductionGuarantor = awaitingIntroductionGuarantor
            }, cancellationToken, saveNow: saveNow,
            nameof(RequestFacility.AwaitingIntroductionGuarantor));
        }

        public async Task<bool> VerifyByZarinLend(VerifyByZarinLendRequestStatusModel statusModel, CancellationToken cancellationToken)
        {
            var requestFacility = await requestFacilityRepository.TableNoTracking
                .Include(p => p.Buyer)
                .ThenInclude(t => t.Person)
                .FirstOrDefaultAsync(f => f.Id == statusModel.RequestFacilityId);

            if (string.IsNullOrEmpty(requestFacility.Buyer.Person.FName)
            || string.IsNullOrEmpty(requestFacility.Buyer.Person.LName)
            || string.IsNullOrEmpty(requestFacility.Buyer.Person.FatherName)
            || requestFacility.Buyer.Person.Gender == null
            || string.IsNullOrEmpty(requestFacility.Buyer.Person.SSID))
                throw new AppException(ApiResultStatusCode.BadRequest, ResourceFile.CivilRegistryNotDone, System.Net.HttpStatusCode.BadRequest);

            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin },
                 WorkFlowFormEnum.VerifyZarrinLend, cancellationToken))
            {
                if (statusModel.Status == StatusEnum.Approved)
                {
                    await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                    {
                        Id = statusModel.RequestFacilityId,
                        OrganizationId = statusModel.LeasingId
                    }, cancellationToken, false, nameof(RequestFacility.OrganizationId), nameof(RequestFacility.UpdateDate));
                }
                else if (statusModel.Status == StatusEnum.ReturnToCorrection || statusModel.Status == StatusEnum.Rejected)
                {
                    if (statusModel.ValidationMustBeRepeated.HasValue)
                    {
                        await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                        {
                            Id = statusModel.RequestFacilityId,
                            ValidationMustBeRepeated = statusModel.ValidationMustBeRepeated
                        }, cancellationToken, false, nameof(RequestFacility.ValidationMustBeRepeated), nameof(RequestFacility.UpdateDate));
                    }
                    if (statusModel.ErrorMessageIds != null && statusModel.ErrorMessageIds.Any())
                    {
                        foreach (var workFlowStepErrorId in statusModel.ErrorMessageIds)
                        {
                            await requestFacilityErrorRepository.AddAsync(new RequestFacilityError()
                            {
                                RequestFacilityId = statusModel.RequestFacilityId,
                                WorkFlowStepErrorId = workFlowStepErrorId,
                                IsDone = false,
                            }, cancellationToken, false);
                        }
                    }
                }

                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.VerifyZarrinLend,
                statusModel.Status, opratorId: statusModel.OpratorId, statusModel.StatusDescription, cancellationToken);

                return true;
            }
            return false;
        }
        public async Task<bool> VerifyCheckByLeasing(RequestStatusModel statusModel, int leasingId, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, leasingId, new List<RoleEnum>() { RoleEnum.BankLeasing },
                 WorkFlowFormEnum.VerifyCheckByLeasing, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.VerifyCheckByLeasing,
                    statusModel.Status, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken);

                return true;
            }
            return false;
        }
        public async Task DepositFacilityAmount([NotNull] RequestFacilityDepositDocumentModel model, IFormFile? file, int leasingId, CancellationToken cancellationToken = default)
        {
            if (model == null)
                logger.LogError("'model' is null");

            var isInPendingDepositFacilityAmountStep = false;
            var isInDepositFacilityAmountStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(model!.RequestFacilityId,
                                                                                                    leasingId,
                                                                                                    new List<RoleEnum>() { RoleEnum.BankLeasing, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing },
                                                                                                    WorkFlowFormEnum.DepositFacilityAmount,
                                                                                                    cancellationToken);
            if (!isInDepositFacilityAmountStep)
                isInPendingDepositFacilityAmountStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId,
                                                                                                             leasingId,
                                                                                                             new List<RoleEnum>() { RoleEnum.BankLeasing, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing },
                                                                                                             WorkFlowFormEnum.PendingDepositFacilityAmount,
                                                                                                             cancellationToken);

            if (!isInDepositFacilityAmountStep && !isInPendingDepositFacilityAmountStep)
                throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'واریز تسهیلات به حساب زرین لند' نمی باشد");

            if (!string.IsNullOrEmpty(model!.DepositDocumentNumber) && await requestFacilityRepository.TableNoTracking
                .AnyAsync(p => p.DepositDocumentNumber != null && p.DepositDocumentNumber.Equals(model.DepositDocumentNumber.Trim())))
                throw new AppException("شماره سند وارد شده تکراری می باشد!");

            #region Save File On Disk & Insert Record In Database
            string? fileName = null;
            if (file != null && file.Length > 0)
            {
                var depositDocument = @"UploadFiles\DepositDocument";
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, depositDocument);
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var relativePath = $"/UploadFiles/DepositDocument/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            var requestFacility = new RequestFacility()
            {
                Id = model.RequestFacilityId,
                DepositDocumentNumber = model.DepositDocumentNumber,
                DepositDate = model.DepositDate,
                DepositDocumentFileName = fileName
            };
            await requestFacilityRepository.UpdateCustomPropertiesAsync(requestFacility, cancellationToken, false,
                nameof(RequestFacility.DepositDocumentFileName),
                nameof(RequestFacility.DepositDate),
                nameof(RequestFacility.DepositDocumentNumber));

            #endregion

            if (isInDepositFacilityAmountStep && model.Status == StatusEnum.ReturnToCorrection)
            {
                var currentStep = await requestFacilityWorkFlowStepRepository
                    .GetByConditionAsync(p => p.RequestFacilityId == model.RequestFacilityId
                    && !p.StatusId.HasValue
                    && p.WorkFlowStep.WorkFlowFormId.Equals(WorkFlowFormEnum.DepositFacilityAmount),
                    cancellationToken);

                foreach (var id in model.WorkFlowStepRejectionReasonIds)
                {
                    requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository.Add(new()
                    {
                        RequestFacilityWorkFlowStepId = currentStep.Id,
                        WorkFlowStepRejectionReasonId = id
                    }, false);
                }
            }
            else if (!isInDepositFacilityAmountStep && model.Status == StatusEnum.ReturnToCorrection)
            {
                var currentStep = await requestFacilityWorkFlowStepRepository
                    .GetByConditionAsync(p => p.RequestFacilityId == model.RequestFacilityId
                    && !p.StatusId.HasValue
                    && p.WorkFlowStep.WorkFlowFormId.Equals(WorkFlowFormEnum.PendingDepositFacilityAmount),
                    cancellationToken);

                foreach (var id in model.WorkFlowStepRejectionReasonIds)
                {
                    requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository.Add(new()
                    {
                        RequestFacilityWorkFlowStepId = currentStep.Id,
                        WorkFlowStepRejectionReasonId = id
                    }, false);
                }
            }

            if (isInDepositFacilityAmountStep)
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
                                                                                             WorkFlowFormEnum.DepositFacilityAmount,
                                                                                             model.Status,
                                                                                             model.CreatorId,
                                                                                             model.Status == StatusEnum.Approved
                                                                                             ? $"واریز مبلغ تسهیلات به حساب زرین لند - {model.StepDescription}"
                                                                                             : model.StepDescription + " - " + model.RejectionReasonDescription,
                                                                                             cancellationToken, true);

                if (model.Status == StatusEnum.ReturnToCorrection) return;
            }

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
                                                                                         WorkFlowFormEnum.PendingDepositFacilityAmount,
                                                                                         model.Status,
                                                                                         model.CreatorId,
                                                                                         $"واریز مبلغ تسهیلات به حساب زرین لند - {model.StepDescription + " - " + model.RejectionReasonDescription}",
                                                                                         cancellationToken, true);

            var requestFacilityInfo = (await requestFacilityRepository.SelectByAsync(p => p.Id == model!.RequestFacilityId,
            p => new
            {
                //p.RequestFacilityCardIssuance.CardNumber,
                p.Amount,
                p.FacilityType.MonthCount,
                p.GlobalSetting.FacilityInterest,
                //p.BuyerId,
                //p.UserOption
            }, cancellationToken))
            .FirstOrDefault()!;

            var amountInstallment = InstallmentCalculator.CalculateAmountInstallment(requestFacilityInfo.Amount, requestFacilityInfo.MonthCount, requestFacilityInfo.FacilityInterest);
            var installmetList = new List<RequestFacilityInstallment>();
            var currentDate = DateTime.Now.Date;
            for (int i = 1; i <= requestFacilityInfo.MonthCount; i++)
            {
                installmetList.Add(new RequestFacilityInstallment()
                {
                    RequestFacilityId = model!.RequestFacilityId,
                    DueDate = currentDate.AddMonthsBaseShamsi(i),
                    Amount = (long)amountInstallment
                });
            }
            await requestFacilityInstallmentRepository.AddRangeAsync(installmetList, cancellationToken);
        }
        public async Task CardRecharge(int requestFacilityId, Guid creatorId, int leasingId, CancellationToken cancellationToken = default)
        {
            var isInChargeCardStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
                                                                                       leasingId,
                                                                                       new List<RoleEnum>() { RoleEnum.BankLeasing, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing },
                                                                                       WorkFlowFormEnum.CardRecharge,
                                                                                       cancellationToken);
            if (!isInChargeCardStep)
                throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'شارژ کارت' نمی باشد");

            var requestFacilityInfo = (await requestFacilityRepository.SelectByAsync(p => p.Id == requestFacilityId,
                p => new
                {
                    p.RequestFacilityCardIssuance.CardNumber,
                    p.Amount,
                    p.FacilityType.MonthCount,
                    p.GlobalSetting.FacilityInterest,
                    p.BuyerId,
                    p.UserOption
                }, cancellationToken))
                .FirstOrDefault();

            if (requestFacilityInfo == null)
                throw new AppException(ApiResultStatusCode.BadRequest);

            //var result = await finnotechService.ChargeCard(requestFacilityId, requestFacilityInfo.CardNumber, requestFacilityInfo.Amount, creatorId, cancellationToken);

            //if (result != null)
            //{
            //    if (result.Item1)
            //    {
            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId,
                                                                                         WorkFlowFormEnum.CardRecharge,
                                                                                         StatusEnum.Approved,
                                                                                         creatorId,
                                                                                         $"شارژ بن کارت مشتری",
                                                                                         cancellationToken, true);

            if (requestFacilityInfo.UserOption == UserOption.PurchaseFromContractedStores)
            {
                await walletTransactionRepository.AddAsync(new WalletTransaction()
                {
                    Amount = requestFacilityInfo.Amount,
                    CreatorId = creatorId,
                    RequestFacilityId = requestFacilityId,
                    WalletTransactionType = WalletTransactionTypeEnum.Deposit,
                    UserId = requestFacilityInfo.BuyerId
                }, cancellationToken, saveNow: false);
            }
            //fill RequestFacilityInstallment entity base detail of RequestFacility

            var amountInstallment = InstallmentCalculator.CalculateAmountInstallment(requestFacilityInfo.Amount, requestFacilityInfo.MonthCount, requestFacilityInfo.FacilityInterest);
            var installmetList = new List<RequestFacilityInstallment>();
            var currentDate = DateTime.Now.Date;
            for (int i = 1; i <= requestFacilityInfo.MonthCount; i++)
            {
                installmetList.Add(new RequestFacilityInstallment()
                {
                    RequestFacilityId = requestFacilityId,
                    DueDate = currentDate.AddMonthsBaseShamsi(i),
                    Amount = (long)amountInstallment
                });
            }
            await requestFacilityInstallmentRepository.AddRangeAsync(installmetList, cancellationToken);
            //}
            //else
            //{
            //    throw new AppException(!string.IsNullOrEmpty(result.Item2) ? result.Item2 : "خطایی نامشخص رخ داده است!");
            //}
            //}
        }
        public async Task<bool> GoToNextStepFromVerifyToPendingResultVerify(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, leasingId, new List<RoleEnum> { RoleEnum.BankLeasing },
                        WorkFlowFormEnum.VerifyLeasing, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.VerifyLeasing,
                     StatusEnum.Approved, userId, "تغییر وضعیت به مرحله در انتظار نتیجه استعلام", cancellationToken: cancellationToken, autoSave);
                return true;
            }
            else
            {
                logger.LogError($"درخواست فوق در مرحله استعلام قرار ندارد. RequestFacilityId : {requestFacilityId}");
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
            }
        }
        public async Task<bool> GoToNextStepFromEnterFacilityToPendingEnterFacility(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, leasingId, new List<RoleEnum> { RoleEnum.BankLeasing },
                        WorkFlowFormEnum.EnterFacilityNumber, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.EnterFacilityNumber,
                     StatusEnum.Approved, userId, "تغییر وضعیت به مرحله در انتظار تنظیم شماره قرارداد", cancellationToken: cancellationToken, autoSave);
                return true;
            }
            else
            {
                logger.LogError($"درخواست فوق در مرحله تنظیم شماره قرارداد قرار ندارد. RequestFacilityId : {requestFacilityId}");
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
            }
        }
        public async Task<bool> GoToNextStepFromBankLeasingInquiryToPendingBankLeasingInquiry(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, leasingId, new List<RoleEnum> { RoleEnum.BankLeasing },
                        WorkFlowFormEnum.BankLeasingInquiry, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.BankLeasingInquiry,
                     StatusEnum.Approved, userId, "تغییر وضعیت به مرحله در انتظار تنظیم شماره انتظامی", cancellationToken: cancellationToken, autoSave);
                return true;
            }
            else
            {
                logger.LogError($"درخواست فوق در مرحله تنظیم شماره انتظامی قرار ندارد. RequestFacilityId : {requestFacilityId}");
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
            }
        }
        public async Task<bool> GoToNextStepFromDepositFacilityAmountToPendingResultDepositFacilityAmount(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, leasingId, new List<RoleEnum> { RoleEnum.BankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.AdminBankLeasing },
                        WorkFlowFormEnum.DepositFacilityAmount, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.DepositFacilityAmount,
                     StatusEnum.Approved, userId, "تغییر وضعیت به مرحله در انتظار واریز تسهیلات", cancellationToken: cancellationToken, autoSave);
                return true;
            }
            else
            {
                logger.LogError($"درخواست فوق در مرحله واریز تسهیلات قرار ندارد. RequestFacilityId : {requestFacilityId}");
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
            }
        }

        public async Task<bool> GoToNextStepFromChargeCardToPendingResultChargeCard(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, leasingId, new List<RoleEnum> { RoleEnum.BankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.AdminBankLeasing },
                        WorkFlowFormEnum.CardRecharge, cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.CardRecharge,
                     StatusEnum.Approved, userId, "تغییر وضعیت به مرحله در انتظار شارژ بن کارت", cancellationToken: cancellationToken, autoSave);
                return true;
            }
            else
            {
                logger.LogError($"درخواست فوق در مرحله شارژ بن کارت قرار ندارد. RequestFacilityId : {requestFacilityId}");
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
            }
        }


        public async Task<bool> VerifyByLeasing(PrimaryVerifyModel statusModel, ApplicantValidationResultModel applicantValidationResultModel, int leasingId, bool autoSave, CancellationToken cancellationToken)
        {
            var isInPendingVerifyLeasing = false;
            var isInVerifyLeasing = await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, leasingId, new List<RoleEnum>() { RoleEnum.BankLeasing },
                 WorkFlowFormEnum.VerifyLeasing, cancellationToken);
            isInPendingVerifyLeasing = await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, leasingId, new List<RoleEnum>() { RoleEnum.BankLeasing },
               WorkFlowFormEnum.PendingForVerifyResult, cancellationToken);
            if (isInVerifyLeasing || isInPendingVerifyLeasing)
            {
                await applicantValidationResultRepository.AddAsync(new ApplicantValidationResult()
                {
                    RequestFacilityId = applicantValidationResultModel.RequestFacilityId!.Value,
                    OrganizationId = leasingId,
                    CreatorId = applicantValidationResultModel.CreatorId,
                    BlackListInquiry = applicantValidationResultModel.BlackListInquiry,
                    CivilRegistryInquiry = applicantValidationResultModel.CivilRegistryInquiry,
                    FacilityInquiry = applicantValidationResultModel.FacilityInquiry,
                    MilitaryInquiry = applicantValidationResultModel.MilitaryInquiry,
                    PostalCodeInquiry = applicantValidationResultModel.PostalCodeInquiry,
                    ReturnedCheckInquiry = applicantValidationResultModel.ReturnedCheckInquiry,
                    SecurityCouncilSanctionsInquiry = applicantValidationResultModel.SecurityCouncilSanctionsInquiry,
                    ShahkarInquiry = applicantValidationResultModel.ShahkarInquiry,
                }, cancellationToken, saveNow: false);

                if (isInVerifyLeasing)
                {
                    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.VerifyLeasing,
                        statusModel.Status, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken, true);
                }
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.PendingForVerifyResult,
                    statusModel.Status, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken, autoSave);

                return true;
            }

            throw new AppException("درخواست فوق در مرحله استعلام قرار ندارد!");
        }
        public async Task<bool> EnterFacilityNumber(EnterFacilityNumberModel statusModel, int leasingId, bool autoSave, CancellationToken cancellationToken)
        {
            var isInPendingEnterFacilityNumberStep = false;
            var isInEnterFacilityNumberStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId,
                                                                                                    leasingId,
                                                                                                    new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing },
                                                                                                    WorkFlowFormEnum.EnterFacilityNumber,
                                                                                                    cancellationToken);
            if (!isInEnterFacilityNumberStep)
                isInPendingEnterFacilityNumberStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId,
                                                                                                           leasingId,
                                                                                                           new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing },
                                                                                                           WorkFlowFormEnum.PendingEnterFacilityNumber,
                                                                                                           cancellationToken);

            if (isInEnterFacilityNumberStep || isInPendingEnterFacilityNumberStep)
            {
                var a = await requestFacilityRepository.TableNoTracking.Where(p => p.Id == statusModel.RequestFacilityId).Select(p => p.UserOption).FirstOrDefaultAsync(cancellationToken);
                var a1 = requestFacilityRepository.TableNoTracking.Where(p => p.Id == statusModel.RequestFacilityId).Select(p => p.UserOption).FirstOrDefault();
                var a12 = requestFacilityRepository.TableNoTracking.Where(p => p.Id == statusModel.RequestFacilityId).Select(p => p.UserOption);

                var userHasSelected_PurchaseFromContractedStores = await requestFacilityRepository.TableNoTracking.Where(p => p.Id == statusModel.RequestFacilityId).Select(p => p.UserOption).FirstOrDefaultAsync(cancellationToken);
                if (statusModel.Status == StatusEnum.Approved &&
                    await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.FacilityNumber.Equals(statusModel.FacilityNumber.Trim())))
                    throw new AppException("شماره تسهیلات وارد شده تکراری می باشد");

                if (statusModel.Status == StatusEnum.Approved)
                {
                    await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                    {
                        Id = statusModel.RequestFacilityId,
                        FacilityNumber = statusModel.FacilityNumber
                    }, cancellationToken, false, nameof(RequestFacility.FacilityNumber), nameof(RequestFacility.UpdateDate));


                    if (isInEnterFacilityNumberStep)
                    {
                        await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.EnterFacilityNumber,
                            StatusEnum.Approved, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken, true);
                    }

                    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.PendingEnterFacilityNumber,
                        StatusEnum.Approved, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken, true);

                    //#region Automatic Approve 'InsuranceIssuance' Step
                    //await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.InsuranceIssuance,
                    //    StatusEnum.Approved, statusModel.OpratorId, "تایید اتوماتیک بعد از ثبت شماره قرارداد/تسهیلات", cancellationToken, autoSave);
                    //#endregion Automatic Approve 'InsuranceIssuance' Step

                    #region Automatic Approve 'InsuranceIssuance' Step
                    if (userHasSelected_PurchaseFromContractedStores == UserOption.PurchaseFromContractedStores)
                    {
                        var system_accountId = await userRepository.TableNoTracking.Where(p => p.UserName == "system_admin").Select(p => p.Id).FirstOrDefaultAsync(cancellationToken);
                        await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.PaySalesCommission,
                            StatusEnum.Approved, system_accountId, $"تایید اتوماتیک مرحله پرداخت هزینه عضویت در باشگاه مشتریان بدلیل اینکه کاربر گزینه '{userHasSelected_PurchaseFromContractedStores.ToDisplay()}' انتخاب کرده است", cancellationToken, autoSave);
                    }
                    #endregion Automatic Approve 'InsuranceIssuance' Step
                }
                else
                {
                    if (isInEnterFacilityNumberStep && statusModel.Status == StatusEnum.ReturnToCorrection)
                    {
                        var currentStep = await requestFacilityWorkFlowStepRepository
                            .GetByConditionAsync(p => p.RequestFacilityId == statusModel.RequestFacilityId
                            && !p.StatusId.HasValue
                            && p.WorkFlowStep.WorkFlowFormId.Equals(WorkFlowFormEnum.EnterFacilityNumber),
                            cancellationToken);

                        foreach (var id in statusModel.WorkFlowStepRejectionReasonIds)
                        {
                            requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository.Add(new()
                            {
                                RequestFacilityWorkFlowStepId = currentStep.Id,
                                WorkFlowStepRejectionReasonId = id
                            }, false);
                        }
                    }
                    else if (isInPendingEnterFacilityNumberStep && statusModel.Status == StatusEnum.ReturnToCorrection)
                    {
                        var currentStep = await requestFacilityWorkFlowStepRepository
                            .GetByConditionAsync(p => p.RequestFacilityId == statusModel.RequestFacilityId
                            && !p.StatusId.HasValue
                            && p.WorkFlowStep.WorkFlowFormId.Equals(WorkFlowFormEnum.PendingEnterFacilityNumber),
                            cancellationToken);

                        foreach (var id in statusModel.WorkFlowStepRejectionReasonIds)
                        {
                            requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository.Add(new()
                            {
                                RequestFacilityWorkFlowStepId = currentStep.Id,
                                WorkFlowStepRejectionReasonId = id
                            }, false);
                        }
                    }

                    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId,
                        isInEnterFacilityNumberStep ? WorkFlowFormEnum.EnterFacilityNumber : WorkFlowFormEnum.PendingEnterFacilityNumber,
                        statusModel.Status, 
                        statusModel.OpratorId, 
                        statusModel.StatusDescription + " - " + statusModel.RejectionReasonDescription,
                        cancellationToken, autoSave);
                }
                return true;
            }

            throw new AppException("درخواست فوق در مرحله ثبت شماره قرارداد/تسهیلات قرار ندارد!");
        }

        public async Task<bool> VerifyAndAllocationByLeasing(VerifyAndPoliceNumberModel statusModel, int leasingId, bool autoSave, CancellationToken cancellationToken)
        {
            var isInBankLeasingInquiryStep = false;
            var isInEnterFacilityNumberStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, leasingId, new List<RoleEnum>() { RoleEnum.BankLeasing },
                 WorkFlowFormEnum.BankLeasingInquiry, cancellationToken);
            if (!isInEnterFacilityNumberStep)
                isInBankLeasingInquiryStep = await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityId, leasingId, new List<RoleEnum>() { RoleEnum.BankLeasing },
                 WorkFlowFormEnum.PendingBankLeasingInquiry, cancellationToken);

            if (isInEnterFacilityNumberStep || isInBankLeasingInquiryStep)
            {
                if (statusModel.Status == StatusEnum.Approved &&
                    await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.PoliceNumber != null && p.PoliceNumber.Equals(statusModel.PoliceNumber.Trim())))
                    throw new AppException("شماره انتظامی وارد شده تکراری می باشد");

                if (statusModel.Status == StatusEnum.Approved)
                {
                    await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
                    {
                        Id = statusModel.RequestFacilityId,
                        PoliceNumber = statusModel.PoliceNumber
                    }, cancellationToken, false, nameof(RequestFacility.PoliceNumber), nameof(RequestFacility.UpdateDate));


                    if (isInEnterFacilityNumberStep)
                    {
                        await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.BankLeasingInquiry,
                            StatusEnum.Approved, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken, true);
                    }

                    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId, WorkFlowFormEnum.PendingBankLeasingInquiry,
                        StatusEnum.Approved, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken, autoSave);
                }
                else
                {
                    if (isInEnterFacilityNumberStep && statusModel.Status == StatusEnum.ReturnToCorrection)
                    {
                        var currentStep = await requestFacilityWorkFlowStepRepository
                            .GetByConditionAsync(p => p.RequestFacilityId == statusModel.RequestFacilityId
                            && !p.StatusId.HasValue
                            && p.WorkFlowStep.WorkFlowFormId.Equals(WorkFlowFormEnum.BankLeasingInquiry),
                            cancellationToken);

                        foreach (var id in statusModel.WorkFlowStepRejectionReasonIds)
                        {
                            requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository.Add(new()
                            {
                                RequestFacilityWorkFlowStepId = currentStep.Id,
                                WorkFlowStepRejectionReasonId = id
                            }, false);
                        }
                    }
                    else if (isInBankLeasingInquiryStep && statusModel.Status == StatusEnum.ReturnToCorrection)
                    {
                        var currentStep = await requestFacilityWorkFlowStepRepository
                            .GetByConditionAsync(p => p.RequestFacilityId == statusModel.RequestFacilityId
                            && !p.StatusId.HasValue
                            && p.WorkFlowStep.WorkFlowFormId.Equals(WorkFlowFormEnum.PendingBankLeasingInquiry),
                            cancellationToken);

                        foreach (var id in statusModel.WorkFlowStepRejectionReasonIds)
                        {
                            requestFacilityWorkFlowStepWorkFlowStepRejectionReasonRepository.Add(new()
                            {
                                RequestFacilityWorkFlowStepId = currentStep.Id,
                                WorkFlowStepRejectionReasonId = id
                            }, false);
                        }
                    }
                    

                    await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityId,
                        isInEnterFacilityNumberStep ? WorkFlowFormEnum.BankLeasingInquiry : WorkFlowFormEnum.PendingBankLeasingInquiry,
                        statusModel.Status, 
                        statusModel.OpratorId, 
                        statusModel.StatusDescription + " - " + statusModel.RejectionReasonDescription, 
                        cancellationToken, autoSave);
                }
                return true;
            }

            throw new AppException("درخواست فوق در مرحله ثبت شماره انتظامی قرار ندارد!");
        }

        #region GetRequestFacilityCompleteInfo
        public async Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, Guid userId, CancellationToken cancellationToken)
        {
            long cardNumber, issuanceCardNumber;
            var model = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) && p.BuyerId.Equals(userId),
                x => new
                {
                    x.FacilityType.MonthCountTitle,
                    x.FacilityType.MonthCount,
                    x.GlobalSetting.FinancialInstitutionFacilityFee,
                    x.GlobalSetting.LendTechFacilityFee,
                    x.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                    x.GlobalSetting.WarantyPercentage,
                    x.GlobalSetting.FacilityInterest,
                    x.GlobalSetting.ValidationFee,
                    x.GuarantorIsRequired,
                    x.Amount,
                    x.CancelByUser,
                    x.FacilityNumber,
                    x.PoliceNumber,
                    x.SignedContractByBankFileName,
                    x.SignedContractByUserFileName,
                    x.DepositDocumentFileName,
                    x.DepositDocumentNumber,
                    x.DepositDate,
                    x.OperatorId,
                    x.UserOption,
                    x.Buyer.Person.PlaceOfBirth,
                    LeasingName = x.OrganizationId.HasValue ? x.Organization.Name : null,
                    LeasingId = x.OrganizationId.HasValue ? x.Organization.Id : (int?)null,
                    LeasingNationalId = x.OrganizationId.HasValue ? x.Organization.NationalId : (long?)null,
                    RequestFaciliyUsagePlaces = x.RequestFaciliyUsagePlaces != null
                        ? x.RequestFaciliyUsagePlaces.Select(p => p.UsagePlace.Name)
                        : null,
                    x.UsagePlaceDescription,
                    x.CreatedDate,
                    x.BuyerId,
                    x.Buyer.UserName,
                    x.Buyer.Person.FName,
                    x.Buyer.Person.LName,
                    x.Buyer.Person.FatherName,
                    x.Buyer.Person.BirthDate,
                    x.Buyer.Person.PostalCode,
                    x.Buyer.Person.Address,
                    x.Buyer.Person.Mobile,
                    UserRoles = x.Buyer.UserRoles,
                    x.Buyer.Person.ZP_Id,
                    PersonId = x.Buyer.Person.Id,
                    SanaTrackingId = x.Buyer.Person.SanaTrackingId,
                    AddressCity = x.Buyer.Person.City.Name,
                    AddressProvince = x.Buyer.Person.City.Parent.Name,
                    x.Buyer.Email,
                    x.Buyer.Person.NationalCode,
                    x.Buyer.Person.SSID,
                    x.Buyer.PhoneNumber,
                    x.Buyer.Person.CustomerNumber,
                    CardIssuance = x.RequestFacilityCardIssuance,
                    UserBankAccounts = x.Buyer.UserBankAccounts.Any()
                    ? x.Buyer.UserBankAccounts.Where(p => p.IsConfirm)
                    .Select(p =>
                    new
                    {
                        p.Deposit,
                        p.CardNumber,
                        p.IBAN,
                        p.DepositOwner,
                        p.BankName,
                    }) : null,
                    IdentityDocuments = x.Buyer.UserIdentityDocuments.Any()
                    ? x.Buyer.UserIdentityDocuments.Where(p => !p.IsDeleted)
                    .Select(p =>
                    new
                    {
                        p.FilePath,
                        p.DocumentType,
                        p.Version,
                        p.CreatedDate
                    }) : null,
                    RequestFacilityWorkFlowSteps = x.RequestFacilityWorkFlowSteps.Any()
                    ? x.RequestFacilityWorkFlowSteps.Select(p => new
                    {
                        WorkFlowStepName = p.WorkFlowStep.Name,
                        p.WorkFlowStep.IsLastStep,
                        p.WorkFlowStep.IsApproveFinalStep,
                        p.StatusId,
                        p.CreatedDate
                    })
                    : null
                }, cancellationToken))
                .Select(p => new RequestFacilityInfoModel()
                {
                    Id = requestFacilityId,
                    LeasingId = p.LeasingId,
                    UserId = p.BuyerId,
                    OperatorId = p.OperatorId,
                    RequestFacilityDetail = new RequestFacilityDetailModel()
                    {
                        Amount = p.Amount,
                        FacilityInterest = p.FacilityInterest,
                        FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                        LendTechFacilityFee = p.LendTechFacilityFee,
                        LendTechFacilityForZarinpalClientFee = p.LendTechFacilityForZarinpalClientFee,
                        UserIsZarinpalClient = p.ZP_Id != null && p.ZP_Id > 0,
                        ValidationFee = p.ValidationFee,
                        WarantyPercentage = p.WarantyPercentage,
                        MonthCount = p.MonthCount,
                        MonthCountTitle = p.MonthCountTitle,
                        SignedContractByUserFileName = p.SignedContractByUserFileName,
                        SignedContractByBankFileName = p.SignedContractByBankFileName,
                        FacilityNumber = p.FacilityNumber,
                        PoliceNumber = p.PoliceNumber,
                        CancelByUser = p.CancelByUser,
                        LeasingName = p.LeasingName,
                        UsagePlaceList = p.RequestFaciliyUsagePlaces != null ? p.RequestFaciliyUsagePlaces.ToList() : null,
                        UsagePlaceOtherDescription = p.UsagePlaceDescription,
                        LeasingNationalId = p.LeasingNationalId,
                        CreateDate = p.CreatedDate,
                        UserOption = p.UserOption,
                        GuarantorIsRequired = p.GuarantorIsRequired
                    },
                    UserIdentityInfo = new UserIdentityInfoModel()
                    {
                        PersonId = p.PersonId,
                        Address = p.Address,
                        Email = p.Email,
                        FName = p.FName,
                        LName = p.LName,
                        FatherName = p.FatherName,
                        Mobile = p.Mobile,
                        NationalCode = p.NationalCode,
                        SSID = p.SSID,
                        CustomerNumber = p.CustomerNumber,
                        //BirthCertificateSerial = p.BirthCertificateSerial,
                        BirthDate = p.BirthDate,
                        PhoneNumber = p.PhoneNumber,
                        PostalCode = p.PostalCode,
                        //BirthPlaceCity = p.BirthPlaceCity,
                        //BirthPlaceProvince = p.BirthPlaceProvince,
                        //CityOfIssue = p.CityOfIssue,
                        //ProvinceOfIssue = p.ProvinceOfIssue,
                        AddressCityName = p.AddressCity,
                        AddressProvinceName = p.AddressProvince,
                        UserName = p.UserName,
                        CardNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().CardNumber : null,
                        IBAN = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().IBAN : null,
                        AccountNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().Deposit : null,
                        DepositOwners = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().DepositOwner : null,
                        BankName = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().BankName : null,
                        PersonSanaTrackingId = p.SanaTrackingId,
                        IsSanaInquiryVisible = !p.UserRoles.Any(a =>
                                                                {
                                                                    if (a.Role is null) return false;
                                                                    if (a.Role.Name is null) return false;

                                                                    return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                                                                }),
                        IsAddressInquiryVisible = !p.UserRoles.Any(a =>
                                                                {
                                                                    if (a.Role is null) return false;
                                                                    if (a.Role.Name is null) return false;

                                                                    return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                                                                }),
                        PlaceOfBirth = p.PlaceOfBirth
                    },
                    CardIssuanceModel = p.CardIssuance != null
                    ? new RequestFacilityCardIssuanceModel()
                    {
                        AccountNumber = p.CardIssuance.AccountNumber,
                        CardNumber = !string.IsNullOrEmpty(p.CardIssuance.CardNumber) && p.CardIssuance.CardNumber.Length == 16 && long.TryParse(p.CardIssuance.CardNumber, out issuanceCardNumber)
                                ? issuanceCardNumber.ToString("####-####-####-####")
                                : null,
                        Id = p.CardIssuance.Id,
                        Cvv = p.CardIssuance.Cvv,
                        ExpireMonth = p.CardIssuance.ExpireMonth,
                        ExpireYear = p.CardIssuance.ExpireYear,
                        SecondPassword = p.CardIssuance.SecondPassword
                    }
                    : null,
                    DepositModel = new RequestFacilityDepositDocumentModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        DepositDocumentNumber = p.DepositDocumentNumber,
                        DepositDate = p.DepositDate,
                        DepositDocumentFileName = p.DepositDocumentFileName,
                    },
                    BuyerIdentityDocuments = p.IdentityDocuments != null
                    ? p.IdentityDocuments.Select(x => new DocumentModel()
                    {
                        FilePath = x.FilePath,
                        Version = x.Version,
                        DocumentType = x.DocumentType,
                        CreatedDate = x.CreatedDate
                    })
                    : null,

                    RequestFacilityWorkFlowStepList = p.RequestFacilityWorkFlowSteps != null
                        ? p.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel()
                        {
                            WorkFlowStepName = x.WorkFlowStepName,
                            IsLastStep = x.IsLastStep,
                            IsApproveFinalStep = x.IsApproveFinalStep,
                            StatusId = x.StatusId,
                            CreateDate = x.CreatedDate
                        }).ToList()
                        : null
                })
                .FirstOrDefault();

            if (model == default(RequestFacilityInfoModel))
                throw new NotFoundException();

            return model;
        }
        public async Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, CancellationToken cancellationToken)
        {
            long cardNumber, issuanceCardNumber;
            var model = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId),
                x => new
                {
                    x.FacilityType.MonthCountTitle,
                    x.FacilityType.MonthCount,
                    x.Amount,
                    x.GlobalSetting.FinancialInstitutionFacilityFee,
                    x.GlobalSetting.LendTechFacilityFee,
                    x.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                    x.GlobalSetting.WarantyPercentage,
                    x.GlobalSetting.FacilityInterest,
                    x.GlobalSetting.ValidationFee,
                    x.CancelByUser,
                    x.FacilityNumber,
                    x.PoliceNumber,
                    x.SignedContractByBankFileName,
                    x.SignedContractByUserFileName,
                    x.DepositDocumentFileName,
                    x.DepositDocumentNumber,
                    x.DepositDate,
                    x.OperatorId,
                    x.UserOption,
                    LeasingName = x.OrganizationId.HasValue ? x.Organization.Name : null,
                    LeasingId = x.OrganizationId.HasValue ? x.Organization.Id : (int?)null,
                    LeasingNationalId = x.OrganizationId.HasValue ? x.Organization.NationalId : (long?)null,
                    RequestFaciliyUsagePlaces = x.RequestFaciliyUsagePlaces != null
                        ? x.RequestFaciliyUsagePlaces.Select(p => p.UsagePlace.Name)
                        : null,
                    x.UsagePlaceDescription,
                    x.CreatedDate,
                    x.BuyerId,
                    x.Buyer.Person.ZP_Id,
                    x.Buyer.UserName,
                    x.Buyer.Person.FName,
                    x.Buyer.Person.LName,
                    x.Buyer.Person.FatherName,
                    x.Buyer.Person.BirthDate,
                    x.Buyer.Person.PostalCode,
                    x.Buyer.Person.Address,
                    x.Buyer.Person.Mobile,
                    x.Buyer.Person.HashCardNumber,
                    x.Buyer.UserRoles,
                    x.Buyer.Person.PlaceOfBirth,
                    PersonSanaTrackingCode = x.Buyer.Person.SanaTrackingId,
                    UserBankAccounts = x.Buyer.UserBankAccounts.Any()
                    ? x.Buyer.UserBankAccounts.Where(p => p.IsConfirm)
                    .Select(p =>
                    new
                    {
                        p.Deposit,
                        p.CardNumber,
                        p.IBAN,
                        p.DepositOwner,
                        p.BankName,
                    }) : null,
                    //BirthPlaceCity = x.Buyer.Person.BirthLocation.Name,
                    //BirthPlaceProvince = x.Buyer.Person.BirthLocation.Parent.Name,
                    //CityOfIssue = x.Buyer.Person.CityOfIssue.Name,
                    //ProvinceOfIssue = x.Buyer.Person.CityOfIssue.Parent.Name,
                    AddressCity = x.Buyer.Person.City.Name,
                    AddressProvince = x.Buyer.Person.City.Parent.Name,
                    x.Buyer.Email,
                    PersonId = x.Buyer.Person.Id,
                    x.Buyer.Person.NationalCode,
                    x.Buyer.Person.SSID,
                    //x.Buyer.Person.BirthCertificateSerial,
                    x.Buyer.PhoneNumber,
                    x.Buyer.Person.CustomerNumber,
                    CardIssuance = x.RequestFacilityCardIssuance,
                    IdentityDocuments = x.Buyer.UserIdentityDocuments.Any()
                    ? x.Buyer.UserIdentityDocuments.Where(p => !p.IsDeleted)
                    .Select(p =>
                    new
                    {
                        p.FilePath,
                        p.DocumentType,
                        p.Version,
                        p.CreatedDate
                    }) : null,
                    RequestFacilityWorkFlowSteps = x.RequestFacilityWorkFlowSteps.Any()
                    ? x.RequestFacilityWorkFlowSteps.Select(p => new
                    {
                        WorkFlowStepName = p.WorkFlowStep.Name,
                        p.WorkFlowStep.IsLastStep,
                        p.WorkFlowStep.IsApproveFinalStep,
                        p.StatusId,
                        p.CreatedDate
                    })
                    : null

                }, cancellationToken))
                .Select(p => new RequestFacilityInfoModel()
                {
                    Id = requestFacilityId,
                    LeasingId = p.LeasingId,
                    UserId = p.BuyerId,
                    OperatorId = p.OperatorId,
                    RequestFacilityDetail = new RequestFacilityDetailModel()
                    {
                        UserIsZarinpalClient = p.ZP_Id != null && p.ZP_Id > 0,
                        Amount = p.Amount,
                        MonthCount = p.MonthCount,
                        MonthCountTitle = p.MonthCountTitle,
                        FacilityInterest = p.FacilityInterest,
                        FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                        LendTechFacilityFee = p.LendTechFacilityFee,
                        LendTechFacilityForZarinpalClientFee = p.LendTechFacilityForZarinpalClientFee,
                        ValidationFee = p.ValidationFee,
                        WarantyPercentage = p.WarantyPercentage,
                        SignedContractByUserFileName = p.SignedContractByUserFileName,
                        SignedContractByBankFileName = p.SignedContractByBankFileName,
                        FacilityNumber = p.FacilityNumber,
                        PoliceNumber = p.PoliceNumber,
                        CancelByUser = p.CancelByUser,
                        LeasingName = p.LeasingName,
                        UsagePlaceList = p.RequestFaciliyUsagePlaces != null ? p.RequestFaciliyUsagePlaces.ToList() : null,
                        UsagePlaceOtherDescription = p.UsagePlaceDescription,
                        LeasingNationalId = p.LeasingNationalId,
                        CreateDate = p.CreatedDate,
                        UserOption = p.UserOption
                    },
                    UserIdentityInfo = new UserIdentityInfoModel()
                    {
                        PersonId = p.PersonId,
                        ZP_Id = p.ZP_Id,
                        Address = p.Address,
                        Email = p.Email,
                        FName = p.FName,
                        LName = p.LName,
                        FatherName = p.FatherName,
                        Mobile = p.Mobile,
                        NationalCode = p.NationalCode,
                        SSID = p.SSID,
                        CustomerNumber = p.CustomerNumber,
                        //BirthCertificateSerial = p.BirthCertificateSerial,
                        BirthDate = p.BirthDate,
                        PhoneNumber = p.PhoneNumber,
                        PostalCode = p.PostalCode,
                        //BirthPlaceCity = p.BirthPlaceCity,
                        //BirthPlaceProvince = p.BirthPlaceProvince,
                        //CityOfIssue = p.CityOfIssue,
                        //ProvinceOfIssue = p.ProvinceOfIssue,
                        AddressCityName = p.AddressCity,
                        AddressProvinceName = p.AddressProvince,
                        UserName = p.UserName,
                        HashCardNumber = p.HashCardNumber,
                        CardNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().CardNumber : null,
                        IBAN = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().IBAN : null,
                        AccountNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().Deposit : null,
                        DepositOwners = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().DepositOwner : null,
                        BankName = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().BankName : null,
                        PersonSanaTrackingId = p.PersonSanaTrackingCode,
                        IsSanaInquiryVisible = !p.UserRoles.Any(a =>
                                                                {
                                                                    if (a.Role is null) return false;
                                                                    if (a.Role.Name is null) return false;

                                                                    return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                                                                }),
                        IsAddressInquiryVisible = !p.UserRoles.Any(a =>
                                                                    {
                                                                        if (a.Role is null) return false;
                                                                        if (a.Role.Name is null) return false;

                                                                        return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                                                                    }),
                        PlaceOfBirth = p.PlaceOfBirth
                    },
                    CardIssuanceModel = p.CardIssuance != null
                    ? new RequestFacilityCardIssuanceModel()
                    {
                        AccountNumber = p.CardIssuance.AccountNumber,
                        CardNumber = !string.IsNullOrEmpty(p.CardIssuance.CardNumber) && p.CardIssuance.CardNumber.Length == 16 && long.TryParse(p.CardIssuance.CardNumber, out issuanceCardNumber)
                                ? issuanceCardNumber.ToString("####-####-####-####")
                                : null,
                        Cvv = p.CardIssuance.Cvv,
                        ExpireMonth = p.CardIssuance.ExpireMonth,
                        ExpireYear = p.CardIssuance.ExpireYear,
                        SecondPassword = p.CardIssuance.SecondPassword,
                        Id = p.CardIssuance.Id
                    }
                    : null,
                    DepositModel = new RequestFacilityDepositDocumentModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        DepositDocumentNumber = p.DepositDocumentNumber,
                        DepositDate = p.DepositDate,
                        DepositDocumentFileName = p.DepositDocumentFileName,
                    },
                    BuyerIdentityDocuments = p.IdentityDocuments != null
                    ? p.IdentityDocuments.Select(x => new DocumentModel()
                    {
                        FilePath = x.FilePath,
                        Version = x.Version,
                        DocumentType = x.DocumentType,
                        CreatedDate = x.CreatedDate,
                    })
                    : null,
                    RequestFacilityWorkFlowStepList = p.RequestFacilityWorkFlowSteps != null
                        ? p.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel()
                        {
                            WorkFlowStepName = x.WorkFlowStepName,
                            IsLastStep = x.IsLastStep,
                            IsApproveFinalStep = x.IsApproveFinalStep,
                            StatusId = x.StatusId,
                            CreateDate = x.CreatedDate
                        }).ToList()
                        : null
                })
                .FirstOrDefault();

            if (model == default(RequestFacilityInfoModel))
                throw new NotFoundException();

            return model;
        }
        public async Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, WorkFlowFormEnum workFlowForm, CancellationToken cancellationToken)
        {
            long cardNumber, issuanceCardNumber;
            var model = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId),
                x => new
                {
                    x.FacilityType.MonthCountTitle,
                    x.FacilityType.MonthCount,
                    x.Amount,
                    x.GlobalSetting.FinancialInstitutionFacilityFee,
                    x.GlobalSetting.LendTechFacilityFee,
                    x.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                    x.GlobalSetting.WarantyPercentage,
                    x.GlobalSetting.FacilityInterest,
                    x.GlobalSetting.ValidationFee,
                    x.CancelByUser,
                    x.FacilityNumber,
                    x.PoliceNumber,
                    x.SignedContractByBankFileName,
                    x.SignedContractByUserFileName,
                    x.DepositDocumentFileName,
                    x.DepositDocumentNumber,
                    x.DepositDate,
                    x.OperatorId,
                    x.UserOption,
                    x.GuarantorIsRequired,
                    x.Buyer.Person.PlaceOfBirth,
                    LeasingName = x.OrganizationId.HasValue ? x.Organization.Name : null,
                    LeasingId = x.OrganizationId.HasValue ? x.Organization.Id : (int?)null,
                    LeasingNationalId = x.OrganizationId.HasValue ? x.Organization.NationalId : (long?)null,
                    RequestFaciliyUsagePlaces = x.RequestFaciliyUsagePlaces != null
                        ? x.RequestFaciliyUsagePlaces.Select(p => p.UsagePlace.Name)
                        : null,
                    x.UsagePlaceDescription,
                    x.CreatedDate,
                    x.BuyerId,
                    x.Buyer.UserName,
                    x.Buyer.Person.FName,
                    x.Buyer.Person.LName,
                    x.Buyer.Person.FatherName,
                    x.Buyer.Person.BirthDate,
                    x.Buyer.Person.PostalCode,
                    x.Buyer.Person.Address,
                    x.Buyer.Person.Mobile,
                    x.Buyer.Person.HashCardNumber,
                    x.Buyer.Person.ZP_Id,
                    x.Buyer.UserRoles,
                    PersonSanaTrackingId = x.Buyer.Person.SanaTrackingId,
                    UserBankAccounts = x.Buyer.UserBankAccounts.Any()
                    ? x.Buyer.UserBankAccounts.Where(p => p.IsConfirm)
                    .Select(p =>
                    new
                    {
                        p.Deposit,
                        p.CardNumber,
                        p.IBAN,
                        p.DepositOwner,
                        p.BankName,
                    }) : null,
                    PersonId = x.Buyer.Person.Id,
                    //BirthPlaceCity = x.Buyer.Person.BirthLocation.Name,
                    //BirthPlaceProvince = x.Buyer.Person.BirthLocation.Parent.Name,
                    //CityOfIssue = x.Buyer.Person.CityOfIssue.Name,
                    //ProvinceOfIssue = x.Buyer.Person.CityOfIssue.Parent.Name,
                    AddressCity = x.Buyer.Person.City.Name,
                    AddressProvince = x.Buyer.Person.City.Parent.Name,
                    x.Buyer.Email,
                    x.Buyer.Person.NationalCode,
                    x.Buyer.Person.SSID,
                    //x.Buyer.Person.BirthCertificateSerial,
                    x.Buyer.PhoneNumber,
                    x.Buyer.Person.CustomerNumber,
                    CardIssuance = x.RequestFacilityCardIssuance,
                    IdentityDocuments = x.Buyer.UserIdentityDocuments.Any()
                    ? x.Buyer.UserIdentityDocuments.Where(p => !p.IsDeleted)
                    .Select(p =>
                    new
                    {
                        p.FilePath,
                        p.DocumentType,
                        p.Version,
                        p.CreatedDate
                    }) : null,
                    RequestFacilityWorkFlowSteps = x.RequestFacilityWorkFlowSteps.Any()
                    ? x.RequestFacilityWorkFlowSteps.Select(p => new
                    {
                        WorkFlowStepName = p.WorkFlowStep.Name,
                        p.WorkFlowStep.IsLastStep,
                        p.WorkFlowStep.IsApproveFinalStep,
                        p.StatusId,
                        p.CreatedDate
                    })
                    : null
                }, cancellationToken))
                .Select(p => new RequestFacilityInfoModel()
                {
                    Id = requestFacilityId,
                    LeasingId = p.LeasingId,
                    UserId = p.BuyerId,
                    OperatorId = p.OperatorId,
                    RequestFacilityDetail = new RequestFacilityDetailModel()
                    {
                        UserIsZarinpalClient = p.ZP_Id != null && p.ZP_Id > 0,
                        Amount = p.Amount,
                        MonthCount = p.MonthCount,
                        MonthCountTitle = p.MonthCountTitle,
                        FacilityInterest = p.FacilityInterest,
                        FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                        LendTechFacilityFee = p.LendTechFacilityFee,
                        LendTechFacilityForZarinpalClientFee = p.LendTechFacilityForZarinpalClientFee,
                        ValidationFee = p.ValidationFee,
                        WarantyPercentage = p.WarantyPercentage,
                        SignedContractByUserFileName = p.SignedContractByUserFileName,
                        SignedContractByBankFileName = p.SignedContractByBankFileName,
                        FacilityNumber = p.FacilityNumber,
                        PoliceNumber = p.PoliceNumber,
                        CancelByUser = p.CancelByUser,
                        GuarantorIsRequired = p.GuarantorIsRequired,
                        LeasingName = p.LeasingName,
                        UsagePlaceList = p.RequestFaciliyUsagePlaces != null ? p.RequestFaciliyUsagePlaces.ToList() : null,
                        UsagePlaceOtherDescription = p.UsagePlaceDescription,
                        LeasingNationalId = p.LeasingNationalId,
                        CreateDate = p.CreatedDate,
                        UserOption = p.UserOption
                    },
                    UserIdentityInfo = new UserIdentityInfoModel()
                    {
                        PersonId = p.PersonId,
                        Address = p.Address,
                        Email = p.Email,
                        FName = p.FName,
                        LName = p.LName,
                        FatherName = p.FatherName,
                        Mobile = p.Mobile,
                        NationalCode = p.NationalCode,
                        SSID = p.SSID,
                        CustomerNumber = p.CustomerNumber,
                        //BirthCertificateSerial = p.BirthCertificateSerial,
                        BirthDate = p.BirthDate,
                        PhoneNumber = p.PhoneNumber,
                        PostalCode = p.PostalCode,
                        //BirthPlaceCity = p.BirthPlaceCity,
                        //BirthPlaceProvince = p.BirthPlaceProvince,
                        //CityOfIssue = p.CityOfIssue,
                        //ProvinceOfIssue = p.ProvinceOfIssue,
                        AddressCityName = p.AddressCity,
                        AddressProvinceName = p.AddressProvince,
                        UserName = p.UserName,
                        HashCardNumber = p.HashCardNumber,
                        CardNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().CardNumber : null,
                        IBAN = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().IBAN : null,
                        AccountNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().Deposit : null,
                        DepositOwners = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().DepositOwner : null,
                        BankName = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().BankName : null,
                        IsSanaInquiryVisible = !p.UserRoles.Any(a =>
                                                            {
                                                                if (a.Role is null) return false;
                                                                if (a.Role.Name is null) return false;

                                                                return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                                                            }),
                        IsAddressInquiryVisible = !p.UserRoles.Any(a =>
                        {
                            if (a.Role is null) return false;
                            if (a.Role.Name is null) return false;

                            return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                        }),
                        PlaceOfBirth = p.PlaceOfBirth,
                        PersonSanaTrackingId = p.PersonSanaTrackingId
                    },
                    CardIssuanceModel = p.CardIssuance != null
                    ? new RequestFacilityCardIssuanceModel()
                    {
                        AccountNumber = p.CardIssuance.AccountNumber,
                        CardNumber = !string.IsNullOrEmpty(p.CardIssuance.CardNumber) && p.CardIssuance.CardNumber.Length == 16 && long.TryParse(p.CardIssuance.CardNumber, out issuanceCardNumber)
                                ? issuanceCardNumber.ToString("####-####-####-####")
                                : null,
                        Id = p.CardIssuance.Id,
                        Cvv = p.CardIssuance.Cvv,
                        ExpireMonth = p.CardIssuance.ExpireMonth,
                        ExpireYear = p.CardIssuance.ExpireYear,
                        SecondPassword = p.CardIssuance.SecondPassword
                    }
                    : null,
                    BonCardEditModel = new RequestFacilityEditCardIssuanceModel()
                    {
                        Id = p.CardIssuance != null ? p.CardIssuance.Id : 0,
                        RequestFacilityId = requestFacilityId,
                    },
                    DepositModel = new RequestFacilityDepositDocumentModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        DepositDocumentNumber = p.DepositDocumentNumber,
                        DepositDate = p.DepositDate,
                        DepositDocumentFileName = p.DepositDocumentFileName,
                    },
                    BuyerIdentityDocuments = p.IdentityDocuments != null
                    ? p.IdentityDocuments.Select(x => new DocumentModel()
                    {
                        FilePath = x.FilePath,
                        Version = x.Version,
                        DocumentType = x.DocumentType,
                        CreatedDate = x.CreatedDate,
                    })
                    : null,
                    RequestFacilityWorkFlowStepList = p.RequestFacilityWorkFlowSteps != null
                        ? p.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel()
                        {
                            WorkFlowStepName = x.WorkFlowStepName,
                            IsLastStep = x.IsLastStep,
                            IsApproveFinalStep = x.IsApproveFinalStep,
                            StatusId = x.StatusId,
                            CreateDate = x.CreatedDate
                        }).ToList()
                        : null
                })
                .FirstOrDefault();

            if (model == default(RequestFacilityInfoModel))
                throw new NotFoundException();

            model.WaitingForZarinLend = await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
                new List<RoleEnum> { RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert }, workFlowForm, cancellationToken);
            model.CurrentStepForm = workFlowForm;
            model.Leasings = (await organizattionRepository.SelectByAsync(p => p.IsActive.Equals(true) && p.OrganizationTypeId == (short)OrganizationTypeEnum.BankLeasing,
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString(),
                    }, cancellationToken))
                    .OrderBy(p => p.Text);

            return model;
        }
        public async Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, int leasingId, CancellationToken cancellationToken)
        {
            long cardNumber, issuanceCardNumber;
            var model = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) && p.OrganizationId.Equals(leasingId),
                x => new
                {
                    x.FacilityType.MonthCountTitle,
                    x.FacilityType.MonthCount,
                    x.Amount,
                    x.GlobalSetting.FinancialInstitutionFacilityFee,
                    x.GlobalSetting.LendTechFacilityFee,
                    x.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                    x.GlobalSetting.WarantyPercentage,
                    x.GlobalSetting.FacilityInterest,
                    x.GlobalSetting.ValidationFee,
                    x.CancelByUser,
                    x.FacilityNumber,
                    x.PoliceNumber,
                    x.SignedContractByBankFileName,
                    x.SignedContractByUserFileName,
                    x.DepositDocumentFileName,
                    x.DepositDocumentNumber,
                    x.DepositDate,
                    x.OperatorId,
                    x.UserOption,
                    LeasingName = x.OrganizationId.HasValue ? x.Organization.Name : null,
                    LeasingId = x.OrganizationId.HasValue ? x.Organization.Id : (int?)null,
                    LeasingNationalId = x.OrganizationId.HasValue ? x.Organization.NationalId : (long?)null,
                    RequestFaciliyUsagePlaces = x.RequestFaciliyUsagePlaces != null
                        ? x.RequestFaciliyUsagePlaces.Select(p => p.UsagePlace.Name)
                        : null,
                    x.UsagePlaceDescription,
                    x.CreatedDate,
                    x.BuyerId,
                    x.Buyer.UserName,
                    x.Buyer.Person.FName,
                    x.Buyer.Person.LName,
                    x.Buyer.Person.FatherName,
                    x.Buyer.Person.SSID,
                    x.Buyer.UserRoles,
                    //x.Buyer.Person.BirthCertificateSerial,
                    x.Buyer.Person.BirthDate,
                    x.Buyer.Person.PostalCode,
                    x.Buyer.Person.Address,
                    x.Buyer.Person.Mobile,
                    x.Buyer.Person.ZP_Id,
                    PersonSanaTrackingCode = x.Buyer.Person.SanaTrackingId,
                    UserBankAccounts = x.Buyer.UserBankAccounts.Any()
                    ? x.Buyer.UserBankAccounts.Where(p => p.IsConfirm)
                    .Select(p =>
                    new
                    {
                        p.Deposit,
                        p.CardNumber,
                        p.IBAN,
                        p.DepositOwner,
                        p.BankName,
                    }) : null,
                    //BirthPlaceCity = x.Buyer.Person.BirthLocation.Name,
                    //BirthPlaceProvince = x.Buyer.Person.BirthLocation.Parent.Name,
                    //CityOfIssue = x.Buyer.Person.CityOfIssue.Name,
                    //ProvinceOfIssue = x.Buyer.Person.CityOfIssue.Parent.Name,
                    AddressCity = x.Buyer.Person.City.Name,
                    AddressProvince = x.Buyer.Person.City.Parent.Name,
                    x.Buyer.Email,
                    PersonId = x.Buyer.Person.Id,
                    x.Buyer.Person.NationalCode,
                    x.Buyer.PhoneNumber,
                    x.Buyer.Person.CustomerNumber,
                    x.Buyer.Person.PlaceOfBirth,
                    CardIssuance = x.RequestFacilityCardIssuance,
                    IdentityDocuments = x.Buyer.UserIdentityDocuments.Any()
                    ? x.Buyer.UserIdentityDocuments.Where(p => !p.IsDeleted)
                    .Select(p =>
                    new
                    {
                        p.FilePath,
                        p.DocumentType,
                        p.Version,
                        p.CreatedDate
                    }) : null,
                    RequestFacilityWorkFlowSteps = x.RequestFacilityWorkFlowSteps.Any()
                    ? x.RequestFacilityWorkFlowSteps.Select(p => new
                    {
                        WorkFlowStepName = p.WorkFlowStep.Name,
                        p.WorkFlowStep.IsLastStep,
                        p.WorkFlowStep.IsApproveFinalStep,
                        p.StatusId,
                        p.CreatedDate
                    })
                    : null
                }, cancellationToken))
                .Select(p => new RequestFacilityInfoModel()
                {
                    Id = requestFacilityId,
                    LeasingId = p.LeasingId,
                    UserId = p.BuyerId,
                    OperatorId = p.OperatorId,
                    RequestFacilityDetail = new RequestFacilityDetailModel()
                    {
                        UserIsZarinpalClient = p.ZP_Id != null && p.ZP_Id > 0,
                        Amount = p.Amount,
                        MonthCount = p.MonthCount,
                        MonthCountTitle = p.MonthCountTitle,
                        FacilityInterest = p.FacilityInterest,
                        FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                        LendTechFacilityFee = p.LendTechFacilityFee,
                        LendTechFacilityForZarinpalClientFee = p.LendTechFacilityForZarinpalClientFee,
                        ValidationFee = p.ValidationFee,
                        WarantyPercentage = p.WarantyPercentage,
                        SignedContractByUserFileName = p.SignedContractByUserFileName,
                        SignedContractByBankFileName = p.SignedContractByBankFileName,
                        FacilityNumber = p.FacilityNumber,
                        PoliceNumber = p.PoliceNumber,
                        CancelByUser = p.CancelByUser,
                        LeasingName = p.LeasingName,
                        UsagePlaceList = p.RequestFaciliyUsagePlaces != null ? p.RequestFaciliyUsagePlaces.ToList() : null,
                        UsagePlaceOtherDescription = p.UsagePlaceDescription,
                        LeasingNationalId = p.LeasingNationalId,
                        CreateDate = p.CreatedDate,
                        UserOption = p.UserOption
                    },
                    UserIdentityInfo = new UserIdentityInfoModel()
                    {
                        PersonId = p.PersonId,
                        Address = p.Address,
                        Email = p.Email,
                        FName = p.FName,
                        LName = p.LName,
                        FatherName = p.FatherName,
                        SSID = p.SSID,
                        CustomerNumber = p.CustomerNumber,
                        //BirthCertificateSerial = p.BirthCertificateSerial,
                        BirthDate = p.BirthDate,
                        Mobile = p.Mobile,
                        NationalCode = p.NationalCode,
                        PhoneNumber = p.PhoneNumber,
                        PostalCode = p.PostalCode,
                        UserName = p.UserName,
                        //BirthPlaceCity = p.BirthPlaceCity,
                        //BirthPlaceProvince = p.BirthPlaceProvince,
                        //CityOfIssue = p.CityOfIssue,
                        //ProvinceOfIssue = p.ProvinceOfIssue,
                        AddressCityName = p.AddressCity,
                        AddressProvinceName = p.AddressProvince,
                        CardNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().CardNumber : null,
                        IBAN = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().IBAN : null,
                        AccountNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().Deposit : null,
                        DepositOwners = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().DepositOwner : null,
                        BankName = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().BankName : null,
                        PersonSanaTrackingId = p.PersonSanaTrackingCode,
                        IsSanaInquiryVisible = !p.UserRoles.Any(a =>
                                                            {
                                                                if (a.Role is null) return false;
                                                                if (a.Role.Name is null) return false;

                                                                return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                                                            }),
                        IsAddressInquiryVisible = !p.UserRoles.Any(a =>
                                                                {
                                                                    if (a.Role is null) return false;
                                                                    if (a.Role.Name is null) return false;

                                                                    return a.Role.Name == "Buyer" || a.Role.Name == "Seller";
                                                                }),
                        PlaceOfBirth = p.PlaceOfBirth
                    },
                    CardIssuanceModel = p.CardIssuance != null
                    ? new RequestFacilityCardIssuanceModel()
                    {
                        AccountNumber = p.CardIssuance.AccountNumber,
                        CardNumber = !string.IsNullOrEmpty(p.CardIssuance.CardNumber) && p.CardIssuance.CardNumber.Length == 16 && long.TryParse(p.CardIssuance.CardNumber, out issuanceCardNumber)
                                ? issuanceCardNumber.ToString("####-####-####-####")
                                : null,
                        Id = p.CardIssuance.Id,
                        Cvv = p.CardIssuance.Cvv,
                        ExpireMonth = p.CardIssuance.ExpireMonth,
                        ExpireYear = p.CardIssuance.ExpireYear,
                        SecondPassword = p.CardIssuance.SecondPassword
                    }
                    : null,
                    DepositModel = new RequestFacilityDepositDocumentModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        DepositDocumentNumber = p.DepositDocumentNumber,
                        DepositDate = p.DepositDate,
                        DepositDocumentFileName = p.DepositDocumentFileName,
                    },
                    BuyerIdentityDocuments = p.IdentityDocuments != null
                    ? p.IdentityDocuments.Select(x => new DocumentModel()
                    {
                        FilePath = x.FilePath,
                        Version = x.Version,
                        DocumentType = x.DocumentType,
                        CreatedDate = x.CreatedDate
                    })
                    : null,
                    RequestFacilityWorkFlowStepList = p.RequestFacilityWorkFlowSteps != null
                        ? p.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel()
                        {
                            WorkFlowStepName = x.WorkFlowStepName,
                            IsLastStep = x.IsLastStep,
                            IsApproveFinalStep = x.IsApproveFinalStep,
                            StatusId = x.StatusId,
                            CreateDate = x.CreatedDate
                        }).ToList()
                        : null
                })
                .FirstOrDefault();

            if (model == default(RequestFacilityInfoModel))
                throw new NotFoundException();

            return model;
        }
        public async Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, int leasingId, WorkFlowFormEnum workFlowForm, CancellationToken cancellationToken)
        {
            long cardNumber, issuanceCardNumber;
            var model = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) && p.OrganizationId.Equals(leasingId),
                x => new
                {
                    x.FacilityType.MonthCountTitle,
                    x.FacilityType.MonthCount,
                    x.Amount,
                    x.GlobalSetting.FinancialInstitutionFacilityFee,
                    x.GlobalSetting.LendTechFacilityFee,
                    x.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                    x.GlobalSetting.WarantyPercentage,
                    x.GlobalSetting.FacilityInterest,
                    x.GlobalSetting.ValidationFee,
                    x.CancelByUser,
                    x.FacilityNumber,
                    x.PoliceNumber,
                    x.SignedContractByBankFileName,
                    x.SignedContractByUserFileName,
                    x.DepositDocumentFileName,
                    x.DepositDocumentNumber,
                    x.DepositDate,
                    x.OperatorId,
                    x.UserOption,
                    LeasingName = x.OrganizationId.HasValue ? x.Organization.Name : null,
                    LeasingId = x.OrganizationId.HasValue ? x.Organization.Id : (int?)null,
                    LeasingNationalId = x.OrganizationId.HasValue ? x.Organization.NationalId : (long?)null,
                    RequestFaciliyUsagePlaces = x.RequestFaciliyUsagePlaces != null
                        ? x.RequestFaciliyUsagePlaces.Select(p => p.UsagePlace.Name)
                        : null,
                    x.UsagePlaceDescription,
                    x.CreatedDate,
                    x.BuyerId,
                    x.Buyer.UserName,
                    PersonId = x.Buyer.Person.Id,
                    x.Buyer.Person.FName,
                    x.Buyer.Person.LName,
                    x.Buyer.Person.FatherName,
                    x.Buyer.Person.PostalCode,
                    x.Buyer.Person.SSID,
                    //x.Buyer.Person.BirthCertificateSerial,
                    x.Buyer.Person.BirthDate,
                    x.Buyer.Person.Address,
                    x.Buyer.Person.Mobile,
                    x.Buyer.Person.ZP_Id,
                    UserBankAccounts = x.Buyer.UserBankAccounts.Any()
                    ? x.Buyer.UserBankAccounts.Where(p => p.IsConfirm)
                    .Select(p =>
                    new
                    {
                        p.Deposit,
                        p.CardNumber,
                        p.IBAN,
                        p.DepositOwner,
                        p.BankName,
                    }) : null,
                    //BirthPlaceCity = x.Buyer.Person.BirthLocation.Name,
                    //BirthPlaceProvince = x.Buyer.Person.BirthLocation.Parent.Name,
                    //CityOfIssue = x.Buyer.Person.CityOfIssue.Name,
                    //ProvinceOfIssue = x.Buyer.Person.CityOfIssue.Parent.Name,
                    AddressCity = x.Buyer.Person.City.Name,
                    AddressProvince = x.Buyer.Person.City.Parent.Name,
                    x.Buyer.Email,
                    x.Buyer.Person.NationalCode,
                    x.Buyer.PhoneNumber,
                    x.Buyer.Person.CustomerNumber,
                    x.Buyer.Person.PlaceOfBirth,
                    PersonSanaTrackingId = x.Buyer.Person.SanaTrackingId,
                    CardIssuance = x.RequestFacilityCardIssuance,
                    IdentityDocuments = x.Buyer.UserIdentityDocuments.Any()
                    ? x.Buyer.UserIdentityDocuments.Where(p => !p.IsDeleted)
                    .Select(p =>
                    new
                    {
                        p.FilePath,
                        p.DocumentType,
                        p.Version,
                        p.CreatedDate
                    }) : null,
                    RequestFacilityWorkFlowSteps = x.RequestFacilityWorkFlowSteps.Any()
                    ? x.RequestFacilityWorkFlowSteps.Select(p => new
                    {
                        WorkFlowStepName = p.WorkFlowStep.Name,
                        p.WorkFlowStep.IsLastStep,
                        p.WorkFlowStep.IsApproveFinalStep,
                        p.StatusId,
                        p.CreatedDate,
                        WorkFlowStepId = p.WorkFlowStep.Id
                    })
                    : null

                }, cancellationToken))
                .Select(p => new RequestFacilityInfoModel()
                {
                    Id = requestFacilityId,
                    LeasingId = p.LeasingId,
                    UserId = p.BuyerId,
                    OperatorId = p.OperatorId,
                    RequestFacilityDetail = new RequestFacilityDetailModel()
                    {
                        UserIsZarinpalClient = p.ZP_Id != null && p.ZP_Id > 0,
                        BuyerId = p.BuyerId,
                        Amount = p.Amount,
                        MonthCount = p.MonthCount,
                        MonthCountTitle = p.MonthCountTitle,
                        FacilityInterest = p.FacilityInterest,
                        FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                        LendTechFacilityFee = p.LendTechFacilityFee,
                        LendTechFacilityForZarinpalClientFee = p.LendTechFacilityForZarinpalClientFee,
                        ValidationFee = p.ValidationFee,
                        WarantyPercentage = p.WarantyPercentage,
                        SignedContractByUserFileName = p.SignedContractByUserFileName,
                        SignedContractByBankFileName = p.SignedContractByBankFileName,
                        FacilityNumber = p.FacilityNumber,
                        PoliceNumber = p.PoliceNumber,
                        CancelByUser = p.CancelByUser,
                        LeasingName = p.LeasingName,
                        UsagePlaceList = p.RequestFaciliyUsagePlaces != null ? p.RequestFaciliyUsagePlaces.ToList() : null,
                        UsagePlaceOtherDescription = p.UsagePlaceDescription,
                        LeasingNationalId = p.LeasingNationalId,
                        CreateDate = p.CreatedDate,
                        UserOption = p.UserOption
                    },
                    UserIdentityInfo = new UserIdentityInfoModel()
                    {
                        PersonId = p.PersonId,
                        Address = p.Address,
                        Email = p.Email,
                        FName = p.FName,
                        LName = p.LName,
                        FatherName = p.FatherName,
                        Mobile = p.Mobile,
                        NationalCode = p.NationalCode,
                        SSID = p.SSID,
                        CustomerNumber = p.CustomerNumber,
                        //BirthCertificateSerial = p.BirthCertificateSerial,
                        BirthDate = p.BirthDate,
                        PhoneNumber = p.PhoneNumber,
                        PostalCode = p.PostalCode,
                        //BirthPlaceCity = p.BirthPlaceCity,
                        //BirthPlaceProvince = p.BirthPlaceProvince,
                        //CityOfIssue = p.CityOfIssue,
                        //ProvinceOfIssue = p.ProvinceOfIssue,
                        AddressCityName = p.AddressCity,
                        AddressProvinceName = p.AddressProvince,
                        UserName = p.UserName,
                        CardNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().CardNumber : null,
                        IBAN = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().IBAN : null,
                        AccountNumber = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().Deposit : null,
                        DepositOwners = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().DepositOwner : null,
                        BankName = p.UserBankAccounts != null && p.UserBankAccounts.Any() ? p.UserBankAccounts.First().BankName : null,
                        PlaceOfBirth = p.PlaceOfBirth,
                        PersonSanaTrackingId = p.PersonSanaTrackingId
                    },
                    CardIssuanceModel = p.CardIssuance != null
                    ? new RequestFacilityCardIssuanceModel()
                    {
                        AccountNumber = p.CardIssuance.AccountNumber,
                        CardNumber = !string.IsNullOrEmpty(p.CardIssuance.CardNumber) && p.CardIssuance.CardNumber.Length == 16 && long.TryParse(p.CardIssuance.CardNumber, out issuanceCardNumber)
                                ? issuanceCardNumber.ToString("####-####-####-####")
                                : null,
                        Id = p.CardIssuance.Id,
                        Cvv = p.CardIssuance.Cvv,
                        ExpireMonth = p.CardIssuance.ExpireMonth,
                        ExpireYear = p.CardIssuance.ExpireYear,
                        SecondPassword = p.CardIssuance.SecondPassword
                    }
                    : null,
                    DepositModel = new RequestFacilityDepositDocumentModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        DepositDocumentNumber = p.DepositDocumentNumber,
                        DepositDate = p.DepositDate,
                        DepositDocumentFileName = p.DepositDocumentFileName,
                    },
                    BuyerIdentityDocuments = p.IdentityDocuments != null
                    ? p.IdentityDocuments.Select(x => new DocumentModel()
                    {
                        FilePath = x.FilePath,
                        Version = x.Version,
                        DocumentType = x.DocumentType,
                        CreatedDate = x.CreatedDate,
                    })
                    : null,
                    RequestFacilityWorkFlowStepList = p.RequestFacilityWorkFlowSteps != null
                        ? p.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel()
                        {
                            WorkFlowStepName = x.WorkFlowStepName,
                            IsLastStep = x.IsLastStep,
                            IsApproveFinalStep = x.IsApproveFinalStep,
                            StatusId = x.StatusId,
                            CreateDate = x.CreatedDate,
                            WorkFlowStepId = x.WorkFlowStepId
                        }).ToList()
                        : null
                })
                .FirstOrDefault();

            if (model == default(RequestFacilityInfoModel))
                throw new NotFoundException();

            model.WaitingForLeasing = await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, leasingId,
                new List<RoleEnum> { RoleEnum.BankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.AdminBankLeasing }, workFlowForm, cancellationToken);
            model.CurrentStepForm = workFlowForm;

            foreach (var item in model.RequestFacilityWorkFlowStepList)
            {
                item.WorkFlowStepRejectionReasons = workFlowStepRejectionReasonRepository
                                        .TableNoTracking
                                        .Include(r => r.RejectionReason)
                                        .Where(w => w.WorkFlowStepId == item.WorkFlowStepId)
                                        .ToList();
            }

            return model;
        }
        #endregion GetRequestFacilityCompleteInfo

        #region CheckRequestFacilityWaitingSpecifiedStepAndRole
        public async Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(Guid userId, int requestFacilityId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            return await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.Id.Equals(requestFacilityId) &&
                                p.BuyerId.Equals(userId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => x == c.Role.Name))), cancellationToken);
        }
        public async Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            return await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.Id.Equals(requestFacilityId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))), cancellationToken);
        }
        public async Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, int leasingId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            return await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.Id.Equals(requestFacilityId) &&
                                p.OrganizationId.Equals(leasingId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))), cancellationToken);
        }

        public async Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, int leasingId, Guid operatorId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            return await requestFacilityRepository.TableNoTracking.AnyAsync(p => p.Id.Equals(requestFacilityId) &&
                                p.OrganizationId.Equals(leasingId) &&
                                p.OperatorId == operatorId &&
                                !p.CancelByUser &&
                                p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))), cancellationToken);
        }

        #endregion CheckRequestFacilityWaitingSpecifiedStepAndRole

        #region GetRequestFacilityIdWaitingSpecifiedStepAndRole
        public async Task<int?> GetRequestFacilityIdWaitingSpecifiedStepAndRole(int requestFacilityId, Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var waitingRequestFacility =
                await requestFacilityRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id.Equals(requestFacilityId) &&
                    p.BuyerId.Equals(userId) &&
                    !p.CancelByUser &&
                    p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                    x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                    !x.StatusId.HasValue &&
                    x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => x == c.Role.Name))), cancellationToken);

            return waitingRequestFacility?.Id;
        }
        public async Task<int?> GetRequestFacilityIdWaitingSpecifiedStepAndRole(Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var waitingRequestFacility =
                await requestFacilityRepository.TableNoTracking.FirstOrDefaultAsync(p => p.BuyerId.Equals(userId) &&
                    !p.CancelByUser &&
                    p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                    x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                    !x.StatusId.HasValue &&
                    x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => x == c.Role.Name))), cancellationToken);

            return waitingRequestFacility?.Id;
        }

        #endregion GetRequestFacilityIdWaitingSpecifiedStepAndRole

        #region GetRequestFacilityWaitingSpecifiedStepAndRole
        public async Task<RequestFacilityDetailModel> GetRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var requestFacility = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) &&
                                p.BuyerId.Equals(userId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))),
                                p => new
                                {
                                    p.Id,
                                    p.BuyerId,
                                    p.Buyer.Person.ZP_Id,
                                    p.CreatedDate,
                                    p.FacilityType.MonthCountTitle,
                                    p.FacilityType.MonthCount,
                                    p.GlobalSetting.FinancialInstitutionFacilityFee,
                                    p.GlobalSetting.LendTechFacilityFee,
                                    p.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                                    p.GlobalSetting.WarantyPercentage,
                                    p.GlobalSetting.FacilityInterest,
                                    p.GlobalSetting.ValidationFee,
                                    p.Amount
                                }, cancellationToken))
                                .FirstOrDefault();

            if (requestFacility != null)
                return new RequestFacilityDetailModel()
                {
                    UserIsZarinpalClient = requestFacility.ZP_Id != null && requestFacility.ZP_Id > 0,
                    Amount = requestFacility.Amount,
                    Id = requestFacility.Id,
                    MonthCountTitle = requestFacility.MonthCountTitle,
                    MonthCount = requestFacility.MonthCount,
                    FacilityInterest = requestFacility.FacilityInterest,
                    FinancialInstitutionFacilityFee = requestFacility.FinancialInstitutionFacilityFee,
                    LendTechFacilityFee = requestFacility.LendTechFacilityFee,
                    LendTechFacilityForZarinpalClientFee = requestFacility.LendTechFacilityForZarinpalClientFee,
                    ValidationFee = requestFacility.ValidationFee,
                    WarantyPercentage = requestFacility.WarantyPercentage,
                    BuyerId = requestFacility.BuyerId
                };

            return null;
        }
        public async Task<RequestFacilityDetailModel> GetRequestFacilityWaitingSpecifiedStepAndRole(Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var requestFacility = (await requestFacilityRepository.SelectByAsync(p => p.BuyerId.Equals(userId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))),
                                p => new
                                {
                                    p.Id,
                                    p.BuyerId,
                                    p.Buyer.Person.ZP_Id,
                                    p.CreatedDate,
                                    p.FacilityType.MonthCountTitle,
                                    p.FacilityType.MonthCount,
                                    p.GlobalSetting.FinancialInstitutionFacilityFee,
                                    p.GlobalSetting.LendTechFacilityFee,
                                    p.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                                    p.GlobalSetting.WarantyPercentage,
                                    p.GlobalSetting.FacilityInterest,
                                    p.GlobalSetting.ValidationFee,
                                    p.Amount
                                }, cancellationToken))
                                .FirstOrDefault();

            if (requestFacility != null)
                return new RequestFacilityDetailModel()
                {
                    UserIsZarinpalClient = requestFacility.ZP_Id != null && requestFacility.ZP_Id > 0,
                    Amount = requestFacility.Amount,
                    FacilityInterest = requestFacility.FacilityInterest,
                    FinancialInstitutionFacilityFee = requestFacility.FinancialInstitutionFacilityFee,
                    LendTechFacilityFee = requestFacility.LendTechFacilityFee,
                    LendTechFacilityForZarinpalClientFee = requestFacility.LendTechFacilityForZarinpalClientFee,
                    ValidationFee = requestFacility.ValidationFee,
                    WarantyPercentage = requestFacility.WarantyPercentage,
                    Id = requestFacility.Id,
                    MonthCountTitle = requestFacility.MonthCountTitle,
                    MonthCount = requestFacility.MonthCount,
                    BuyerId = requestFacility.BuyerId
                };

            return null;
        }
        public async Task<RequestFacilityDetailModel> GetRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var requestFacility = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))),
                                p => new
                                {
                                    p.Id,
                                    p.BuyerId,
                                    p.Buyer.Person.ZP_Id,
                                    p.CreatedDate,
                                    p.FacilityType.MonthCountTitle,
                                    p.GlobalSetting.FinancialInstitutionFacilityFee,
                                    p.GlobalSetting.LendTechFacilityFee,
                                    p.GlobalSetting.LendTechFacilityForZarinpalClientFee,
                                    p.GlobalSetting.WarantyPercentage,
                                    p.GlobalSetting.FacilityInterest,
                                    p.GlobalSetting.ValidationFee,
                                    p.Amount
                                }, cancellationToken))
                                .FirstOrDefault();

            if (requestFacility != null)
                return new RequestFacilityDetailModel()
                {
                    UserIsZarinpalClient = requestFacility.ZP_Id != null && requestFacility.ZP_Id > 0,
                    Amount = requestFacility.Amount,
                    FacilityInterest = requestFacility.FacilityInterest,
                    FinancialInstitutionFacilityFee = requestFacility.FinancialInstitutionFacilityFee,
                    LendTechFacilityFee = requestFacility.LendTechFacilityFee,
                    LendTechFacilityForZarinpalClientFee = requestFacility.LendTechFacilityForZarinpalClientFee,
                    ValidationFee = requestFacility.ValidationFee,
                    WarantyPercentage = requestFacility.WarantyPercentage,
                    Id = requestFacility.Id,
                    MonthCountTitle = requestFacility.MonthCountTitle,
                    BuyerId = requestFacility.BuyerId
                };

            return null;
        }
        #endregion GetRequestFacilityWaitingSpecifiedStepAndRole

        #region GetRequestFacilityStepsHistory
        public async Task<List<RequestFacilityWorkFlowStepHistoryModel>> GetRequestFacilityStepsHistory(int requestFacilityId, CancellationToken cancellationToken)
        {
            return (await requestFacilityWorkFlowStepRepository.TableNoTracking
                .Where(p => p.RequestFacilityId == requestFacilityId)
                .OrderByDescending(p => p.CreatedDate)
                .ThenByDescending(p => p.UpdateDate)
                .Select(p => new RequestFacilityWorkFlowStepHistoryModel()
                {
                    StatusDescription = p.StatusDescription,
                    IsApproveFinalStep = p.WorkFlowStep.IsApproveFinalStep,
                    IsLastStep = p.WorkFlowStep.IsLastStep,
                    WorkFlowStepName = p.WorkFlowStep.Name,
                    StatusName = p.StatusId.HasValue ? p.Status.Description : null,
                    Operator = p.Oprator.Person.FName + ' ' + p.Oprator.Person.LName + "(" + p.Oprator.UserName + ")",
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    StatusId = p.StatusId,
                }).ToListAsync(cancellationToken));
        }
        public async Task<List<RequestFacilityWorkFlowStepHistoryModel>> GetRequestFacilityStepsHistory(int requestFacilityId, Guid buyerId, CancellationToken cancellationToken)
        {
            return (await requestFacilityWorkFlowStepRepository.TableNoTracking
                .Where(p => p.RequestFacilityId == requestFacilityId && p.RequestFacility.BuyerId.Equals(buyerId) && p.WorkFlowStep.StepIsManual)
                .OrderByDescending(p => p.CreatedDate)
                .ThenByDescending(p => p.UpdateDate)
                .Select(p => new RequestFacilityWorkFlowStepHistoryModel()
                {
                    StatusDescription = p.StatusDescription,
                    IsApproveFinalStep = p.WorkFlowStep.IsApproveFinalStep,
                    IsLastStep = p.WorkFlowStep.IsLastStep,
                    WorkFlowStepName = p.WorkFlowStep.Name,
                    StatusName = p.StatusId.HasValue ? p.Status.Description : null,
                    Operator = p.Oprator.Person.FName + ' ' + p.Oprator.Person.LName,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    StatusId = p.StatusId,
                }).ToListAsync(cancellationToken));
        }
        public async Task<List<RequestFacilityWorkFlowStepHistoryModel>> GetRequestFacilityStepsHistory(int requestFacilityId, int organizationId, CancellationToken cancellationToken)
        {
            return (await requestFacilityWorkFlowStepRepository.TableNoTracking
                .Where(p => p.RequestFacilityId == requestFacilityId && p.RequestFacility.OrganizationId == organizationId)
                .OrderByDescending(p => p.CreatedDate)
                .ThenByDescending(p => p.UpdateDate)
                .Select(p => new RequestFacilityWorkFlowStepHistoryModel()
                {
                    StatusDescription = p.StatusDescription,
                    IsApproveFinalStep = p.WorkFlowStep.IsApproveFinalStep,
                    IsLastStep = p.WorkFlowStep.IsLastStep,
                    WorkFlowStepName = p.WorkFlowStep.Name,
                    StatusName = p.StatusId.HasValue ? p.Status.Description : null,
                    Operator = p.Oprator.Person.FName + ' ' + p.Oprator.Person.LName,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    StatusId = p.StatusId,
                }).ToListAsync(cancellationToken));
        }
        #endregion GetRequestFacilityStepsHistory

        public async Task<List<SelectListItem>> GetFacilityTypes(CancellationToken cancellationToken)
        {
            return (await facilityTypeRepository.SelectByAsync(p => p.IsActive.Equals(true),
               p => new SelectListItem
               {
                   Text = p.MonthCount.ToString(),
                   Value = p.Id.ToString(),
               }, cancellationToken)).ToList();
        }
        public async Task<List<SelectListItem>> GetUsagePlaces(CancellationToken cancellationToken)
        {
            return (await usagePlaceRepository.SelectByAsync(p => p.IsActive.Equals(true),
               p => new SelectListItem
               {
                   Text = p.Name,
                   Value = p.Id.ToString(),
               }, cancellationToken)).ToList();
        }
        public async Task<RequestFacilityModel> PrepareModelForAdd(Guid userId, string userRisk, CancellationToken cancellationToken)
        {
            var activeGlobaSetting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            if (activeGlobaSetting == null)
                throw new AppException("تنظیمات مربوط به تسهیلات یافت نشد!");

            var model = new RequestFacilityModel();
            model.ZP_Id = await userRepository.TableNoTracking.Where(p => p.Id == userId).Select(p => p.Person.ZP_Id == null || p.Person.ZP_Id == 0 ? null : p.Person.ZP_Id).FirstOrDefaultAsync();
            model.ActiveGlobalSettingId = activeGlobaSetting.Id;
            model.FacilityInterest = activeGlobaSetting.FacilityInterest;
            model.FinancialInstitutionFacilityFee = activeGlobaSetting.FinancialInstitutionFacilityFee;
            model.LendTechFacilityFee = activeGlobaSetting.LendTechFacilityFee;
            model.LendTechFacilityForZarinpalClientFee = activeGlobaSetting.LendTechFacilityForZarinpalClientFee;
            model.WarantyPercentage = activeGlobaSetting.WarantyPercentage;
            model.ValidationFee = activeGlobaSetting.ValidationFee;

            model.PaymentPeriods = await GetFacilityTypes(cancellationToken);
            model.UsagePlaces = await GetUsagePlaces(cancellationToken);

            model.UserHasOpenRequest = await UserHasOpenRequest(userId, cancellationToken);
            model.SumAmountApprovedFacilitiesNotPaid = await GetUserSumAmountApprovedFacilities(userId, cancellationToken);

            model.IranCreditScoringResultRules = await iranCreditScoringResultRuleRepository
                .TableNoTracking
                .Where(p => p.IsActive &&
                            p.IranCreditScoringResultRuleType == IranCreditScoringResultRuleType.ForRequestFacility &&
                            ((userRisk != string.Empty && userRisk.Contains(p.Risk!) && p.Risk != string.Empty) ||
                            (userRisk == string.Empty && p.Risk == string.Empty)))
                .Select(p => new IranCreditScoringResultRuleModel
                {
                    GuarantorIsRequired = p.GuarantorIsRequired,
                    MaximumAmount = p.MaximumAmount,
                    MinimumAmount = p.MinimumAmount,
                    Risk = p.Risk!
                })
                .ToListAsync(cancellationToken);


            return model;
        }
        public async Task<int> AddRequestFacilityByBuyer(RequestFacilityAddModel requestFacilityModel, CancellationToken cancellationToken)
        {
            var activeGlobaSetting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            if (activeGlobaSetting == null)
                throw new AppException("تنظیمات مربوط به تسهیلات یافت نشد!");

            if (await UserHasOpenRequest(requestFacilityModel.BuyerId, cancellationToken))
                throw new AppException(@"کاربر گرامی شما در حال حاضر درخواست فعالی دارید که در لیست درخواست ها میتوانید اقدام به تکمیل آن نمایید
                                         شما نمیتوانید بیش از یک درخواست تسهیلات(وام) فعال بصورت همزمان داشته باشید برای ثبت درخواست جدید یا باید منتظر مشخص شدن وضعیت نهایی درخواست(تایید نهایی / رد درخواست) باشید یا نسبت به 'لغو درخواست' اقدام نمایید");

            var sumAmountApprovedFacilitiesNotPaid = await GetUserSumAmountApprovedFacilities(requestFacilityModel.BuyerId, cancellationToken);
            if (requestFacilityModel.AmountRequest > (500000000 - sumAmountApprovedFacilitiesNotPaid))
                throw new AppException($@"کاربر گرامی مجموع وام های دریافتی و تسویه نشده شما تا این لحظه {sumAmountApprovedFacilitiesNotPaid:N0} ريال می باشد،شما در مجموع می توانید مبلغ {500000000:N0} ريال تسهیلات فعال تسویه نشده داشته باشید");

            var lastUserRisk = await iranCreditScoringRepository.TableNoTracking
                .Where(p => p.CreatorId.Equals(requestFacilityModel.BuyerId) &&
                            p.CreatedDate.AddDays(activeGlobaSetting.ValidityPeriodOfValidation).Date >= DateTime.Now.Date)
                .Select(p => p.Risk)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastUserRisk == null) throw new AppException("نتیجه اعتبارسنجی فعالی یافت نشد!");

            var rulesBaseUserVerifyResult = await iranCreditScoringResultRuleRepository
                            .TableNoTracking
                            .OrderBy(p => p.MaximumAmount)
                            .Where(p => p.IsActive &&
                                        p.IranCreditScoringResultRuleType == IranCreditScoringResultRuleType.ForRequestFacility &&
                                        ((lastUserRisk != string.Empty && lastUserRisk.Contains(p.Risk) && p.Risk != string.Empty) ||
                                        (lastUserRisk == string.Empty && p.Risk == string.Empty)))
                            .Select(p => new
                            {
                                p.Id,
                                p.GuarantorIsRequired,
                                p.MaximumAmount,
                                p.MinimumAmount,
                                p.Risk
                            })
                            .ToListAsync(cancellationToken);

            #region According to the validation result, we check that the user has met the conditions
            if (!rulesBaseUserVerifyResult.Any())
                throw new AppException("کاربر گرامی متاسفانه شما شرایط ثبت درخواست تسهیلات را ندارید!");

            var iranCreditScoringResultRuleId = -1;
            var guarantorIsRequired = false;
            foreach (var rule in rulesBaseUserVerifyResult)
            {
                if (requestFacilityModel.AmountRequest > rule.MaximumAmount) continue;
                iranCreditScoringResultRuleId = rule.Id;
                guarantorIsRequired = rule.GuarantorIsRequired;
                break;
            }

            if (iranCreditScoringResultRuleId == -1)
                throw new AppException("کاربر گرامی متاسفانه شما شرایط ثبت درخواست تسهیلات را ندارید!");

            #endregion According to the validation result, we check that the user has met the conditions

            var firstStep = await workFlowStepRepository.GetFirstStep(WorkFlowEnum.RequestFacility, new List<RoleEnum> { RoleEnum.Buyer }, cancellationToken);
            if (firstStep == null)
                throw new AppException("First Step to WorkFlow to ID=1 is NULL");

            //var usagePlaces = requestFacilityModel.UsagePlaceIds
            //    .Select(p => new RequestFaciliyUsagePlace()
            //    {
            //        UsagePlaceId = p
            //    })
            //    .ToList();
            var requestFacility = new RequestFacility()
            {
                IranCreditScoringResultRuleId = iranCreditScoringResultRuleId,
                GuarantorIsRequired = guarantorIsRequired,
                AwaitingIntroductionGuarantor = guarantorIsRequired,
                GlobalSettingId = activeGlobaSetting.Id,
                Amount = requestFacilityModel.AmountRequest,
                BuyerId = requestFacilityModel.BuyerId,
                FacilityTypeId = requestFacilityModel.FacilityTypeId,
                UserOption = requestFacilityModel.UserOption,
                //UsagePlaceDescription = requestFacilityModel.UsagePlaceIds.Any(p => p == 10000) ? requestFacilityModel.UsagePlaceDescription : null,
                //StatusId = null,
                RequestFacilityWorkFlowSteps = new List<RequestFacilityWorkFlowStep>()
                {
                    new RequestFacilityWorkFlowStep()
                    {
                        OpratorId = requestFacilityModel.BuyerId,
                        StatusId = (short)StatusEnum.Approved,
                        StatusDescription ="بعد از ثبت درخواست تسهیلات توسط خریدار،بصورت اتوماتیک توسط سیستم ایجاد و تایید شد",
                        WorkFlowStepId = firstStep.Id,
                    },
                     new RequestFacilityWorkFlowStep()
                    {
                        OpratorId = requestFacilityModel.BuyerId,
                        StatusDescription ="بعد از ثبت درخواست تسهیلات توسط خریدار،بصورت اتوماتیک توسط سیستم ایجاد شد",
                        WorkFlowStepId = firstStep.ApproveNextStepId!.Value,
                    }
                },
                //RequestFaciliyUsagePlaces = usagePlaces
            };
            await requestFacilityRepository.AddAsync(requestFacility, cancellationToken, true);
            return requestFacility.Id;
        }
        public async Task CancelByUser(int requestFacilityId, Guid userId, CancellationToken cancellationToken)
        {
            var requestFacility = await requestFacilityRepository.GetByConditionAsync(p => p.Id.Equals(requestFacilityId) && p.BuyerId.Equals(userId), cancellationToken);
            await RequestFacilityCancelation(requestFacility, userId, cancellationToken);
        }

        public async Task CancelByZarinLendAdmin(int requestFacilityId, Guid userId, CancellationToken cancellationToken)
        {
            var requestFacility = await requestFacilityRepository.GetByConditionAsync(p => p.Id.Equals(requestFacilityId), cancellationToken);
            await RequestFacilityCancelation(requestFacility, userId, cancellationToken);
        }

        private async Task RequestFacilityCancelation(RequestFacility requestFacility, Guid userId, CancellationToken cancellationToken) 
        {
            if (requestFacility == null)
                throw new LogicException($"درخواست یافت نشد!", new { requestFacility.Id, userId });

            if (requestFacility.CancelByUser)
                throw new AppException("درخواست قبلا بسته شده است!");

            if (await requestFacilityWorkFlowStepRepository.TableNoTracking
                 .AnyAsync(p => p.RequestFacilityId.Equals(requestFacility.Id) &&
                                p.WorkFlowStep.IsApproveFinalStep &&
                                p.WorkFlowStep.IsLastStep &&
                                p.StatusId.HasValue &&
                                p.StatusId.Value == (short)StatusEnum.Approved))
                throw new LogicException("درخواست تایید نهایی شده است،امکان 'انصراف درخواست' وجود ندارد");

            if (await requestFacilityWorkFlowStepRepository.TableNoTracking
                .AnyAsync(p => p.RequestFacilityId.Equals(requestFacility.Id) &&
                               p.WorkFlowStep.IsLastStep &&
                               p.StatusId.HasValue))
                throw new LogicException("درخواست رد شده شده است،امکان 'انصراف درخواست' وجود ندارد");

            await requestFacilityRepository.UpdateCustomPropertiesAsync(new RequestFacility()
            {
                Id = requestFacility.Id,
                CancelByUser = true
            }, cancellationToken,
            true,
            nameof(RequestFacility.CancelByUser),
            nameof(RequestFacility.UpdateDate));
        }

        public async Task<PagingDto<RequestFacilityListModel>> GetBuyerRequests(Guid userId, PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var buyerFacilities = requestFacilityRepository.TableNoTracking
                .Include(p => p.FacilityType)
                .Include(p => p.Organization)
                .Include(p => p.Buyer.Person)
                    .ThenInclude(p => p.Organization)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowForm)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                            .ThenInclude(p => p.Role)
                .AsSplitQuery()
                .Where(p => p.BuyerId.Equals(userId));

            if (filter.FilterList == null || !filter.FilterList.Any())
            {
                buyerFacilities = buyerFacilities.Where(p => !p.CancelByUser &&
                                                             (p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue) ||
                                                              p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.IsApproveFinalStep &&
                                                                                                      x.WorkFlowStep.IsLastStep &&
                                                                                                      x.StatusId.HasValue &&
                                                                                                      x.StatusId.Value == (short)StatusEnum.Approved)));
            }
            else
                ApplyFilter(ref buyerFacilities, filter, new List<RoleEnum> { RoleEnum.Buyer });

            var waitingFacilities = buyerFacilities
                .Where(p => !p.CancelByUser &&
                       p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                                                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => RoleEnum.Buyer.ToString() == c.Role.Name)))
                .Select(p => new { item = p, Order = 1 });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = buyerFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                .Select(p => new { item = p, Order = 2 });

            var filterList = (await waitingFacilities.Union(otherFacilities)
                .OrderBy(p => p.Order)
                .ThenByDescending(p => p.item.CreatedDate)
                .ThenByDescending(p => p.item.UpdateDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false))
                .Select(p => new RequestFacilityListModel
                {
                    Id = p.item.Id,
                    Amount = p.item.Amount,
                    MonthCountTitle = p.item.FacilityType.MonthCountTitle,
                    LeasingName = p.item.OrganizationId.HasValue ? p.item.Organization.Name : string.Empty,
                    CancelByUser = p.item.CancelByUser,
                    CreateDate = p.item.CreatedDate,
                    UpdateDate = p.item.UpdateDate,
                    GuarantorIsRequired = p.item.GuarantorIsRequired,
                    AwaitingIntroductionGuarantor = p.item.AwaitingIntroductionGuarantor,
                    RequestFacilityWorkFlowStepList = p.item.RequestFacilityWorkFlowSteps.Any()
                    ? p.item.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel
                    {
                        Id = x.Id,
                        WorkFlowStepId = x.WorkFlowStepId,
                        WorkFlowStepName = x.WorkFlowStep.Name,
                        IsLastStep = x.WorkFlowStep.IsLastStep,
                        IsApproveFinalStep = x.WorkFlowStep.IsApproveFinalStep,
                        StepIsManual = x.WorkFlowStep.StepIsManual,
                        WorkFlowFormUrl = x.WorkFlowStep.WorkFlowFormId.HasValue ? x.WorkFlowStep.WorkFlowForm.Url : null,
                        StatusId = x.StatusId,
                        StatusDescription = x.StatusId.HasValue ? x.Status.Description : null,
                        CreateDate = x.CreatedDate,
                        UpdateDate = x.UpdateDate,
                        WorkFlowStepRoles = x.WorkFlowStep.WorkFlowStepRoles.Any() ? x.WorkFlowStep.WorkFlowStepRoles.Select(p => p.Role.Name) : null
                    }).ToList()
                    : null
                }).ToList();

            if (filterList != null && filterList.Any())
            {
                //filterList = filterList.ToList();
                foreach (var request in filterList)
                {
                    if (request.RequestFacilityWorkFlowStepList != null &&
                        request.RequestFacilityWorkFlowStepList.Any())
                    {
                        request.MaxWorkFlowStepId = request.RequestFacilityWorkFlowStepList.Max(p => p.Id);
                        request.LastActionDate = request.RequestFacilityWorkFlowStepList.Max(p => p.CreateDate);
                        if (!request.CancelByUser &&
                            request.RequestFacilityWorkFlowStepList.Any(p => !p.StatusId.HasValue &&
                                                                        p.WorkFlowStepRoles != null &&
                                                                        p.StepIsManual &&
                                                                        !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                                                        p.WorkFlowStepRoles.Any(x => x == RoleEnum.Buyer.ToString())))
                        {
                            var waitingStep = request.RequestFacilityWorkFlowStepList.First(p => !p.StatusId.HasValue &&
                                    p.WorkFlowStepRoles != null &&
                                    p.StepIsManual &&
                                    !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                    p.WorkFlowStepRoles.Any(x => x == RoleEnum.Buyer.ToString()));
                            request.FormUrl = $"{waitingStep.WorkFlowFormUrl}/{request.Id}";
                        }
                    }
                }

                var waitngList = filterList.Where(p => p.FormUrl != null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();
                var noneWaitingList = filterList.Where(p => p.FormUrl == null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();

                filterList = waitngList.Concat(noneWaitingList).ToList();
            }

            var totalRowCounts = await buyerFacilities.CountAsync();

            var pagingResult = new PagingDto<RequestFacilityListModel>()
            {
                CurrentPage = filter.Page,
                Data = filterList,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return pagingResult;
        }
        public async Task<PagingDto<RequestFacilityListModel>> GetAllLeasingRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var rolesString = roles.Select(p => p.ToString());
            var organizationFacilities = requestFacilityRepository.TableNoTracking
                .Include(p => p.Organization)
                .Include(p => p.FacilityType)
                .Include(p => p.Buyer.Person)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowForm)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                            .ThenInclude(p => p.Role)
                .AsSplitQuery()
                .Where(p => p.OrganizationId.Equals(leasingId));
            ApplyFilter(ref organizationFacilities, filter, roles);

            var waitingFacilities = organizationFacilities
              .Where(p => !p.CancelByUser &&
                     p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                     x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
              .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = organizationFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var filterList = (await waitingFacilities.Union(otherFacilities)
                .OrderBy(p => p.Order)
                .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false))
                .Select(p => new RequestFacilityListModel
                {
                    Id = p.item.Id,
                    Amount = p.item.Amount,
                    MonthCountTitle = p.item.FacilityType.MonthCountTitle,
                    CancelByUser = p.item.CancelByUser,
                    Requester = $"{p.item.Buyer.Person.FName} {p.item.Buyer.Person.LName}",
                    RequesterFName = p.item.Buyer.Person.FName,
                    RequesterLName = p.item.Buyer.Person.LName,
                    NationalCode = p.item.Buyer.Person.NationalCode,
                    CreateDate = p.item.CreatedDate,
                    UpdateDate = p.item.UpdateDate,
                    SignedContractByBankFileName = p.item.SignedContractByBankFileName,
                    RequestFacilityWorkFlowStepList = p.item.RequestFacilityWorkFlowSteps.Any()
                    ? p.item.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel
                    {
                        Id = x.Id,
                        WorkFlowStepId = x.WorkFlowStepId,
                        WorkFlowFormId = x.WorkFlowStep.WorkFlowFormId,
                        WorkFlowStepName = x.WorkFlowStep.Name,
                        IsLastStep = x.WorkFlowStep.IsLastStep,
                        IsApproveFinalStep = x.WorkFlowStep.IsApproveFinalStep,
                        WorkFlowFormUrl = x.WorkFlowStep.WorkFlowFormId.HasValue ? x.WorkFlowStep.WorkFlowForm.Url : null,
                        StatusId = x.StatusId,
                        StatusDescription = x.StatusId.HasValue ? x.Status.Description : null,
                        CreateDate = x.CreatedDate,
                        UpdateDate = x.UpdateDate,
                        StepIsManual = x.WorkFlowStep.StepIsManual,
                        WorkFlowStepRoles = x.WorkFlowStep.WorkFlowStepRoles.Any() ? x.WorkFlowStep.WorkFlowStepRoles.Select(p => p.Role.Name) : null
                    }).ToList()
                    : null
                }).ToList();

            if (filterList.Any())
            {
                foreach (var request in filterList)
                {
                    if (request.RequestFacilityWorkFlowStepList != null &&
                        request.RequestFacilityWorkFlowStepList.Any())
                    {
                        request.MaxWorkFlowStepId = request.RequestFacilityWorkFlowStepList.Max(p => p.Id);
                        request.LastActionDate = request.RequestFacilityWorkFlowStepList.Max(p => p.CreateDate);
                        if (!request.CancelByUser &&
                            request.RequestFacilityWorkFlowStepList.Any(p => !p.StatusId.HasValue &&
                                                                        p.WorkFlowStepRoles != null &&
                                                                        p.StepIsManual &&
                                                                        !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                                                        p.WorkFlowStepRoles.Any(x => rolesString.Any(d => d == x))))
                        {
                            var waitingStep = request.RequestFacilityWorkFlowStepList.First(p => !p.StatusId.HasValue &&
                                    p.WorkFlowStepRoles != null &&
                                    p.StepIsManual &&
                                    !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                    p.WorkFlowStepRoles.Any(x => rolesString.Any(d => d == x)));
                            request.FormUrl = $"{waitingStep.WorkFlowFormUrl}/{request.Id}";
                            //request.FormUrl = $"{waitingStep.WorkFlowFormUrl}";
                        }
                    }
                }
                var waitngList = filterList.Where(p => p.FormUrl != null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();
                var noneWaitingList = filterList.Where(p => p.FormUrl == null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();

                filterList = waitngList.Concat(noneWaitingList).ToList();
            }

            var totalRowCounts = await organizationFacilities.CountAsync();

            var pagingResult = new PagingDto<RequestFacilityListModel>()
            {
                CurrentPage = filter.Page,
                Data = filterList,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return pagingResult;
        }
        public async Task<PagingDto<RequestFacilityListModel>> SearchLeasingRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var rolesString = roles.Select(p => p.ToString());
            var allFacilities = requestFacilityRepository.TableNoTracking
                .Where(p => p.OrganizationId == leasingId)
                .Include(p => p.Organization)
                .Include(p => p.FacilityType)
                .Include(p => p.Buyer.Person)
                .Include(p => p.Operator.Person)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowForm)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                            .ThenInclude(p => p.Role)
                .AsSplitQuery();

            ApplyFilter(ref allFacilities, filter, roles);

            var waitingFacilities = allFacilities
              .Where(p => !p.CancelByUser &&
                     p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                     x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
              .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = allFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var query = executeForExport
                        ? (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false))
                        : (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .Skip((filter.Page - 1) * filter.PageSize)
                                                 .Take(filter.PageSize)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false));

            var filterList = query
                             .Select(p => new RequestFacilityListModel
                             {
                                 Id = p.item.Id,
                                 Amount = p.item.Amount,
                                 MonthCountTitle = p.item.FacilityType.MonthCountTitle,
                                 CancelByUser = p.item.CancelByUser,
                                 LeasingName = p.item.OrganizationId.HasValue ? p.item.Organization.Name : string.Empty,
                                 Requester = $"{p.item.Buyer.Person.FName} {p.item.Buyer.Person.LName}",
                                 RequesterFName = p.item.Buyer.Person.FName,
                                 RequesterLName = p.item.Buyer.Person.LName,
                                 Operator = p.item.OperatorId.HasValue ? $"{p.item.Operator.Person.FName} {p.item.Operator.Person.LName}" : string.Empty,
                                 NationalCode = p.item.Buyer.Person.NationalCode,
                                 SignedContractByBankFileName = p.item.SignedContractByBankFileName,
                                 CreateDate = p.item.CreatedDate,
                                 UpdateDate = p.item.UpdateDate,
                                 RequestFacilityWorkFlowStepList = p.item.RequestFacilityWorkFlowSteps.Any()
                                     ? p.item.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel
                                     {
                                         Id = x.Id,
                                         WorkFlowStepId = x.WorkFlowStepId,
                                         WorkFlowStepName = x.WorkFlowStep.Name,
                                         IsLastStep = x.WorkFlowStep.IsLastStep,
                                         IsApproveFinalStep = x.WorkFlowStep.IsApproveFinalStep,
                                         WorkFlowFormId = x.WorkFlowStep.WorkFlowFormId,
                                         WorkFlowFormUrl = x.WorkFlowStep.WorkFlowFormId.HasValue ? x.WorkFlowStep.WorkFlowForm.Url : null,
                                         StatusId = x.StatusId,
                                         StatusDescription = x.StatusId.HasValue ? x.Status.Description : null,
                                         CreateDate = x.CreatedDate,
                                         UpdateDate = x.UpdateDate,
                                         StepIsManual = x.WorkFlowStep.StepIsManual,
                                         WorkFlowStepRoles = x.WorkFlowStep.WorkFlowStepRoles.Any() ? x.WorkFlowStep.WorkFlowStepRoles.Select(p => p.Role.Name) : null
                                     }).ToList()
                                     : null,
                             }).ToList();

            if (filterList.Any())
            {
                foreach (var request in filterList)
                {
                    if (request.RequestFacilityWorkFlowStepList != null &&
                        request.RequestFacilityWorkFlowStepList.Any())
                    {
                        request.MaxWorkFlowStepId = request.RequestFacilityWorkFlowStepList.Max(p => p.Id);
                        request.LastActionDate = request.RequestFacilityWorkFlowStepList.Max(p => p.CreateDate);
                        if (!request.CancelByUser &&
                            request.RequestFacilityWorkFlowStepList.Any(p => !p.StatusId.HasValue &&
                                                                        p.WorkFlowStepRoles != null &&
                                                                        p.StepIsManual &&
                                                                        !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                                                        p.WorkFlowStepRoles.Any(x => rolesString.Any(d => d == x))))
                        {
                            var waitingStep = request.RequestFacilityWorkFlowStepList.First(p => !p.StatusId.HasValue &&
                                    p.WorkFlowStepRoles != null &&
                                    p.StepIsManual &&
                                    !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                    p.WorkFlowStepRoles.Any(x => rolesString.Any(d => d == x)));
                            request.FormUrl = $"{waitingStep.WorkFlowFormUrl}/{request.Id}";
                        }
                    }
                }
                var waitngList = filterList.Where(p => p.FormUrl != null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();
                var noneWaitingList = filterList.Where(p => p.FormUrl == null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();

                filterList = waitngList.Concat(noneWaitingList).ToList();
            }

            if (!executeForExport)
            {
                var totalRowCounts = await allFacilities.CountAsync();

                return new PagingDto<RequestFacilityListModel>()
                {
                    CurrentPage = filter.Page,
                    Data = filterList,
                    TotalRowCount = totalRowCounts,
                    TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
                };
            }
            else
            {
                return new PagingDto<RequestFacilityListModel>()
                {
                    Data = filterList
                };
            }
        }
        public async Task<PagingDto<RequestFacilityListModel>> SearchRequest(PagingFilterDto filter, List<RoleEnum> roles, bool executeForExport, CancellationToken cancellationToken)
        {
            var rolesString = roles.Select(p => p.ToString());
            var allFacilities = requestFacilityRepository.TableNoTracking
                .Include(p => p.Organization)
                .Include(p => p.FacilityType)
                .Include(p => p.Buyer.Person)
                .Include(p => p.Operator.Person)
                .Include(p => p.RequestFacilityGuarantors)
                    .ThenInclude(p => p.Guarantor)
                        .ThenInclude(p => p.Person)
                .Include(p => p.RequestFacilityGuarantors)
                    .ThenInclude(p => p.RequestFacilityGuarantorWorkFlowSteps)
                        .ThenInclude(p => p.WorkFlowStep)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowForm)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                            .ThenInclude(p => p.Role)
                .AsSplitQuery();

            ApplyFilter(ref allFacilities, filter, roles);

            var waitingFacilities = allFacilities
              .Where(p => !p.CancelByUser &&
                     p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                     x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
              .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = allFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });


            var query = executeForExport
                        ? (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false))
                        : (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .Skip((filter.Page - 1) * filter.PageSize)
                                                 .Take(filter.PageSize)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false));

            var filterList = query
                .Select(p => new RequestFacilityListModel
                {
                    Id = p.item.Id,
                    ReguesterId = p.item.BuyerId,
                    Amount = p.item.Amount,
                    MonthCountTitle = p.item.FacilityType.MonthCountTitle,
                    CancelByUser = p.item.CancelByUser,
                    LeasingName = p.item.OrganizationId.HasValue ? p.item.Organization.Name : string.Empty,
                    Requester = $"{p.item.Buyer.Person.FName} {p.item.Buyer.Person.LName}",
                    RequesterFName = p.item.Buyer.Person.FName,
                    RequesterLName = p.item.Buyer.Person.LName,
                    RequesterMobile = p.item.Buyer.Person.Mobile,
                    BirthDate = p.item.Buyer.Person.BirthDate,
                    Operator = p.item.OperatorId.HasValue ? $"{p.item.Operator.Person.FName} {p.item.Operator.Person.LName}" : string.Empty,
                    NationalCode = p.item.Buyer.Person.NationalCode,
                    CreateDate = p.item.CreatedDate,
                    UpdateDate = p.item.UpdateDate,
                    GuarantorIsRequired = p.item.GuarantorIsRequired,
                    AwaitingIntroductionGuarantor = p.item.AwaitingIntroductionGuarantor,
                    RequestFacilityGuarantorsDetail = p.item.GuarantorIsRequired
                    ? p.item.RequestFacilityGuarantors.SelectMany(x => x.RequestFacilityGuarantorWorkFlowSteps,
                                                                  (x, y) => new { x, y })
                                                      .GroupBy(p => new { p.x.Id, p.x.Guarantor.Person.FullName, p.x.CancelByUser }, p => p.y,
                                                               (keys, items) => new RequestFacilityGuarantorDetailModel
                                                               {
                                                                   RequestFacilityGuarantorId = keys.Id,
                                                                   GuarantorFullName = keys.FullName,
                                                                   CancelByUser = keys.CancelByUser,
                                                                   RequestFacilityGuarantorWorkFlowStepList = items.Any()
                                                                        ? items.Select(x => new WorkFlowStepListModel
                                                                        {
                                                                            Id = x.Id,
                                                                            WorkFlowStepId = x.WorkFlowStepId,
                                                                            WorkFlowStepName = x.WorkFlowStep.Name,
                                                                            IsLastStep = x.WorkFlowStep.IsLastStep,
                                                                            IsApproveFinalStep = x.WorkFlowStep.IsApproveFinalStep,
                                                                            //WorkFlowFormUrl = x.WorkFlowStep.WorkFlowFormId.HasValue ? x.WorkFlowStep.WorkFlowForm.Url : null,
                                                                            StatusId = x.StatusId,
                                                                            //StatusDescription = x.StatusId.HasValue ? x.Status.Description : null,
                                                                            CreateDate = x.CreatedDate,
                                                                            UpdateDate = x.UpdateDate,
                                                                            StepIsManual = x.WorkFlowStep.StepIsManual,
                                                                            //WorkFlowStepRoles = x.WorkFlowStep.WorkFlowStepRoles.Any() ? x.WorkFlowStep.WorkFlowStepRoles.Select(p => p.Role.Name) : null
                                                                        }).ToList()
                                                                         : null,
                                                               }).ToList()


                    : null,
                    RequestFacilityWorkFlowStepList = p.item.RequestFacilityWorkFlowSteps.Any()
                        ? p.item.RequestFacilityWorkFlowSteps.Select(x => new WorkFlowStepListModel
                        {
                            Id = x.Id,
                            WorkFlowStepId = x.WorkFlowStepId,
                            WorkFlowStepName = x.WorkFlowStep.Name,
                            IsLastStep = x.WorkFlowStep.IsLastStep,
                            IsApproveFinalStep = x.WorkFlowStep.IsApproveFinalStep,
                            WorkFlowFormUrl = x.WorkFlowStep.WorkFlowFormId.HasValue ? x.WorkFlowStep.WorkFlowForm.Url : null,
                            WorkFlowFormId = x.WorkFlowStep.WorkFlowFormId,
                            StatusId = x.StatusId,
                            StatusDescription = x.StatusId.HasValue ? x.Status.Description : null,
                            CreateDate = x.CreatedDate,
                            UpdateDate = x.UpdateDate,
                            StepIsManual = x.WorkFlowStep.StepIsManual,
                            WorkFlowStepRoles = x.WorkFlowStep.WorkFlowStepRoles.Any() ? x.WorkFlowStep.WorkFlowStepRoles.Select(p => p.Role.Name) : null
                        }).ToList()
                        : null,
                }).ToList();

            if (filterList.Any())
            {
                foreach (var request in filterList)
                {
                    if (request.RequestFacilityWorkFlowStepList != null &&
                        request.RequestFacilityWorkFlowStepList.Any())
                    {
                        request.MaxWorkFlowStepId = request.RequestFacilityWorkFlowStepList.Max(p => p.Id);
                        request.LastActionDate = request.RequestFacilityWorkFlowStepList.Max(p => p.CreateDate);
                        if (!request.CancelByUser &&
                            request.RequestFacilityWorkFlowStepList.Any(p => !p.StatusId.HasValue &&
                                                                        p.WorkFlowStepRoles != null &&
                                                                        p.StepIsManual &&
                                                                        !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                                                        p.WorkFlowStepRoles.Any(x => roles.Any(c => c.ToString() == x))))
                        {
                            var waitingStep = request.RequestFacilityWorkFlowStepList.First(p => !p.StatusId.HasValue &&
                                    p.WorkFlowStepRoles != null &&
                                    p.StepIsManual &&
                                    !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                    p.WorkFlowStepRoles.Any(x => roles.Any(c => c.ToString() == x)));
                            request.FormUrl = $"{waitingStep.WorkFlowFormUrl}/{request.Id}";
                        }
                    }
                }
                var waitngList = filterList.Where(p => p.FormUrl != null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();
                var noneWaitingList = filterList.Where(p => p.FormUrl == null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();

                filterList = waitngList.Concat(noneWaitingList).ToList();
            }

            if (!executeForExport)
            {
                var totalRowCounts = await allFacilities.CountAsync();

                return new PagingDto<RequestFacilityListModel>()
                {
                    CurrentPage = filter.Page,
                    Data = filterList,
                    TotalRowCount = totalRowCounts,
                    TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
                };
            }
            else
            {
                return new PagingDto<RequestFacilityListModel>()
                {
                    Data = filterList
                };
            }
        }
        public async Task<PagingDto<PendingInEnterFacilityNumberModel>> SearchPendingInEnterFacilityNumberRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var rolesString = roles.Select(p => p.ToString());
            var allFacilities = requestFacilityRepository.TableNoTracking
                .Where(p => p.OrganizationId == leasingId)
                .Include(p => p.Buyer.Person)
                .Include(p => p.Buyer)
                    .ThenInclude(p => p.UserBankAccounts)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .AsSplitQuery();

            ApplyFilter(ref allFacilities, filter, roles);

            var waitingFacilities = allFacilities
              .Where(p => !p.CancelByUser &&
                     p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                     x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
              .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = allFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var query = executeForExport
                        ? (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false))
                        : (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .Skip((filter.Page - 1) * filter.PageSize)
                                                 .Take(filter.PageSize)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false));

            var filterList = query
                             .Select(p => new PendingInEnterFacilityNumberModel
                             {
                                 Id = p.item.Id,
                                 Amount = p.item.Amount,
                                 Requester = $"{p.item.Buyer.Person.FName} {p.item.Buyer.Person.LName}",
                                 RequesterFName = p.item.Buyer.Person.FName,
                                 RequesterLName = p.item.Buyer.Person.LName,
                                 NationalCode = p.item.Buyer.Person.NationalCode,
                                 AccountNumber = p.item.Buyer.UserBankAccounts.Any(p => p.IsConfirm) ? p.item.Buyer.UserBankAccounts.Where(p => p.IsConfirm).First().Deposit! : string.Empty,
                                 CustomerNumber = p.item.Buyer.Person.CustomerNumber!
                             }).ToList();


            if (!executeForExport)
            {
                var totalRowCounts = await allFacilities.CountAsync();

                return new PagingDto<PendingInEnterFacilityNumberModel>()
                {
                    CurrentPage = filter.Page,
                    Data = filterList,
                    TotalRowCount = totalRowCounts,
                    TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
                };
            }
            else
            {
                return new PagingDto<PendingInEnterFacilityNumberModel>()
                {
                    Data = filterList
                };
            }
        }
        public async Task<PagingDto<PendingInEnterPoliceNumberModel>> SearchPendingInEnterPoliceNumberRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var rolesString = roles.Select(p => p.ToString());
            var allFacilities = requestFacilityRepository.TableNoTracking
                .Where(p => p.OrganizationId == leasingId)
                .Include(p => p.Buyer.Person)
                .Include(p => p.Buyer)
                    .ThenInclude(p => p.UserBankAccounts)
                .Include(p => p.RequestFacilityPromissories)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .AsSplitQuery();

            ApplyFilter(ref allFacilities, filter, roles);

            var waitingFacilities = allFacilities
              .Where(p => !p.CancelByUser &&
                     p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                     x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
              .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = allFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var query = executeForExport
                        ? (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false))
                        : (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .Skip((filter.Page - 1) * filter.PageSize)
                                                 .Take(filter.PageSize)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false));

            var filterList = query
                             .Select(p => new PendingInEnterPoliceNumberModel
                             {
                                 Id = p.item.Id,
                                 Amount = p.item.Amount,
                                 Requester = $"{p.item.Buyer.Person.FName} {p.item.Buyer.Person.LName}",
                                 RequesterFName = p.item.Buyer.Person.FName,
                                 RequesterLName = p.item.Buyer.Person.LName,
                                 NationalCode = p.item.Buyer.Person.NationalCode,
                                 AccountNumber = p.item.Buyer.UserBankAccounts.Any(p => p.IsConfirm) ? p.item.Buyer.UserBankAccounts.Where(p => p.IsConfirm).First().Deposit! : string.Empty,
                                 PromissoryNumber = p.item.RequestFacilityPromissories.Any(p => p.IsActive && p.MultiSignedPdf != null && p.PromissoryId != null)
                                    ? p.item.RequestFacilityPromissories.Where(p => p.IsActive && p.MultiSignedPdf != null && p.PromissoryId != null).First().PromissoryId! :
                                    string.Empty,
                                 CustomerNumber = p.item.Buyer.Person.CustomerNumber!,
                                 FacilityNumber = p.item.FacilityNumber
                             }).ToList();


            if (!executeForExport)
            {
                var totalRowCounts = await allFacilities.CountAsync();

                return new PagingDto<PendingInEnterPoliceNumberModel>()
                {
                    CurrentPage = filter.Page,
                    Data = filterList,
                    TotalRowCount = totalRowCounts,
                    TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
                };
            }
            else
            {
                return new PagingDto<PendingInEnterPoliceNumberModel>()
                {
                    Data = filterList
                };
            }
        }
        public async Task<PagingDto<PendingInEnterPoliceNumberModel>> SearchPendingInDepositFacilityAmountRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var rolesString = roles.Select(p => p.ToString());
            var allFacilities = requestFacilityRepository.TableNoTracking
                .Where(p => p.OrganizationId == leasingId)
                .Include(p => p.Buyer.Person)
                .Include(p => p.Buyer)
                    .ThenInclude(p => p.UserBankAccounts)
                //.Include(p => p.RequestFacilityCardIssuance)
                .Include(p => p.RequestFacilityPromissories)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .AsSplitQuery();

            ApplyFilter(ref allFacilities, filter, roles);

            var waitingFacilities = allFacilities
              .Where(p => !p.CancelByUser &&
                     p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                     x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
              .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = allFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var query = executeForExport
                        ? (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false))
                        : (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .Skip((filter.Page - 1) * filter.PageSize)
                                                 .Take(filter.PageSize)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false));

            var filterList = query
                             .Select(p => new PendingInEnterPoliceNumberModel
                             {
                                 Id = p.item.Id,
                                 Amount = p.item.Amount,
                                 Requester = $"{p.item.Buyer.Person.FName} {p.item.Buyer.Person.LName}",
                                 RequesterFName = p.item.Buyer.Person.FName,
                                 RequesterLName = p.item.Buyer.Person.LName,
                                 NationalCode = p.item.Buyer.Person.NationalCode,
                                 AccountNumber = p.item.Buyer.UserBankAccounts.Any(p => p.IsConfirm) ? p.item.Buyer.UserBankAccounts.Where(p => p.IsConfirm).First().Deposit! : string.Empty,
                                 PromissoryNumber = p.item.RequestFacilityPromissories.Any(p => p.IsActive && p.MultiSignedPdf != null && p.PromissoryId != null)
                                    ? p.item.RequestFacilityPromissories.Where(p => p.IsActive && p.MultiSignedPdf != null && p.PromissoryId != null).First().PromissoryId! :
                                    string.Empty,
                                 CustomerNumber = p.item.Buyer.Person.CustomerNumber!,
                                 FacilityNumber = p.item.FacilityNumber!,
                                 PoliceNumber = p.item.PoliceNumber!
                             }).ToList();


            if (!executeForExport)
            {
                var totalRowCounts = await allFacilities.CountAsync();

                return new PagingDto<PendingInEnterPoliceNumberModel>()
                {
                    CurrentPage = filter.Page,
                    Data = filterList,
                    TotalRowCount = totalRowCounts,
                    TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
                };
            }
            else
            {
                return new PagingDto<PendingInEnterPoliceNumberModel>()
                {
                    Data = filterList
                };
            }
        }

        public async Task<PagingDto<PendingInCardRechargeModel>> SearchPendingInChargeCardRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var rolesString = roles.Select(p => p.ToString());
            var allFacilities = requestFacilityRepository.TableNoTracking
                .Where(p => p.OrganizationId == leasingId)
                .Include(p => p.Buyer.Person)
                //.Include(p => p.Buyer)
                //    .ThenInclude(p => p.UserBankAccounts)
                .Include(p => p.RequestFacilityCardIssuance)
                //.Include(p => p.RequestFacilityPromissories)
                .Include(p => p.RequestFacilityWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .AsSplitQuery();

            ApplyFilter(ref allFacilities, filter, roles);

            var waitingFacilities = allFacilities
              .Where(p => !p.CancelByUser &&
                     p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                     x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
              .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingFacilities.Select(p => p.item.Id).ToList();

            var otherFacilities = allFacilities.Where(p => !waitingRequestIDs.Contains(p.Id))
                                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityWorkFlowSteps.Max(x => x.Id) });

            var query = executeForExport
                        ? (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false))
                        : (await waitingFacilities.Union(otherFacilities)
                                                 .OrderBy(p => p.Order)
                                                 .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                                                 .Skip((filter.Page - 1) * filter.PageSize)
                                                 .Take(filter.PageSize)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false));

            var filterList = query
                             .Select(p => new PendingInCardRechargeModel
                             {
                                 Id = p.item.Id,
                                 Amount = p.item.Amount,
                                 Requester = $"{p.item.Buyer.Person.FName} {p.item.Buyer.Person.LName}",
                                 RequesterFName = p.item.Buyer.Person.FName,
                                 RequesterLName = p.item.Buyer.Person.LName,
                                 NationalCode = p.item.Buyer.Person.NationalCode,
                                 AccountNumber = p.item.RequestFacilityCardIssuance.AccountNumber,//p.item.Buyer.UserBankAccounts != null && p.item.Buyer.UserBankAccounts.Any() ? p.item.Buyer.UserBankAccounts.First().Deposit! : string.Empty,
                                 CardNumber = p.item.RequestFacilityCardIssuance.CardNumber,
                                 //PromissoryNumber = p.item.RequestFacilityPromissories.Any(p => p.IsActive && p.MultiSignedPdf != null && p.PromissoryId != null)
                                 //   ? p.item.RequestFacilityPromissories.Where(p => p.IsActive && p.MultiSignedPdf != null && p.PromissoryId != null).First().PromissoryId! :
                                 //   string.Empty,
                                 CustomerNumber = p.item.Buyer.Person.CustomerNumber!,
                                 FacilityNumber = p.item.FacilityNumber!,
                                 //PoliceNumber = p.item.PoliceNumber!
                             }).ToList();


            if (!executeForExport)
            {
                var totalRowCounts = await allFacilities.CountAsync();

                return new PagingDto<PendingInCardRechargeModel>()
                {
                    CurrentPage = filter.Page,
                    Data = filterList,
                    TotalRowCount = totalRowCounts,
                    TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
                };
            }
            else
            {
                return new PagingDto<PendingInCardRechargeModel>()
                {
                    Data = filterList
                };
            }
        }
        private IQueryable<RequestFacility> ApplyFilter(ref IQueryable<RequestFacility> requestFacilities, PagingFilterDto filter, List<RoleEnum> roles)
        {
            if (filter != null && filter.FilterList != null)
            {
                foreach (var item in filter.FilterList)
                {
                    switch (item.PropertyName)
                    {
                        //case "CheckedRequestFacilityIds":
                        //    {
                        //        List<int> propertyValue = ((Newtonsoft.Json.Linq.JArray)item.PropertyValue).Values<int>().ToList();
                        //        if (propertyValue.Any())
                        //            requestFacilities = requestFacilities.Where(p => propertyValue.Contains(p.Id));
                        //        break;
                        //    }
                        case "OrganizationId":
                            {
                                int organizationId = item.PropertyValue;
                                requestFacilities = requestFacilities.Where(p => p.OrganizationId == organizationId);
                                break;
                            }
                        case "OperatorId":
                            {
                                Guid operatorId = new Guid(item.PropertyValue);
                                requestFacilities = requestFacilities.Where(p => p.OperatorId == operatorId);
                                break;
                            }
                        case "SearchOperatorId":
                            {
                                if (!string.IsNullOrWhiteSpace(item.PropertyValue))
                                {
                                    Guid searchOperatorId = new Guid(item.PropertyValue);
                                    requestFacilities = requestFacilities.Where(p => p.OperatorId == searchOperatorId);
                                }
                                break;
                            }
                        case "FName":
                            {
                                string propertyValue = item.PropertyValue;
                                propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                                if (!string.IsNullOrEmpty(propertyValue))
                                    requestFacilities = requestFacilities.Where(p => p.Buyer.Person.FName.Replace(" ", string.Empty).Contains(propertyValue));
                                break;
                            }
                        case "LName":
                            {
                                string propertyValue = item.PropertyValue;
                                propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                                if (!string.IsNullOrEmpty(propertyValue))
                                    requestFacilities = requestFacilities.Where(p => p.Buyer.Person.LName.Replace(" ", string.Empty).Contains(propertyValue));
                                break;
                            }
                        case "NationalCode":
                            {
                                string propertyValue = item.PropertyValue;
                                requestFacilities = requestFacilities.Where(p => p.Buyer.Person.NationalCode.Contains(propertyValue));
                                break;
                            }
                        case "WaitingStepId":
                            {
                                int propertyValue = Convert.ToInt32(item.PropertyValue);
                                requestFacilities = requestFacilities.Where(p => //!p.CancelByUser &&
                                    p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStepId == propertyValue));
                                break;
                            }
                        case "StartDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                requestFacilities = requestFacilities.Where(p => p.CreatedDate.Date >= propertyValue.Date);
                                break;
                            }
                        case "EndDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                requestFacilities = requestFacilities.Where(p => p.CreatedDate.Date <= propertyValue.Date);
                                break;
                            }
                        case "PaymentFacilityStartDate":
                            {
                                var cardChargeWorkFlowStepId = 100025;
                                var cardChargeCompletionWorkFlowStepId = 1000251;
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                requestFacilities = requestFacilities
                                    .Where(p => p.RequestFacilityWorkFlowSteps.OrderBy(o => o.Id).Last().CreatedDate.Date >= propertyValue.Date
                                        && (p.RequestFacilityWorkFlowSteps.OrderBy(o => o.Id).Last().WorkFlowStepId == cardChargeWorkFlowStepId
                                        || p.RequestFacilityWorkFlowSteps.OrderBy(o => o.Id).Last().WorkFlowStepId == cardChargeCompletionWorkFlowStepId));
                                break;
                            }
                        case "PaymentFacilityEndDate":
                            {
                                var cardChargeWorkFlowStepId = 100025;
                                var cardChargeCompletionWorkFlowStepId = 1000251;
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                requestFacilities = requestFacilities
                                    .Where(p => p.RequestFacilityWorkFlowSteps.OrderBy(o => o.Id).Last().CreatedDate.Date <= propertyValue.Date
                                        && (p.RequestFacilityWorkFlowSteps.OrderBy(o => o.Id).Last().WorkFlowStepId == cardChargeWorkFlowStepId
                                        || p.RequestFacilityWorkFlowSteps.OrderBy(o => o.Id).Last().WorkFlowStepId == cardChargeCompletionWorkFlowStepId));
                                break;
                            }
                        case "StartAmount":
                            {
                                long propertyValue = item.PropertyValue;
                                requestFacilities = requestFacilities.Where(p => p.Amount >= propertyValue);
                                break;
                            }
                        case "EndAmount":
                            {
                                long propertyValue = item.PropertyValue;
                                requestFacilities = requestFacilities.Where(p => p.Amount <= propertyValue);
                                break;
                            }
                        case "FacilityStatus":
                            {
                                long propertyValue = item.PropertyValue;
                                switch (propertyValue)
                                {
                                    case (long)FacilityStatus.WaitingRequest://Waiting Request Facilities for Roles
                                        {
                                            if (roles == null) break;

                                            if (roles.Any(p => p == RoleEnum.SupervisorLeasing || p == RoleEnum.BankLeasing || p == RoleEnum.Buyer ||
                                                               p == RoleEnum.SuperAdmin || p == RoleEnum.Admin || p == RoleEnum.ZarinLendExpert))
                                            {
                                                var rolesString = roles != null ? roles.Select(p => p.ToString()).ToList() : Enumerable.Empty<string>();
                                                requestFacilities = requestFacilities.Where(p => !p.CancelByUser &&
                                                    p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                                                    (!rolesString.Any() || x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(x => x == c.Role.Name)))));
                                            }
                                            else if (roles.Any(p => p == RoleEnum.AdminBankLeasing))
                                            {
                                                var rolesString = roles != null ? roles.Select(p => p.ToString()) : Enumerable.Empty<string>();
                                                requestFacilities = requestFacilities.Where(p => !p.CancelByUser &&
                                                    (p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue &&
                                                                                             x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                                                                                             (!rolesString.Any() || x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name)))) ||
                                                    (p.RequestFacilityWorkFlowSteps.Any(x => x.StatusId.HasValue &&
                                                                                             x.StatusId == (short)StatusEnum.Approved &&
                                                                                             x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature) &&
                                                                                             (p.SignedContractByBankFileName == null || p.SignedContractByBankFileName == string.Empty))));
                                            }
                                            break;
                                        }
                                    case (long)FacilityStatus.JustShowWaitingSignatureRequest:
                                        {
                                            var rolesString = roles != null ? roles.Select(p => p.ToString()) : Enumerable.Empty<string>();
                                            requestFacilities = requestFacilities.Where(p => !p.CancelByUser &&
                                                    (p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue &&
                                                                                             x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                                                                                             x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature &&
                                                                                             (!rolesString.Any() || x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name)))) ||
                                                    (p.RequestFacilityWorkFlowSteps.Any(x => x.StatusId.HasValue &&
                                                                                             x.StatusId == (short)StatusEnum.Approved &&
                                                                                             x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature) &&
                                                                                             (p.SignedContractByBankFileName == null || p.SignedContractByBankFileName == string.Empty))));
                                        }
                                        break;
                                    case (long)FacilityStatus.ApprovedRequest://Approve Request Facilities
                                        {
                                            requestFacilities = requestFacilities.Where(p => !p.CancelByUser &&
                                            p.RequestFacilityWorkFlowSteps
                                                                        .Any(x => x.StatusId.HasValue &&
                                                                                  x.WorkFlowStepId == 100025 || x.WorkFlowStepId == 1000251 &&
                                                                                  x.StatusId.Value == (short)StatusEnum.Approved));
                                            break;
                                        }
                                    case (long)FacilityStatus.RejectCancelRequest://Close/Reject Request Facilities
                                        {
                                            requestFacilities = requestFacilities.Where(p => p.CancelByUser ||
                                                p.RequestFacilityWorkFlowSteps
                                                                        .Any(x => !x.WorkFlowStep.IsApproveFinalStep &&
                                                                                  x.WorkFlowStep.IsLastStep));
                                            break;
                                        }
                                    case (long)FacilityStatus.OpenRequest://Open Request Facilities
                                        {
                                            requestFacilities = requestFacilities.Where(p => !p.CancelByUser &&
                                                                                              p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue));
                                            break;
                                        }
                                    case (long)FacilityStatus.OpenAndNotSetOperator://Open Request Facilities and Not set Operator
                                        {
                                            requestFacilities = requestFacilities.Where(p => !p.CancelByUser &&
                                                                                             !p.OperatorId.HasValue &&
                                                                                              p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue));
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            return requestFacilities;
        }
        public async Task<List<CompletedWorkFlowStepModel>> GetRequestFacilitySteps(int requestFacilityId, CancellationToken cancellationToken)
        {
            return (await requestFacilityWorkFlowStepRepository.SelectByAsync(p => p.RequestFacilityId == requestFacilityId,
                p => new CompletedWorkFlowStepModel()
                {
                    WorkFlowStepId = p.WorkFlowStepId,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    StatusId = p.StatusId
                }, cancellationToken))
                .ToList();
        }
        public async Task<List<int>> GetAllRequestFacilityIdsForSign(int leasingId, PagingFilterDto filter, List<int> checkedRequestFacilityIds, List<RoleEnum> roles, CancellationToken cancellationToken)
        {
            var organizationFacilities = requestFacilityRepository.TableNoTracking.Where(p => p.OrganizationId.Equals(leasingId));
            if (checkedRequestFacilityIds != null && checkedRequestFacilityIds.Any())
                organizationFacilities = organizationFacilities.Where(p => checkedRequestFacilityIds.Any(x => x == p.Id));

            var rolesString = roles.Select(p => p.ToString());
            ApplyFilter(ref organizationFacilities, filter, roles);
            return await organizationFacilities
                .Where(p => p.RequestFacilityWorkFlowSteps
                            .Any(x => !x.StatusId.HasValue &&
                                      x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature &&
                                      x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(d => d == c.Role.Name))) ||
                       (p.RequestFacilityWorkFlowSteps
                            .Any(x => x.StatusId.HasValue &&
                                      x.StatusId == (short)StatusEnum.Approved &&
                                      x.WorkFlowStep.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature &&
                                      x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(d => d == c.Role.Name))) &&
                                      (p.SignedContractByBankFileName == null || p.SignedContractByBankFileName == string.Empty)))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);
        }
        public async Task<PersonModel?> GetPersonInfo(int requestFacilityId, CancellationToken cancellationToken)
        {
            return (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId),
                    selector: p => new PersonModel()
                    {
                        NationalCode = p.Buyer.Person.NationalCode,
                        FName = p.Buyer.Person.FName,
                        LName = p.Buyer.Person.LName
                    }, cancellationToken))
                    .FirstOrDefault();
        }
        public async Task<PersonCompleteInfoModel?> GetCompletePersonInfo(int requestFacilityId, CancellationToken cancellationToken)
        {
            return (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId),
                     p => new PersonCompleteInfoModel()
                     {
                         FName = p.Buyer.Person.FName,
                         LName = p.Buyer.Person.LName,
                         FatherName = p.Buyer.Person.FatherName,
                         NationalCode = p.Buyer.Person.NationalCode,
                         SSID = p.Buyer.Person.SSID,
                         BirthDate = p.Buyer.Person.BirthDate,
                         //UserType = p.Buyer.Person.OrganizationId.HasValue ? p.Buyer.Person.Organization.OrganizationType.Name : "حقیقی",
                         PostalCode = p.Buyer.Person.PostalCode,
                         Gender = p.Buyer.Person.Gender,
                         Nationality = p.Buyer.Person.Country.Name,
                         Address = p.Buyer.Person.Address,
                         Mobile = p.Buyer.Person.Mobile,
                         //ProvinceOfBirth = p.Buyer.Person.BirthLocation.Parent.Name,
                         //CityOfBirth = p.Buyer.Person.BirthLocation.Name,
                         AddressProvince = p.Buyer.Person.City.Parent.Name,
                         AddressCity = p.Buyer.Person.City.Name,
                         PhoneNumber = p.Buyer.PhoneNumber,
                         Email = p.Buyer.Email,
                         FacilityNumber = p.FacilityNumber
                     }, cancellationToken))
                .FirstOrDefault();
        }
        public async Task<RequestFacilityInfoContractModel?> GetRequestFacilityInfoForContract(int requestFacilityId, CancellationToken cancellationToken)
        {
            return (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId),
                     p => new RequestFacilityInfoContractModel()
                     {
                         FName = p.Buyer.Person.FName,
                         LName = p.Buyer.Person.LName,
                         FatherName = p.Buyer.Person.FatherName,
                         NationalCode = p.Buyer.Person.NationalCode,
                         SSID = p.Buyer.Person.SSID,
                         IdentificationSerial = p.Buyer.Person.IdentificationSerial ?? string.Empty,
                         BirthDate = p.Buyer.Person.BirthDate,
                         //UserType = p.Buyer.Person.OrganizationId.HasValue ? p.Buyer.Person.Organization.OrganizationType.Name : "حقیقی",
                         PostalCode = p.Buyer.Person.PostalCode,
                         Gender = p.Buyer.Person.Gender,
                         Nationality = p.Buyer.Person.Country.Name,
                         Address = p.Buyer.Person.Address,
                         Mobile = p.Buyer.Person.Mobile,
                         //ProvinceOfBirth = p.Buyer.Person.BirthLocation.Parent.Name,
                         //CityOfBirth = p.Buyer.Person.BirthLocation.Name,
                         CityOfIssue = p.Buyer.Person.PlaceOfBirth ?? string.Empty,
                         AddressProvince = p.Buyer.Person.City.Parent.Name,
                         AddressCity = p.Buyer.Person.City.Name,
                         PhoneNumber = p.Buyer.PhoneNumber,
                         Email = p.Buyer.Email,
                         FacilityNumber = p.FacilityNumber!,
                         OrganizationAddress = p.Organization.Address,
                         OrganizationAgent = "اسماعیل هاشم پور",
                         OrganizationBranch = "خدمات نوین",
                         OrganizationCode = "7000",
                         OrganizationName = "بانک آینده سهامی عام",
                         ContractSubject = "طرح اعتباری زرین لند",
                         Amount = p.Amount,
                         Month = p.FacilityType.MonthCount,
                         FacilityInterest = p.GlobalSetting.FacilityInterest,
                         WarantyPercentage = p.GlobalSetting.WarantyPercentage,
                         CustomerNumber = p.Buyer.Person.CustomerNumber,
                         SafteNumber = p.RequestFacilityPromissories.Any(p => p.IsActive && !string.IsNullOrEmpty(p.MultiSignedPdf) && !string.IsNullOrEmpty(p.PromissoryId))
                            ? p.RequestFacilityPromissories.FirstOrDefault(p => p.IsActive && !string.IsNullOrEmpty(p.MultiSignedPdf) && !string.IsNullOrEmpty(p.PromissoryId))!.PromissoryId!
                            : string.Empty
                     }, cancellationToken))
                .FirstOrDefault();
        }
        public async Task<int?> GetRequestFacilityId(string nationalCode, int orgaizationId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum,
            CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var result = await requestFacilityRepository
                .TableNoTracking
                .Where(p => p.Buyer.Person.NationalCode.Equals(nationalCode) &&
                       p.OrganizationId == orgaizationId &&
                       !p.CancelByUser &&
                       p.RequestFacilityWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))))
                .Select(p => p.Id)
                .ToListAsync();

            if (result.Any())
                return result[0];

            return null;
        }
        public async Task<List<PersonCompleteInfoModel>> GetPersonListWaitngInSpecialStep(PagingFilterDto filter, int organizationId, List<RoleEnum> roles, CancellationToken cancellationToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var rolesString = roles.Select(p => p.ToString());
            var organizationFacilities = requestFacilityRepository.TableNoTracking.Where(p => p.OrganizationId.Equals(organizationId));
            ApplyFilter(ref organizationFacilities, filter, roles);

            return await organizationFacilities.Select(
                p => new PersonCompleteInfoModel()
                {
                    RequestFacilityId = p.Id,
                    FName = p.Buyer.Person.FName,
                    LName = p.Buyer.Person.LName,
                    FatherName = p.Buyer.Person.FatherName,
                    NationalCode = p.Buyer.Person.NationalCode,
                    SSID = p.Buyer.Person.SSID,
                    BirthDate = p.Buyer.Person.BirthDate,
                    UserType = p.Buyer.Person.OrganizationId.HasValue ? p.Buyer.Person.Organization.OrganizationType.Name : "حقیقی",
                    PostalCode = p.Buyer.Person.PostalCode,
                    Gender = p.Buyer.Person.Gender,
                    Nationality = p.Buyer.Person.Country.Name,
                    Address = p.Buyer.Person.Address,
                    Mobile = p.Buyer.Person.Mobile,
                    //ProvinceOfBirth = p.Buyer.Person.BirthLocation.Parent.Name,
                    //CityOfBirth = p.Buyer.Person.BirthLocation.Name,
                    AddressProvince = p.Buyer.Person.City.Parent.Name,
                    AddressCity = p.Buyer.Person.City.Name,
                    PhoneNumber = p.Buyer.PhoneNumber,
                    CustomerNumber = p.Buyer.Person.CustomerNumber,

                }).ToListAsync(cancellationToken);
        }
        public async Task<List<SelectListItem>> SelectApprovalFacility(Guid userId, CancellationToken cancellationToken)
        {
            return (await requestFacilityRepository.SelectByAsync(p => p.BuyerId.Equals(userId) &&
                                                                       !p.CancelByUser &&
                                                                       !p.DoneFacility &&
                                                                       p.RequestFacilityWorkFlowSteps
                                                                            .Any(x => x.WorkFlowStep.IsApproveFinalStep &&
                                                                                      x.WorkFlowStep.IsLastStep &&
                                                                                      x.StatusId.HasValue &&
                                                                                      x.StatusId.Value == (short)StatusEnum.Approved),
                    p => new SelectListItem
                    {
                        //Text = $"شماره تسهیلات : {p.FacilityNumber} - وام {p.Amount.ToString("N0")} ريالی - {p.FacilityType.MonthCountTitle}",
                        Text = $"تسهیلات : {p.FacilityNumber} - {p.Amount:N0} ريال",
                        Value = p.Id.ToString(),
                    }, cancellationToken))
                    .ToList();
        }
        public async Task<RequestFacilityFilterModel> PrepareFilterModelForSearch(List<RoleEnum> roles, CancellationToken cancellationToken)
        {
            var model = new RequestFacilityFilterModel();
            if (roles != null && roles.Any())
            {
                var rolesString = roles.Select(p => p.ToString());
                //model.WorkFlowSteps = (await workFlowStepRepository
                //    .SelectByAsync(p => p.StepIsManual && p.WorkFlowStepRoles.Any(x => rolesString.Any(c => c == x.Role.Name)),
                //                       p => new SelectListItem
                //                       {
                //                           Value = p.Id.ToString(),
                //                           Text = p.Name
                //                       }, cancellationToken))
                //                       .ToList();
                model.WorkFlowSteps = (await workFlowStepRepository
                    .SelectByAsync(p => !p.IsFirstStep && p.StepIsManual && p.WorkFlowId == (int)WorkFlowEnum.RequestFacility,
                    p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    }, cancellationToken))
                    .ToList();
            }
            else
            {
                model.WorkFlowSteps = (await workFlowStepRepository
                    .SelectByAsync(p => !p.IsFirstStep && p.WorkFlowId == (int)WorkFlowEnum.RequestFacility,
                    p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    }, cancellationToken))
                    .ToList();
            }

            return model;
        }
    }
}
