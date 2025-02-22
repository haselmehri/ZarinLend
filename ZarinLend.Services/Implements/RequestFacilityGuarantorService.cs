using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class RequestFacilityGuarantorService : IRequestFacilityGuarantorService, IScopedDependency
    {
        private readonly ILogger<RequestFacilityGuarantorService> logger;
        private readonly IBaseRepository<RequestFacilityGuarantor> requestFacilityGuarantorRepository;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository;
        private readonly IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository;
        private readonly IBaseRepository<IranCreditScoring> iranCreditScoringRepository;
        private readonly IUserService userService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly IWorkFlowStepRepository workFlowStepRepository;

        public RequestFacilityGuarantorService(ILogger<RequestFacilityGuarantorService> logger,
            IBaseRepository<RequestFacilityGuarantor> requestFacilityGuarantorRepository,
            IRequestFacilityService requestFacilityService,
            IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository,
            IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository,
            IBaseRepository<IranCreditScoring> iranCreditScoringRepository,
            IUserService userService,
            IGlobalSettingService globalSettingService,
        IWorkFlowStepRepository workFlowStepRepository)
        {
            this.logger = logger;
            this.requestFacilityGuarantorRepository = requestFacilityGuarantorRepository;
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityGuarantorWorkFlowStepRepository = requestFacilityGuarantorWorkFlowStepRepository;
            this.iranCreditScoringResultRuleRepository = iranCreditScoringResultRuleRepository;
            this.iranCreditScoringRepository = iranCreditScoringRepository;
            this.userService = userService;
            this.globalSettingService = globalSettingService;
            this.workFlowStepRepository = workFlowStepRepository;
        }

        public async Task<int> RegisterGuarantor(Guid guarantorUserId, int requestFacilityId, CancellationToken cancellationToken)
        {
            //if (await UserHasOpenRequest(requestFacilityModel.BuyerId, cancellationToken))
            //    throw new AppException(@"کاربر گرامی شما در حال حاضر درخواست فعالی دارید که در لیست درخواست ها میتوانید اقدام به تکمیل آن نمایید
            //                             شما نمیتوانید بیش از یک درخواست تسهیلات(وام) فعال بصورت همزمان داشته باشید برای ثبت درخواست جدید یا باید منتظر مشخص شدن وضعیت نهایی درخواست(تایید نهایی / رد درخواست) باشید یا نسبت به 'لغو درخواست' اقدام نمایید");

            //var sumAmountApprovedFacilitiesNotPaid = await GetUserSumAmountApprovedFacilities(requestFacilityModel.BuyerId, cancellationToken);
            //if (requestFacilityModel.AmountRequest > (500000000 - sumAmountApprovedFacilitiesNotPaid))
            //    throw new AppException($@"کاربر گرامی مجموع وام های دریافتی و تسویه نشده شما تا این لحظه {sumAmountApprovedFacilitiesNotPaid:N0} ريال می باشد،شما در مجموع می توانید مبلغ {500000000:N0} ريال تسهیلات فعال تسویه نشده داشته باشید");

            var firstStep = await workFlowStepRepository.GetFirstStep(WorkFlowEnum.RegisterGuarantor, new List<RoleEnum> { RoleEnum.Buyer }, cancellationToken);
            if (firstStep == null)
                throw new AppException("First Step to WorkFlow to ID=1 is NULL");

            var activeGlobaSetting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            if (activeGlobaSetting == null)
                throw new AppException("تنظیمات مربوط به تسهیلات یافت نشد!");

            var lastUserRisk = await iranCreditScoringRepository.TableNoTracking
                .Where(p => p.CreatorId.Equals(guarantorUserId) &&
                            p.CreatedDate.AddDays(activeGlobaSetting.ValidityPeriodOfValidation).Date >= DateTime.Now.Date)
                .Select(p => p.Risk)
                .FirstOrDefaultAsync(cancellationToken);

            var rulesBaseUserVerifyResult = await iranCreditScoringResultRuleRepository
                .TableNoTracking
                .Where(p => p.IsActive &&
                            ((lastUserRisk != string.Empty && lastUserRisk.Contains(p.Risk) && p.Risk != string.Empty) ||
                            (lastUserRisk == string.Empty && p.Risk == string.Empty)) &&
                            p.IranCreditScoringResultRuleType == IranCreditScoringResultRuleType.ForRequestFacilityGuarantor)
                .Select(p => new
                {
                    p.Id,
                    p.Risk
                })
                .ToListAsync(cancellationToken);

            #region According to the validation result, we check that the user has met the conditions
            if (!rulesBaseUserVerifyResult.Any())
                throw new AppException("کاربر گرامی متاسفانه شما شرایط ثبت درخواست بعنوان ضامن را ندارید!");

            #endregion According to the validation result, we check that the user has met the conditions

            var requestFacilityGuarantor = new RequestFacilityGuarantor()
            {
                GuarantorUserId = guarantorUserId,
                RequestFacilityId = requestFacilityId,
                IranCreditScoringResultRuleId = rulesBaseUserVerifyResult.FirstOrDefault().Id,
                RequestFacilityGuarantorWorkFlowSteps = new List<RequestFacilityGuarantorWorkFlowStep>()
                {
                    new RequestFacilityGuarantorWorkFlowStep()
                    {
                        OpratorId = guarantorUserId,
                        StatusId = (short)StatusEnum.Approved,
                        StatusDescription ="این مرحله بعد از ثبت درخواست ضامن،بصورت اتوماتیک توسط سیستم ایجاد و تایید شد",
                        WorkFlowStepId = firstStep.Id,
                    },
                     new RequestFacilityGuarantorWorkFlowStep()
                    {
                        OpratorId = guarantorUserId,
                        StatusDescription ="این مرحله بعد از ثبت درخواست ضامن،بصورت اتوماتیک توسط سیستم ایجاد شد",
                        WorkFlowStepId = firstStep.ApproveNextStepId.Value,
                    }
                },
                //RequestFaciliyUsagePlaces = usagePlaces
            };
            await requestFacilityGuarantorRepository.AddAsync(requestFacilityGuarantor, cancellationToken, true);
            return requestFacilityGuarantor.Id;
        }

        public async Task<RequestFacilityGuarantorAddModel> PrepareModelForAdd(string userRisk, CancellationToken cancellationToken)
        {
            //todo check other rules such as this Guarantor is as Guarantor for others.... 
            var model = new RequestFacilityGuarantorAddModel();

            //model.IranCreditScoringResultRules = await iranCreditScoringResultRuleRepository
            //    .TableNoTracking
            //    .Where(p => p.IsActive &&
            //                p.IranCreditScoringResultRuleType == IranCreditScoringResultRuleType.ForRequestFacilityGuarantor &&
            //                ((userRisk != string.Empty && userRisk.Contains(p.Risk) && p.Risk != string.Empty) ||
            //                (userRisk == string.Empty && p.Risk == string.Empty)))
            //    .Select(p => new IranCreditScoringResultRuleModel
            //    {
            //        GuarantorIsRequired = p.GuarantorIsRequired,
            //        MaximumAmount = p.MaximumAmount,
            //        MinimumAmount = p.MinimumAmount,
            //        Risk = p.Risk
            //    })
            //    .ToListAsync(cancellationToken);

            model.IranCreditScoringResultRules = await iranCreditScoringResultRuleRepository
                .TableNoTracking
                .Where(p => p.IsActive &&
                            p.IranCreditScoringResultRuleType == IranCreditScoringResultRuleType.ForRequestFacilityGuarantor)
                .Select(p => new IranCreditScoringResultRuleModel
                {
                    //GuarantorIsRequired = p.GuarantorIsRequired,
                    //MaximumAmount = p.MaximumAmount,
                    //MinimumAmount = p.MinimumAmount,
                    Risk = p.Risk
                })
                .ToListAsync(cancellationToken);

            return model;
        }
        public async Task<List<CompletedWorkFlowStepModel>> GetRequestFacilityGuarantorSteps(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            return (await requestFacilityGuarantorWorkFlowStepRepository.SelectByAsync(p => p.RequestFacilityGuarantorId == requestFacilityGuarantorId,
                p => new CompletedWorkFlowStepModel()
                {
                    WorkFlowStepId = p.WorkFlowStepId,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    StatusId = p.StatusId
                }, cancellationToken))
                .ToList();
        }

        public async Task<bool> HasGaurantorWithApprovedWorkFlow(int requestFacilityId, CancellationToken cancellationToken)
        {
            return await requestFacilityGuarantorRepository.TableNoTracking
                .AnyAsync(p => p.RequestFacilityId == requestFacilityId &&
                               p.RequestFacility.GuarantorIsRequired &&
                               !p.CancelByUser &&
                               p.RequestFacilityGuarantorWorkFlowSteps.Any(c => c.WorkFlowStep.IsApproveFinalStep &&
                                                                                c.WorkFlowStep.IsLastStep &&
                                                                                c.StatusId == (short)StatusEnum.Approved), cancellationToken);
        }
        public async Task<RequestFacilityGuarantorInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityGuarantorId, WorkFlowFormEnum workFlowForm, CancellationToken cancellationToken)
        {

            var model = await GetRequestFacilityCompleteInfo(requestFacilityGuarantorId, cancellationToken: cancellationToken);
            model.WaitingForZarinLend = await CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityGuarantorId,
                                                                                              new List<RoleEnum> { RoleEnum.Admin, RoleEnum.SuperAdmin },
                                                                                              workFlowForm,
                                                                                              cancellationToken);
            model.CurrentStepForm = workFlowForm;

            return model;
        }

        public async Task<RequestFacilityGuarantorInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityGuarantorId, Guid? guarantorUserId = null, CancellationToken cancellationToken = default)
        {
            var model = (await requestFacilityGuarantorRepository.SelectByAsync(p => p.Id.Equals(requestFacilityGuarantorId) && (!guarantorUserId.HasValue || p.GuarantorUserId == guarantorUserId.Value),
                x => new
                {
                    x.RequestFacility.GlobalSetting.FinancialInstitutionFacilityFee,
                    x.RequestFacility.GlobalSetting.LendTechFacilityFee,
                    x.RequestFacility.GlobalSetting.WarantyPercentage,
                    x.RequestFacility.GlobalSetting.FacilityInterest,
                    x.RequestFacility.FacilityType.MonthCountTitle,
                    x.RequestFacility.FacilityType.MonthCount,
                    x.RequestFacility.Amount,
                    x.CancelByUser,
                    //x.OperatorId,
                    x.CreatedDate,
                    x.GuarantorUserId,
                    x.Guarantor.UserName,
                    x.Guarantor.Person.FName,
                    x.Guarantor.Person.LName,
                    x.Guarantor.Person.FatherName,
                    x.Guarantor.Person.BirthDate,
                    x.Guarantor.Person.PostalCode,
                    x.Guarantor.Person.Address,
                    x.Guarantor.Person.Mobile,
                    x.Guarantor.UserRoles,
                    PersonId = x.Guarantor.Person.Id,
                    AddressCity = x.Guarantor.Person.City.Name,
                    AddressProvince = x.Guarantor.Person.City.Parent.Name,
                    x.Guarantor.Email,
                    x.Guarantor.Person.NationalCode,
                    x.Guarantor.Person.SSID,
                    x.Guarantor.PhoneNumber,
                    IdentityDocuments = x.Guarantor.UserIdentityDocuments.Any()
                        ? x.Guarantor.UserIdentityDocuments.Where(p => !p.IsDeleted)
                                                           .Select(p =>
                                                                    new
                                                                    {
                                                                        p.FilePath,
                                                                        p.DocumentType,
                                                                        p.Version,
                                                                        p.CreatedDate
                                                                    })
                        : null,
                    RequestFacilityGuarantorWorkFlowSteps = x.RequestFacilityGuarantorWorkFlowSteps.Any()
                    ? x.RequestFacilityGuarantorWorkFlowSteps.Select(p => new
                    {
                        WorkFlowStepName = p.WorkFlowStep.Name,
                        p.WorkFlowStep.IsLastStep,
                        p.WorkFlowStep.IsApproveFinalStep,
                        p.StatusId,
                        p.CreatedDate
                    })
                    : null
                }, cancellationToken))
                .Select(p => new RequestFacilityGuarantorInfoModel()
                {
                    Id = requestFacilityGuarantorId,
                    UserId = p.GuarantorUserId,
                    //OperatorId = p.OperatorId,
                    RequestFacilityDetail = new RequestFacilityDetailModel()
                    {
                        FacilityInterest = p.FacilityInterest,
                        FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                        LendTechFacilityFee = p.LendTechFacilityFee,
                        WarantyPercentage = p.WarantyPercentage,
                        Amount = p.Amount,
                        MonthCount = p.MonthCount,
                        MonthCountTitle = p.MonthCountTitle,
                        CancelByUser = p.CancelByUser,
                        CreateDate = p.CreatedDate,
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
                        BirthDate = p.BirthDate,
                        PhoneNumber = p.PhoneNumber,
                        PostalCode = p.PostalCode,
                        AddressCityName = p.AddressCity,
                        AddressProvinceName = p.AddressProvince,
                        UserName = p.UserName,
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

                    RequestFacilityGuarantorWorkFlowStepList = p.RequestFacilityGuarantorWorkFlowSteps != null
                        ? p.RequestFacilityGuarantorWorkFlowSteps.Select(x => new WorkFlowStepListModel()
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

            if (model == default(RequestFacilityGuarantorInfoModel))
                throw new NotFoundException();

            return model;
        }
        public async Task CancelByUser(int requestFacilityGuarantorId, Guid userId, CancellationToken cancellationToken)
        {
            var requestFacility = await requestFacilityGuarantorRepository.GetByConditionAsync(p => p.Id.Equals(requestFacilityGuarantorId) && p.GuarantorUserId.Equals(userId), cancellationToken);
            if (requestFacility == null)
                throw new LogicException($"درخواست یافت نشد!", new { requestFacilityGuarantorId, userId });

            if (requestFacility.CancelByUser)
                throw new AppException("درخواست قبلا بسته شده است!");

            if (await requestFacilityGuarantorWorkFlowStepRepository.TableNoTracking
                 .AnyAsync(p => p.RequestFacilityGuarantorId.Equals(requestFacilityGuarantorId) &&
                                p.WorkFlowStep.IsApproveFinalStep &&
                                p.WorkFlowStep.IsLastStep &&
                                p.StatusId.HasValue &&
                                p.StatusId.Value == (short)StatusEnum.Approved))
                throw new LogicException("درخواست تایید نهایی شده است،امکان 'انصراف درخواست' وجود ندارد");

            if (await requestFacilityGuarantorWorkFlowStepRepository.TableNoTracking
                .AnyAsync(p => p.RequestFacilityGuarantorId.Equals(requestFacilityGuarantorId) &&
                               p.WorkFlowStep.IsLastStep &&
                               p.StatusId.HasValue))
                throw new LogicException("درخواست رد شده شده است،امکان 'انصراف درخواست' وجود ندارد");

            await requestFacilityGuarantorRepository.UpdateCustomPropertiesAsync(new RequestFacilityGuarantor()
            {
                Id = requestFacilityGuarantorId,
                CancelByUser = true
            }, cancellationToken,
            true,
            nameof(RequestFacility.CancelByUser),
            nameof(RequestFacility.UpdateDate));
        }

        public async Task<PagingDto<RequestFacilityGuarantorListModel>> GetRequests(PagingFilterDto filter, List<RoleEnum> roles, CancellationToken cancellationToken)
        {
            var rolesString = roles.Select(p => p.ToString());
            var requestList = requestFacilityGuarantorRepository.TableNoTracking
                .Include(p => p.Guarantor)
                    .ThenInclude(p => p.Person)
                .Include(p => p.RequestFacility)
                    .ThenInclude(p => p.FacilityType)
                .Include(p => p.RequestFacility)
                    .ThenInclude(p => p.GlobalSetting)
                .Include(p => p.RequestFacility)
                    .ThenInclude(p => p.Buyer)
                        .ThenInclude(p => p.Person)
                .Include(p => p.RequestFacilityGuarantorWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                .Include(p => p.RequestFacilityGuarantorWorkFlowSteps)
                    .ThenInclude(p => p.Status)
                .Include(p => p.RequestFacilityGuarantorWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowForm)
                .Include(p => p.RequestFacilityGuarantorWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                .Include(p => p.RequestFacilityGuarantorWorkFlowSteps)
                    .ThenInclude(p => p.WorkFlowStep)
                        .ThenInclude(p => p.WorkFlowStepRoles)
                            .ThenInclude(p => p.Role)
                .AsSplitQuery();
            //.Where(p => p.GuarantorUserId.Equals(guarantorId));

            ApplyFilter(ref requestList, filter, roles);

            var waitingRequests = requestList
                .Where(p => !p.CancelByUser &&
                       p.RequestFacilityGuarantorWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                       x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(y => y == c.Role.Name))))
                .Select(p => new { item = p, Order = 1, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityGuarantorWorkFlowSteps.Max(x => x.Id) });

            var waitingRequestIDs = waitingRequests.Select(p => p.item.Id).ToList();

            var otherFacilities = requestList.Where(p => !waitingRequestIDs.Contains(p.Id))
                .Select(p => new { item = p, Order = 2, MaxRequestFacilityWorkFlowStepId = p.RequestFacilityGuarantorWorkFlowSteps.Max(x => x.Id) });

            var filterList = (await waitingRequests.Union(otherFacilities)
                .OrderBy(p => p.Order)
                .ThenByDescending(p => p.MaxRequestFacilityWorkFlowStepId)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false))
                .Select(p => new RequestFacilityGuarantorListModel
                {
                    Id = p.item.Id,
                    GuarantorUserId = p.item.GuarantorUserId,
                    GuarantorFullName = p.item.Guarantor.Person.FullName,
                    GuarantorNationalCode = p.item.Guarantor.Person.NationalCode,
                    Amount = p.item.RequestFacility.Amount,
                    MonthCountTitle = p.item.RequestFacility.FacilityType.MonthCountTitle,
                    MonthCount = p.item.RequestFacility.FacilityType.MonthCount,
                    FinancialInstitutionFacilityFee = p.item.RequestFacility.GlobalSetting.FinancialInstitutionFacilityFee,
                    LendTechFacilityFee = p.item.RequestFacility.GlobalSetting.LendTechFacilityFee,
                    WarantyPercentage = p.item.RequestFacility.GlobalSetting.WarantyPercentage,
                    FacilityInterest = p.item.RequestFacility.GlobalSetting.FacilityInterest,
                    Requester = $"{p.item.RequestFacility.Buyer.Person.FName} {p.item.RequestFacility.Buyer.Person.LName}",
                    NationalCode = p.item.RequestFacility.Buyer.Person.NationalCode,
                    //LeasingName = p.item.RequestFacility.OrganizationId.HasValue ? p.item.RequestFacility.Organization.Name : string.Empty,
                    CancelByUser = p.item.CancelByUser,
                    CreateDate = p.item.CreatedDate,
                    UpdateDate = p.item.UpdateDate,
                    RequestFacilityGuarantorWorkFlowStepList = p.item.RequestFacilityGuarantorWorkFlowSteps.Any()
                    ? p.item.RequestFacilityGuarantorWorkFlowSteps.Select(x => new WorkFlowStepListModel
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
                filterList = filterList.ToList();
                foreach (var request in filterList)
                {
                    if (request.RequestFacilityGuarantorWorkFlowStepList != null &&
                        request.RequestFacilityGuarantorWorkFlowStepList.Any())
                    {
                        request.MaxWorkFlowStepId = request.RequestFacilityGuarantorWorkFlowStepList.Max(p => p.Id);
                        request.LastActionDate = request.RequestFacilityGuarantorWorkFlowStepList.Max(p => p.CreateDate);
                        if (!request.CancelByUser &&
                            request.RequestFacilityGuarantorWorkFlowStepList.Any(p => !p.StatusId.HasValue &&
                                                                        p.WorkFlowStepRoles != null &&
                                                                        p.StepIsManual &&
                                                                        !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                                                        p.WorkFlowStepRoles.Any(x => roles.Any(c => c.ToString() == x))))
                        {
                            var waitingStep = request.RequestFacilityGuarantorWorkFlowStepList.First(p => !p.StatusId.HasValue &&
                                    p.WorkFlowStepRoles != null &&
                                    p.StepIsManual &&
                                    !string.IsNullOrEmpty(p.WorkFlowFormUrl) &&
                                    p.WorkFlowStepRoles.Any(x => roles.Any(c => c.ToString() == x)));
                            request.FormUrl = $"{waitingStep.WorkFlowFormUrl}/{request.Id}";
                        }

                        var waitngList = filterList.Where(p => p.FormUrl != null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();
                        var noneWaitingList = filterList.Where(p => p.FormUrl == null).OrderByDescending(p => p.MaxWorkFlowStepId).ToList();

                        filterList = waitngList.Concat(noneWaitingList).ToList();
                    }
                }
            }

            var totalRowCounts = await requestList.CountAsync();

            var pagingResult = new PagingDto<RequestFacilityGuarantorListModel>()
            {
                CurrentPage = filter.Page,
                Data = filterList,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return pagingResult;
        }
        private IQueryable<RequestFacilityGuarantor> ApplyFilter(ref IQueryable<RequestFacilityGuarantor> requestFacilitieGuarantors, PagingFilterDto filter, List<RoleEnum> roles)
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
                        //case "OperatorId":
                        //    {
                        //        Guid operatorId = new Guid(item.PropertyValue);
                        //        requestFacilitieGuarantors = requestFacilitieGuarantors.Where(p => p.OperatorId == operatorId);
                        //        break;
                        //    }
                        //case "FName":
                        //    {
                        //        string propertyValue = item.PropertyValue;
                        //        propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                        //        if (!string.IsNullOrEmpty(propertyValue))
                        //            requestFacilities = requestFacilities.Where(p => p.Buyer.Person.FName.Replace(" ", string.Empty).Contains(propertyValue));
                        //        break;
                        //    }
                        //case "LName":
                        //    {
                        //        string propertyValue = item.PropertyValue;
                        //        propertyValue = propertyValue.CleanString().Replace(" ", string.Empty);
                        //        if (!string.IsNullOrEmpty(propertyValue))
                        //            requestFacilities = requestFacilities.Where(p => p.Buyer.Person.LName.Replace(" ", string.Empty).Contains(propertyValue));
                        //        break;
                        //    }
                        //case "NationalCode":
                        //    {
                        //        string propertyValue = item.PropertyValue;
                        //        requestFacilities = requestFacilities.Where(p => p.Buyer.Person.Nationalcode.Contains(propertyValue));
                        //        break;
                        //    }
                        //case "WaitingStepId":
                        //    {
                        //        int propertyValue = Convert.ToInt32(item.PropertyValue);
                        //        requestFacilities = requestFacilities.Where(p => //!p.CancelByUser &&
                        //            p.RequestFacilityWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStepId == propertyValue));
                        //        break;
                        //    }
                        //case "StartDate":
                        //    {
                        //        DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                        //        requestFacilities = requestFacilities.Where(p => p.CreatedDate.Date >= propertyValue.Date);
                        //        break;
                        //    }
                        //case "EndDate":
                        //    {
                        //        DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                        //        requestFacilities = requestFacilities.Where(p => p.CreatedDate.Date <= propertyValue.Date);
                        //        break;
                        //    }
                        //case "StartAmount":
                        //    {
                        //        long propertyValue = item.PropertyValue;
                        //        requestFacilities = requestFacilities.Where(p => p.Amount >= propertyValue);
                        //        break;
                        //    }
                        //case "EndAmount":
                        //    {
                        //        long propertyValue = item.PropertyValue;
                        //        requestFacilities = requestFacilities.Where(p => p.Amount <= propertyValue);
                        //        break;
                        //    }
                        case "GuarantorUserId":
                            Guid operatorId = new Guid(item.PropertyValue.ToString());
                            requestFacilitieGuarantors = requestFacilitieGuarantors.Where(p => p.GuarantorUserId == operatorId);
                            break;
                        case "RequestStatus":
                            {
                                long propertyValue = item.PropertyValue;
                                switch (propertyValue)
                                {
                                    case (long)RequestFacilityGuarantorStatus.WaitingRequest://Waiting Request Facilities for Roles
                                        {
                                            if (roles == null) break;

                                            if (roles.Any(p => p == RoleEnum.Buyer))
                                            {
                                                var rolesString = roles != null ? roles.Select(p => p.ToString()).ToList() : Enumerable.Empty<string>();
                                                requestFacilitieGuarantors = requestFacilitieGuarantors.Where(p => !p.CancelByUser &&
                                                    p.RequestFacilityGuarantorWorkFlowSteps.Any(x => !x.StatusId.HasValue && x.WorkFlowStep.WorkFlowStepRoles.Any() &&
                                                    (!rolesString.Any() || x.WorkFlowStep.WorkFlowStepRoles.Any(c => rolesString.Any(x => x == c.Role.Name)))));
                                            }
                                            break;
                                        }
                                    case (long)RequestFacilityGuarantorStatus.ApprovedRequest://Approve Request Facilities
                                        {
                                            requestFacilitieGuarantors = requestFacilitieGuarantors.Where(p => !p.CancelByUser &&
                                            p.RequestFacilityGuarantorWorkFlowSteps
                                                                        .Any(x => x.WorkFlowStep.IsApproveFinalStep &&
                                                                                  x.WorkFlowStep.IsLastStep &&
                                                                                  x.StatusId.HasValue &&
                                                                                  x.StatusId.Value == (short)StatusEnum.Approved));
                                            break;
                                        }
                                    case (long)RequestFacilityGuarantorStatus.RejectCancelRequest://Close/Reject Request Facilities
                                        {
                                            requestFacilitieGuarantors = requestFacilitieGuarantors.Where(p => p.CancelByUser ||
                                                p.RequestFacilityGuarantorWorkFlowSteps
                                                                        .Any(x => !x.WorkFlowStep.IsApproveFinalStep &&
                                                                                  x.WorkFlowStep.IsLastStep));
                                            break;
                                        }
                                    case (long)RequestFacilityGuarantorStatus.OpenRequest://Open Request Facilities
                                        {
                                            requestFacilitieGuarantors = requestFacilitieGuarantors.Where(p => !p.CancelByUser &&
                                                                                              p.RequestFacilityGuarantorWorkFlowSteps.Any(x => !x.StatusId.HasValue));
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

            return requestFacilitieGuarantors;
        }

        public async Task<int?> GetRequestFacilityId(int id, CancellationToken cancellationToken)
        {
            var waitingRequestFacility =
                await requestFacilityGuarantorRepository.TableNoTracking
                    .Where(p => p.Id.Equals(id))
                    .Select(p => new { p.RequestFacilityId })
                    .FirstOrDefaultAsync(cancellationToken);

            return waitingRequestFacility?.RequestFacilityId;
        }

        public async Task<bool> VerifyGuarantorByZarinLend(RequestFacilityGuarantorStatusModel statusModel, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityGuarantorId, new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin },
                 WorkFlowFormEnum.VerifyGuarantorByZarinLend, cancellationToken))
            {
                await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityGuarantorId, WorkFlowFormEnum.VerifyGuarantorByZarinLend,
                    statusModel.Status, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken);

                return true;
            }
            throw new AppException("درخواستی در این مرحبه وجود ندارد!");
        }

        public async Task<bool> VerifyGuaranteesByZarinLend(RequestFacilityGuarantorStatusModel statusModel, CancellationToken cancellationToken)
        {
            if (await CheckRequestFacilityWaitingSpecifiedStepAndRole(statusModel.RequestFacilityGuarantorId, new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin },
                 WorkFlowFormEnum.VerifyGuaranteesByZarinLend, cancellationToken))
            {
                var requestFacilityInfo = await requestFacilityGuarantorRepository.GetColumnValueAsync(p => p.Id == statusModel.RequestFacilityGuarantorId,
                    p => new
                    {
                        p.RequestFacilityId,
                        p.RequestFacility.BuyerId,
                        p.RequestFacility.AwaitingIntroductionGuarantor,
                        p.RequestFacility.GuarantorIsRequired
                    }, cancellationToken);

                //var requestFacilityRequiredGuarantor = await requestFacilityService.CheckGuarantorIsRequired(requestFacilityInfo.RequestFacilityId, cancellationToken);
                //var hasGaurantorWithApprovedWorkFlow = requestFacilityRequiredGuarantor ? await HasGaurantorWithApprovedWorkFlow(requestFacilityInfo.RequestFacilityId, cancellationToken) : false;
                if (statusModel.Status == StatusEnum.Approved && requestFacilityInfo.GuarantorIsRequired && requestFacilityInfo.AwaitingIntroductionGuarantor)
                    await requestFacilityService.UpdateAwaitingIntroductionGuarantor(requestFacilityInfo.RequestFacilityId, awaitingIntroductionGuarantor: false, saveNow: false, cancellationToken);

                await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(statusModel.RequestFacilityGuarantorId, WorkFlowFormEnum.VerifyGuaranteesByZarinLend,
                    statusModel.Status, statusModel.OpratorId, statusModel.StatusDescription, cancellationToken);

                if (statusModel.Status == StatusEnum.Approved && 
                    await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityInfo.BuyerId,
                                                                                                 requestFacilityInfo.RequestFacilityId,
                                                                                                 new List<RoleEnum> { RoleEnum.Buyer },
                                                                                                 WorkFlowFormEnum.UploadIdentityDocuments,
                                                                                                 cancellationToken))
                    await userService.GoToNextStepFromUploadDocumentsToVerifyZarinLendIfPossible(requestFacilityInfo.BuyerId, requestFacilityInfo.RequestFacilityId, cancellationToken);

                return true;
            }
            throw new AppException("درخواستی در این مرحبه وجود ندارد!");
        }

        #region Get/Check RequestFacilityGuarantorId Waiting in Specified Step And Role

        public async Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityGuarantorId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            return await requestFacilityGuarantorRepository.TableNoTracking.AnyAsync(p => p.Id.Equals(requestFacilityGuarantorId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityGuarantorWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => c.Role.Name == x))), cancellationToken);
        }
        public async Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(Guid userId, int requestFacilityGuarantorId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            return await requestFacilityGuarantorRepository.TableNoTracking.AnyAsync(p => p.Id.Equals(requestFacilityGuarantorId) &&
                                p.GuarantorUserId.Equals(userId) &&
                                !p.CancelByUser &&
                                p.RequestFacilityGuarantorWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                !x.StatusId.HasValue &&
                                x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => x == c.Role.Name))), cancellationToken);
        }
        public async Task<int?> GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(int id, Guid userId, List<RoleEnum> roles,
            WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var waitingRequestFacility =
                await requestFacilityGuarantorRepository.TableNoTracking
                .Where(p => p.Id.Equals(id) &&
                            p.GuarantorUserId.Equals(userId) &&
                            !p.CancelByUser &&
                            p.RequestFacilityGuarantorWorkFlowSteps
                                .Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                                          x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                                          !x.StatusId.HasValue &&
                                          x.WorkFlowStep.WorkFlowStepRoles
                                            .Any(c => stringRoles.Any(x => x == c.Role.Name))))
                .Select(p => new { p.Id })
                .FirstOrDefaultAsync(cancellationToken);

            return waitingRequestFacility?.Id;
        }
        public async Task<int?> GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum,
            CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());
            var waitingRequestFacility =
                await requestFacilityGuarantorRepository.TableNoTracking.FirstOrDefaultAsync(p => p.GuarantorUserId.Equals(userId) &&
                    !p.CancelByUser &&
                    p.RequestFacilityGuarantorWorkFlowSteps.Any(x => x.WorkFlowStep.WorkFlowFormId.HasValue &&
                    x.WorkFlowStep.WorkFlowFormId == workFlowFormEnum &&
                    !x.StatusId.HasValue &&
                    x.WorkFlowStep.WorkFlowStepRoles.Any(c => stringRoles.Any(x => x == c.Role.Name))), cancellationToken);

            return waitingRequestFacility?.Id;
        }

        #endregion Get RequestFacilityGuarantorId Waiting in Specified Step And Role
    }
}
