using Asp.Versioning;
using AutoMapper;
using Common;
using Common.CustomAttribute;
using Common.CustomFileAttribute;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Services;
using Services.Dto;
using Services.Model;
using Services.Model.NeginHub;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;
using ZarinLend.Services.Model;
using ZarinLend.Services.Model.NeginHub;
using static Common.Utilities.FileExtensions;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class RequestFacilityController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly ISamatService samatService;
        private readonly IEmailSender emailSender;
        private readonly IExcelService excelService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IVerifyResultExcelService verifyResultExcelService;
        private readonly IApplicantValidationResultService applicantValidationResultService;
        private readonly INeginHubService neginHubService;
        private readonly IPersonService personService;

        public RequestFacilityController(IMapper mapper, IRequestFacilityService requestFacilityService, ISamatService samatService,
            IEmailSender emailSender, IExcelService excelService, IWebHostEnvironment webHostEnvironment, IVerifyResultExcelService verifyResultExcelService,
            IApplicantValidationResultService applicantValidationResultService,
            INeginHubService neginHubService, IPersonService personService)
        {
            _mapper = mapper;
            this.requestFacilityService = requestFacilityService;
            this.samatService = samatService;
            this.emailSender = emailSender;
            this.excelService = excelService;
            this.webHostEnvironment = webHostEnvironment;
            this.verifyResultExcelService = verifyResultExcelService;
            this.applicantValidationResultService = applicantValidationResultService;
            this.neginHubService = neginHubService;
            this.personService = personService;
        }

        #region Apis for External usings

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> MoveToVerifyStepFromUploadDocuments(int requestFacilityId, CancellationToken cancellationToken)
        {
            return await requestFacilityService.MoveToVerifyStepFromUploadDocuments(requestFacilityId, new Guid(User.Identity!.GetUserId()), cancellationToken);
        }

        #endregion Apis for External usings

        #region All Roles Methods

        [HttpPost("[action]")]
        [CustomAuthorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetFacilityTypes(CancellationToken cancellationToken)
        {
            return new ApiResult<dynamic>(true, ApiResultStatusCode.Success, await requestFacilityService.GetFacilityTypes(cancellationToken));
        }

        [HttpPost("[action]")]
        [CustomAuthorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetUsagePlaces(CancellationToken cancellationToken)
        {
            return new ApiResult<dynamic>(true, ApiResultStatusCode.Success, await requestFacilityService.GetUsagePlaces(cancellationToken));
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, RoleEnum.BankLeasing, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetRequestFacilityStepsHistory(int requestFacilityId, CancellationToken cancellationToken)
        {
            List<RequestFacilityWorkFlowStepHistoryModel> result;
            if (User.IsInRole(RoleEnum.Buyer.ToString()))
                result = await requestFacilityService.GetRequestFacilityStepsHistory(requestFacilityId, new Guid(User.Identity!.GetUserId()), cancellationToken);
            else if (User.IsInRole(RoleEnum.AdminBankLeasing.ToString()) || User.IsInRole(RoleEnum.SupervisorLeasing.ToString()) || User.IsInRole(RoleEnum.BankLeasing.ToString()))
                result = await requestFacilityService.GetRequestFacilityStepsHistory(requestFacilityId, User.Identity!.GetUserLeasingId(), cancellationToken);
            else
                result = await requestFacilityService.GetRequestFacilityStepsHistory(requestFacilityId, cancellationToken);

            return new ApiResult<dynamic>(true, ApiResultStatusCode.Success, result);
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> CardRecharge(int requestFacilityId, CancellationToken cancellationToken)
        {
            var creatorId = new Guid(User.Identity!.GetUserId());
            await requestFacilityService.CardRecharge(requestFacilityId, creatorId, User.Identity!.GetUserLeasingId(), cancellationToken);
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        #endregion All Roles Methods

        #region Leasing Methods

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> AssignRequestFacilityToUser(AssignRequestFacilityToUserModel model, CancellationToken cancellationToken)
        {
            Assert.NotEmpty(model.RequestFacilityIds, nameof(model.RequestFacilityIds));
            model.CreatorId = new Guid(User.Identity!.GetUserId());
            await requestFacilityService.AssignRequestFacilityToUser(model, cancellationToken);
            return true;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<RequestFacilityListModel>> GetAllLeasingRequest(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var role = User.IsInRole(RoleEnum.AdminBankLeasing.ToString())
                    ? RoleEnum.AdminBankLeasing
                    : (User.IsInRole(RoleEnum.SupervisorLeasing.ToString()) ? RoleEnum.SupervisorLeasing : RoleEnum.BankLeasing);
            return await requestFacilityService.SearchLeasingRequest(User.Identity!.GetUserLeasingId(), new List<RoleEnum> { role }, filter, false, cancellationToken);
            //return await requestFacilityService.GetAllLeasingRequest(User.Identity!.GetUserLeasingId(), new List<RoleEnum> { role }, filter, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<RequestFacilityListModel>> SearchLeasingRequest(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var role = User.IsInRole(RoleEnum.AdminBankLeasing.ToString())
                        ? RoleEnum.AdminBankLeasing
                        : (User.IsInRole(RoleEnum.SupervisorLeasing.ToString()) ? RoleEnum.SupervisorLeasing : RoleEnum.BankLeasing);
            return await requestFacilityService.SearchLeasingRequest(User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { role }, filter, false, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string?> SearchLeasingRequestExport(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var role = User.IsInRole(RoleEnum.AdminBankLeasing.ToString())
                        ? RoleEnum.AdminBankLeasing
                        : (User.IsInRole(RoleEnum.SupervisorLeasing.ToString()) ? RoleEnum.SupervisorLeasing : RoleEnum.BankLeasing);
            return await GenerateExcel((await requestFacilityService.SearchLeasingRequest(User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { role }, filter, true, cancellationToken)).Data, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> EnterFacilityNumber(EnterFacilityNumberModel model, CancellationToken cancellationToken)
        {
            if (model.Status != StatusEnum.ReturnToCorrection && string.IsNullOrEmpty(model.FacilityNumber))
                throw new Exception("شماره قرارداد اجباری است.");

            model.OpratorId = new Guid(User.Identity!.GetUserId());
            return await requestFacilityService.EnterFacilityNumber(model, User.Identity!.GetUserLeasingId(), true, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> VerifyAndAllocationByLeasing(VerifyAndPoliceNumberModel model, CancellationToken cancellationToken)
        {
            if (model.Status != StatusEnum.ReturnToCorrection && string.IsNullOrEmpty(model.PoliceNumber))
                throw new Exception("شماره انتظامی اجباری است.");

            model.OpratorId = new Guid(User.Identity!.GetUserId());
            return await requestFacilityService.VerifyAndAllocationByLeasing(model, User.Identity!.GetUserLeasingId(), true, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> VerifyCheckByLeasing(RequestStatusModel model, CancellationToken cancellationToken)
        {
            model.OpratorId = new Guid(User.Identity!.GetUserId());
            return await requestFacilityService.VerifyCheckByLeasing(model, User.Identity!.GetUserLeasingId(), cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> DepositFacilityAmount([FromForm] RequestFacilityDepositDocumentModel model, IFormFile? file, int leasingId, CancellationToken cancellationToken)
        {
            model.CreatorId = new Guid(User.Identity!.GetUserId());
            await requestFacilityService.DepositFacilityAmount(model, file, User.Identity!.GetUserLeasingId(), cancellationToken);
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> VerifyByLeasing(RequestStatusAndApplicantValidationResultParam model, CancellationToken cancellationToken)
        {
            model.RequestStatus.OpratorId = new Guid(User.Identity!.GetUserId());
            model.ApplicantValidationResult.CreatorId = new Guid(User.Identity!.GetUserId());
            return await requestFacilityService.VerifyByLeasing(model.RequestStatus, model.ApplicantValidationResult, User.Identity!.GetUserLeasingId(), true, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string?> DownloadPendingBankLeasingInquiryRequestsExcel(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();
            var personList = (await requestFacilityService.SearchPendingInEnterPoliceNumberRequest(User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, filter, true, cancellationToken)).Data.ToList();
            if (personList != null && personList.Any())
            {
                for (var i = 0; i < personList.Count; i++)
                {
                    var result = await requestFacilityService.GoToNextStepFromBankLeasingInquiryToPendingBankLeasingInquiry(personList[i].Id, organizationId, new Guid(User.Identity!.GetUserId()),
                         autoSave: i == personList.Count - 1, cancellationToken);
                }
            }
            return await GenerateBankLeasingInquiryExcel(personList, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string?> DownloadPendingDepositFacilityAmountRequestsExcel(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();
            var personList = (await requestFacilityService.SearchPendingInDepositFacilityAmountRequest(User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, filter, true, cancellationToken)).Data.ToList();
            if (personList != null && personList.Any())
            {
                for (var i = 0; i < personList.Count; i++)
                {
                    var result = await requestFacilityService.GoToNextStepFromDepositFacilityAmountToPendingResultDepositFacilityAmount(personList[i].Id, organizationId, new Guid(User.Identity!.GetUserId()),
                         autoSave: i == personList.Count - 1, cancellationToken);
                }
            }
            return await GenerateDepositFacilityAmountExcel(personList, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string?> DownloadPendingChargeCardRequestsExcel(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();
            var personList = (await requestFacilityService.SearchPendingInChargeCardRequest(User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, filter, true, cancellationToken)).Data.ToList();
            if (personList != null && personList.Any())
            {
                for (var i = 0; i < personList.Count; i++)
                {
                    var result = await requestFacilityService.GoToNextStepFromChargeCardToPendingResultChargeCard(personList[i].Id, organizationId, new Guid(User.Identity!.GetUserId()),
                         autoSave: i == personList.Count - 1, cancellationToken);
                }
            }
            return await GenerateCardRechargeExcel(personList, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 1024)]
        public virtual async Task<dynamic> UploadBankLeasingInquiryResult([MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/),
            AllowedExtensions(new[] {".xls",".xlsx"},errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )] IFormFile bankLeasingInquiryrResultFile,
            CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();

            if (bankLeasingInquiryrResultFile == null || bankLeasingInquiryrResultFile.Length == 0)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل نتیجه شماره انتظامی را جهت بارگذاری،انتخاب کنید!");

            using (var ms = new MemoryStream())
            {
                bankLeasingInquiryrResultFile.CopyTo(ms);
                var policeNumberResultList = ExtractPoliceNumberFromExcel(ms);
                if (policeNumberResultList == null)
                    throw new AppException("براساس فایل بارگذاری شده هیچ اطلاعاتی یافت نشد");

                if (policeNumberResultList.Any(p => p.HasError))
                    throw new AppException(policeNumberResultList.First(p => p.HasError).ErrorMessage);

                var dictionary = new Dictionary<string, StatusEnum>();
                for (int i = 0; i < policeNumberResultList.Count; i++)
                {
                    var requestFacilityId = await requestFacilityService.GetRequestFacilityId(policeNumberResultList[i].NationalCode,
                        organizationId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.PendingBankLeasingInquiry, cancellationToken);
                    if (!requestFacilityId.HasValue)
                    {
                        requestFacilityId = await requestFacilityService.GetRequestFacilityId(policeNumberResultList[i].NationalCode,
                        organizationId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.BankLeasingInquiry, cancellationToken);
                        if (!requestFacilityId.HasValue)
                            throw new AppException(ApiResultStatusCode.LogicError,
                            $"براساس کد ملی {policeNumberResultList[i].NationalCode} اطلاعاتی یافت نشد<br/>ممکن است درخواست تسهیلاتی براساس کد ملی فوق در این مرحله وجود نداشته باشد");
                    }

                    policeNumberResultList[i].CreatorId = new Guid(User.Identity!.GetUserId());
                    policeNumberResultList[i].RequestFacilityId = requestFacilityId.Value;

                    var statusModel = new VerifyAndPoliceNumberModel()
                    {
                        PoliceNumber = policeNumberResultList[i].PoliceNumber,
                        LeasingId = organizationId,
                        OpratorId = new Guid(User.Identity!.GetUserId()),
                        Status = StatusEnum.Approved,
                        RequestFacilityId = requestFacilityId.Value
                    };
                    await requestFacilityService.VerifyAndAllocationByLeasing(statusModel, organizationId, policeNumberResultList.Count() - 1 == i, cancellationToken);

                    dictionary.Add(policeNumberResultList[i].NationalCode, StatusEnum.Approved);
                }
                return new ApiResult<Dictionary<string, StatusEnum>>(true, ApiResultStatusCode.Success, dictionary);
            }
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 1024)]
        public virtual async Task<dynamic> UploadDepositFacilityAmountResult([MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/),
            AllowedExtensions(new[] {".xls",".xlsx"},errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )] IFormFile depositFacilityAmountResultFile,
            CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();

            if (depositFacilityAmountResultFile == null || depositFacilityAmountResultFile.Length == 0)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل نتیجه واریز تسهیلات را جهت بارگذاری،انتخاب کنید!");

            using (var ms = new MemoryStream())
            {
                depositFacilityAmountResultFile.CopyTo(ms);
                var depostFacilityAmountResultList = ExtractDepositFacilityAmountFromExcel(ms);
                if (depostFacilityAmountResultList == null)
                    throw new AppException("براساس فایل بارگذاری شده هیچ اطلاعاتی یافت نشد");

                if (depostFacilityAmountResultList.Any(p => p.HasError))
                    throw new AppException(depostFacilityAmountResultList.First(p => p.HasError).ErrorMessage);

                var dictionary = new Dictionary<string, StatusEnum>();
                for (int i = 0; i < depostFacilityAmountResultList.Count; i++)
                {
                    var requestFacilityId = await requestFacilityService.GetRequestFacilityId(depostFacilityAmountResultList[i].NationalCode,
                        organizationId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.PendingDepositFacilityAmount, cancellationToken);
                    if (!requestFacilityId.HasValue)
                    {
                        requestFacilityId = await requestFacilityService.GetRequestFacilityId(depostFacilityAmountResultList[i].NationalCode,
                        organizationId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.DepositFacilityAmount, cancellationToken);
                        if (!requestFacilityId.HasValue)
                            throw new AppException(ApiResultStatusCode.LogicError,
                            $"براساس کد ملی {depostFacilityAmountResultList[i].NationalCode} اطلاعاتی یافت نشد<br/>ممکن است درخواست تسهیلاتی براساس کد ملی فوق در این مرحله وجود نداشته باشد");
                    }

                    depostFacilityAmountResultList[i].CreatorId = new Guid(User.Identity!.GetUserId());
                    depostFacilityAmountResultList[i].RequestFacilityId = requestFacilityId.Value;

                    var statusModel = new RequestFacilityDepositDocumentModel()
                    {
                        CreatorId = new Guid(User.Identity!.GetUserId()),
                        StepDescription = "تغییر",
                        RequestFacilityId = requestFacilityId.Value,
                        Status = StatusEnum.Approved,
                    };
                    await requestFacilityService.DepositFacilityAmount(statusModel, null, organizationId, cancellationToken);

                    dictionary.Add(depostFacilityAmountResultList[i].NationalCode, StatusEnum.Approved);
                }
                return new ApiResult<Dictionary<string, StatusEnum>>(true, ApiResultStatusCode.Success, dictionary);
            }
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 1024)]
        public virtual async Task<dynamic> UploadCardRechargeResult([MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/),
            AllowedExtensions([".xls",".xlsx"],errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )] IFormFile cardRechargeResultFile, CancellationToken cancellationToken)
        {
            var leasingId = User.Identity!.GetUserLeasingId();

            if (cardRechargeResultFile == null || cardRechargeResultFile.Length == 0)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل نتیجه واریز تسهیلات را جهت بارگذاری،انتخاب کنید!");

            using (var ms = new MemoryStream())
            {
                cardRechargeResultFile.CopyTo(ms);
                var depostFacilityAmountResultList = ExtractDepositFacilityAmountFromExcel(ms);
                if (depostFacilityAmountResultList == null)
                    throw new AppException("براساس فایل بارگذاری شده هیچ اطلاعاتی یافت نشد");

                if (depostFacilityAmountResultList.Any(p => p.HasError))
                    throw new AppException(depostFacilityAmountResultList.First(p => p.HasError).ErrorMessage);

                var dictionary = new Dictionary<string, StatusEnum>();
                for (int i = 0; i < depostFacilityAmountResultList.Count; i++)
                {
                    var requestFacilityId = await requestFacilityService.GetRequestFacilityId(depostFacilityAmountResultList[i].NationalCode,
                        leasingId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.CardRecharge, cancellationToken);
                    if (!requestFacilityId.HasValue)
                    {
                        throw new AppException(ApiResultStatusCode.LogicError,
                        $"براساس کد ملی {depostFacilityAmountResultList[i].NationalCode} اطلاعاتی یافت نشد<br/>ممکن است درخواست تسهیلاتی براساس کد ملی فوق در این مرحله وجود نداشته باشد");
                    }

                    depostFacilityAmountResultList[i].CreatorId = new Guid(User.Identity!.GetUserId());
                    depostFacilityAmountResultList[i].RequestFacilityId = requestFacilityId.Value;

                    await requestFacilityService.CardRecharge(requestFacilityId.Value, new Guid(User.Identity!.GetUserId()), leasingId, cancellationToken);

                    dictionary.Add(depostFacilityAmountResultList[i].NationalCode, StatusEnum.Approved);
                }
                return new ApiResult<Dictionary<string, StatusEnum>>(true, ApiResultStatusCode.Success, dictionary);
            }
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 1024)]
        public virtual async Task<dynamic> UploadGroupVerifyOctectResult([MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/),
            AllowedExtensions(new string[] {".xls",".xlsx"},errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )]  IFormFile verifyOctectResultFile, CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();

            if (verifyOctectResultFile == null || verifyOctectResultFile.Length == 0)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل نتیجه اعتبارسنجی را جهت بارگذاری،انتخاب کنید!");

            using (var ms = new MemoryStream())
            {
                verifyOctectResultFile.CopyTo(ms);
                var extractVerifyResultList = applicantValidationResultService.ExtractVerifyResultFromExcel(ms);
                if (extractVerifyResultList == null || !extractVerifyResultList.Any())
                    throw new AppException("براساس فایل بارگذاری شده هیچ اطلاعاتی یافت نشد");

                for (int i = 0; i < extractVerifyResultList.Count; i++)
                {
                    var requestFacilityId = await requestFacilityService.GetRequestFacilityId(extractVerifyResultList[i].NationalCode,
                        organizationId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.PendingForVerifyResult, cancellationToken);
                    if (!requestFacilityId.HasValue)
                    {
                        extractVerifyResultList[i].ErrorMessage += $"براساس کد ملی {extractVerifyResultList[i].NationalCode} اطلاعاتی یافت نشد.ممکن است درخواست تسهیلاتی براساس کد ملی فوق در این مرحله وجود نداشته باشد<br/>";
                        extractVerifyResultList[i].HasError = true;
                    }
                    else
                        extractVerifyResultList[i].RequestFacilityId = requestFacilityId.Value;
                }
                var dictionary = new Dictionary<string, StatusEnum?>();
                if (extractVerifyResultList.Any())
                    await verifyResultExcelService.SaveVerifyResultFromFile(verifyOctectResultFile, extractVerifyResultList, User.Identity!.GetUserLeasingId(),
                        new Guid(User.Identity!.GetUserId()), cancellationToken);

                var validVerifyResultList = extractVerifyResultList.Where(p => !p.HasError && p.FinalResult.HasValue).ToList();
                if (!validVerifyResultList.Any())
                    throw new AppException("با توجه فایل اکسل فوق هیچ تسهیلاتی یافت نشد یا براساس کد ملی های در فایل هیچ تسهیلاتی در مرحله استعلام قرار ندارد");

                for (int i = 0; i < validVerifyResultList.Count; i++)
                {
                    //TODO : replace this code to FinalResult!!!
                    StatusEnum? actionType = validVerifyResultList[i].BlackListInquiry.HasValue && validVerifyResultList[i].BlackListInquiry.Value &&
                                 validVerifyResultList[i].CivilRegistryInquiry.HasValue && validVerifyResultList[i].CivilRegistryInquiry.Value &&
                                 validVerifyResultList[i].FacilityInquiry.HasValue && validVerifyResultList[i].FacilityInquiry.Value &&
                                 validVerifyResultList[i].MilitaryInquiry.HasValue && validVerifyResultList[i].MilitaryInquiry.Value &&
                                 validVerifyResultList[i].PostalCodeInquiry.HasValue && validVerifyResultList[i].PostalCodeInquiry.Value &&
                                 validVerifyResultList[i].ReturnedCheckInquiry.HasValue && validVerifyResultList[i].ReturnedCheckInquiry.Value &&
                                 validVerifyResultList[i].SecurityCouncilSanctionsInquiry.HasValue && validVerifyResultList[i].SecurityCouncilSanctionsInquiry.Value &&
                                 validVerifyResultList[i].ShahkarInquiry.HasValue && validVerifyResultList[i].ShahkarInquiry.Value
                                 ? StatusEnum.Approved
                                 : ((validVerifyResultList[i].BlackListInquiry.HasValue && !validVerifyResultList[i].BlackListInquiry.Value) ||
                                    (validVerifyResultList[i].CivilRegistryInquiry.HasValue && !validVerifyResultList[i].CivilRegistryInquiry.Value) ||
                                    (validVerifyResultList[i].FacilityInquiry.HasValue && !validVerifyResultList[i].FacilityInquiry.Value) ||
                                    (validVerifyResultList[i].MilitaryInquiry.HasValue && !validVerifyResultList[i].MilitaryInquiry.Value) ||
                                    (validVerifyResultList[i].PostalCodeInquiry.HasValue && !validVerifyResultList[i].PostalCodeInquiry.Value) ||
                                    (validVerifyResultList[i].ReturnedCheckInquiry.HasValue && !validVerifyResultList[i].ReturnedCheckInquiry.Value) ||
                                    (validVerifyResultList[i].SecurityCouncilSanctionsInquiry.HasValue && !validVerifyResultList[i].SecurityCouncilSanctionsInquiry.Value) ||
                                    (validVerifyResultList[i].ShahkarInquiry.HasValue && !validVerifyResultList[i].ShahkarInquiry.Value))
                                    ? StatusEnum.ReturnToCorrection
                                    : null;

                    if (actionType != null)
                    {
                        var requestStatus = new PrimaryVerifyModel()
                        {
                            OpratorId = new Guid(User.Identity!.GetUserId()),
                            LeasingId = organizationId,
                            RequestFacilityId = validVerifyResultList[i].RequestFacilityId.Value,
                            Status = actionType.Value,
                            StatusDescription = actionType == StatusEnum.Approved
                                ? "تغییر وضعیت و رفتن به مرحله بعد براساس اکسل اعتبارسنجی 8 گانه بصورت گروهی"
                                : $"رد و ارسال به مرحله قبل جهت اصلاح پرونده.<br/>{string.Join("<br/>", validVerifyResultList[i].ErrorMessage)}"
                        };
                        validVerifyResultList[i].CreatorId = new Guid(User.Identity!.GetUserId());
                        validVerifyResultList[i].RequestFacilityId = validVerifyResultList[i].RequestFacilityId;
                        await requestFacilityService.VerifyByLeasing(requestStatus, validVerifyResultList[i], User.Identity!.GetUserLeasingId(),
                            validVerifyResultList.Count() - 1 == i, cancellationToken);
                    }
                    dictionary.Add(validVerifyResultList[i].NationalCode, actionType);
                }
                return new ApiResult<Dictionary<string, StatusEnum?>>(true, ApiResultStatusCode.Success, dictionary);
            }
        }


        [HttpPost("[action]/{requestFacilityId:int}/{organizationId:int}")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 100/*Max File Size : 100KB*/)]
        public virtual async Task<ApplicantValidationResultModel> UploadSingleVerifyOctectResult([MaxFileSize(1 * 1024 * 100/*Max File Size : 100KB*/),
            AllowedExtensions(new string[] {".xls",".xlsx"},errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )] IFormFile verifyOctectResultFile, int requestFacilityId, int organizationId, CancellationToken cancellationToken)
        {
            if (User.Identity!.GetUserLeasingId() != organizationId)
                throw new AppException(ApiResultStatusCode.UnAuthorized, "خطای دسترسی!");
            if (verifyOctectResultFile == null)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل نتیجه اعتبارسنجی را جهت بارگذاری،انتخاب کنید!");

            if (verifyOctectResultFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    verifyOctectResultFile.CopyTo(ms);
                    var personInfo = await requestFacilityService.GetPersonInfo(requestFacilityId, cancellationToken);
                    var extractVerifyResult = applicantValidationResultService.ExtractVerifyResultFromExcel(ms);
                    if (personInfo != null && extractVerifyResult != null && extractVerifyResult.Any())
                    {
                        if (personInfo.NationalCode != extractVerifyResult[0].NationalCode)
                            throw new AppException(ApiResultStatusCode.LogicError,
                                $"کد ملی موجود در فایل اکسل با کد ملی درخواست دهنده تسهیلات مطابقت ندارد!<br/>کد ملی درخواست دهنده " +
                                $": {personInfo.NationalCode}<br/>کد ملی در فایل اکسل : {extractVerifyResult[0].NationalCode}");

                        return extractVerifyResult[0];
                    }
                }
            }

            return null;
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string> DownloadGroupVerifyOctectExcel(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();
            var personList = await requestFacilityService.GetPersonListWaitngInSpecialStep(filter, organizationId,
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, cancellationToken);
            if (personList != null && personList.Any())
            {
                for (var i = 0; i < personList.Count; i++)
                {
                    var result = await requestFacilityService.GoToNextStepFromVerifyToPendingResultVerify(personList[i].RequestFacilityId, organizationId, new Guid(User.Identity!.GetUserId()),
                         autoSave: i == personList.Count - 1, cancellationToken);
                }
            }
            return await GenerateVerifyOctectExcel(personList, cancellationToken);
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string?> DownloadPendingEnterFacilityNumberRequestsExcel(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();
            var personList = (await requestFacilityService.SearchPendingInEnterFacilityNumberRequest(User.Identity!.GetUserLeasingId(),
                                                                                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing },
                                                                                filter,
                                                                                true,
                                                                                cancellationToken)).Data.ToList();

            if (personList != null && personList.Any())
            {
                for (var i = 0; i < personList.Count; i++)
                {
                    var result = await requestFacilityService.GoToNextStepFromEnterFacilityToPendingEnterFacility(personList[i].Id, organizationId, new Guid(User.Identity!.GetUserId()),
                         autoSave: i == personList.Count - 1, cancellationToken);
                }

                return await GenerateEnterFacilityNumberExcel(personList, cancellationToken);
            }
            return null;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 1024)]
        public virtual async Task<dynamic> UploadFacilityNumberResult([MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/),
            AllowedExtensions([".xls",".xlsx"],errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )] IFormFile enterFacilityNumberResultFile,
            CancellationToken cancellationToken)
        {
            var organizationId = User.Identity!.GetUserLeasingId();

            if (enterFacilityNumberResultFile == null || enterFacilityNumberResultFile.Length == 0)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل نتیجه شماره قرارداد را جهت بارگذاری،انتخاب کنید!");

            using (var ms = new MemoryStream())
            {
                enterFacilityNumberResultFile.CopyTo(ms);
                var facilityNumberResultList = ExtractFacilityNumberFromExcel(ms);
                if (facilityNumberResultList == null)
                    throw new AppException("براساس فایل بارگذاری شده هیچ اطلاعاتی یافت نشد");

                if (facilityNumberResultList.Any(p => p.HasError))
                    throw new AppException(facilityNumberResultList.First(p => p.HasError).ErrorMessage);

                var dictionary = new Dictionary<string, StatusEnum>();
                for (int i = 0; i < facilityNumberResultList.Count; i++)
                {
                    var requestFacilityId = await requestFacilityService.GetRequestFacilityId(facilityNumberResultList[i].NationalCode,
                        organizationId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.PendingEnterFacilityNumber, cancellationToken);
                    if (!requestFacilityId.HasValue)
                    {
                        requestFacilityId = await requestFacilityService.GetRequestFacilityId(facilityNumberResultList[i].NationalCode,
                        organizationId, new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.EnterFacilityNumber, cancellationToken);
                        if (!requestFacilityId.HasValue)
                            throw new AppException(ApiResultStatusCode.LogicError,
                            $"براساس کد ملی {facilityNumberResultList[i].NationalCode} اطلاعاتی یافت نشد<br/>ممکن است درخواست تسهیلاتی براساس کد ملی فوق در این مرحله وجود نداشته باشد");
                    }

                    facilityNumberResultList[i].CreatorId = new Guid(User.Identity!.GetUserId());
                    facilityNumberResultList[i].RequestFacilityId = requestFacilityId.Value;

                    var statusModel = new EnterFacilityNumberModel()
                    {
                        FacilityNumber = facilityNumberResultList[i].FacilityNumber,
                        LeasingId = organizationId,
                        OpratorId = new Guid(User.Identity!.GetUserId()),
                        Status = StatusEnum.Approved,
                        RequestFacilityId = requestFacilityId.Value
                    };
                    await requestFacilityService.EnterFacilityNumber(statusModel, organizationId, facilityNumberResultList.Count() - 1 == i, cancellationToken);

                    dictionary.Add(facilityNumberResultList[i].NationalCode, StatusEnum.Approved);
                }
                return new ApiResult<Dictionary<string, StatusEnum>>(true, ApiResultStatusCode.Success, dictionary);
            }
        }

        #endregion Leasing Methods

        #region ZarinLend Admin/SuperAdmin

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<RequestFacilityListModel>> SearchRequest(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await requestFacilityService.SearchRequest(filter, new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert }, false, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string?> SearchRequestExport(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await GenerateExcel((await requestFacilityService.SearchRequest(filter,
                                                                                   new List<RoleEnum> { RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert },
                                                                                   true, cancellationToken)).Data,
                                                                                   cancellationToken);
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> SendRequestFacilityDetailToLeasingEmail(int requestFacilityId, CancellationToken cancellationToken)
        {
            var requestDetail = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, cancellationToken);
            var activeFacilities = await samatService.GetUserFacilitiesFromDB(requestDetail.Id, cancellationToken);
            var backCheques = await samatService.GetUserBackChequesFromDB(requestDetail.Id, cancellationToken);
            var excelFileStream = await excelService.GenerateRequestFacilityDetail(requestDetail, activeFacilities, backCheques, cancellationToken);

            var userIdentityDocumentPath = @"UploadFiles\UserIdentityDocument";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userIdentityDocumentPath);
            var userIdentityDocumentPathList = new List<FilePath>();
            foreach (var document in requestDetail.BuyerIdentityDocuments)
            {
                userIdentityDocumentPathList.Add(
                    new FilePath()
                    {
                        PhysicalPath = Path.Combine(uploadFolder, Path.GetFileName(document.FilePath)),
                        FileName = $"{document.DocumentType.ToDisplay()}.{Path.GetExtension(document.FilePath)}"
                    });
            }

            var documentsZipArchive = await GetZipArchive(userIdentityDocumentPathList);
            var detaiExcelZipArchive = await GetZipArchive(new List<InMemoryFile>() {
                new InMemoryFile()
                {
                    FileName="جزئیات درخواست تسهیلات.xls",
                    Content=excelFileStream
                }
            });

            await emailSender.SendEmailAsync(new Common.MailRequest()
            {
                Body = $"با سلام<br/>اطلاعات درخواست تسهیلات برای شما ارسال گردید<br/>مدت بازپرداخت : {requestDetail.RequestFacilityDetail.MonthCountTitle} <br/>با تشکر - زرین لند",
                Subject = $"درخواست تسهیلات :{requestDetail.RequestFacilityDetail.Amount.ToString("N0")} ريال",
                ToEmail = "haseli2684@gmail.com;h_ebrahimi@leasingiran.ir;payandehmehrdad@gmail.com",
                //ToEmail = "haseli2684@gmail.com",
                StreamAttachments = new List<MemoryStream>() { documentsZipArchive, detaiExcelZipArchive }
            });

            return true;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> SetGuarantorIsRequired(RequestGuarantorIsRequiredModel model, CancellationToken cancellationToken)
        {
            return await requestFacilityService.SetGuarantorIsRequired(model, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> VerifyByZarinLend(VerifyByZarinLendRequestStatusModel model, CancellationToken cancellationToken)
        {
            model.OpratorId = new Guid(User.Identity!.GetUserId());
            return await requestFacilityService.VerifyByZarinLend(model, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> VerifyCheckByZarinLend(RequestStatusModel model, CancellationToken cancellationToken)
        {
            model.OpratorId = new Guid(User.Identity!.GetUserId());
            return await requestFacilityService.VerifyCheckByZarinLend(model, cancellationToken);
        }

        #endregion ZarinLend Admin

        #region Buyer Methods

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Add([FromForm] RequestFacilityAddModel model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var nationalCode = User.Identity.GetUserName();

            var sanaInquieryDataInputModel = new ZarinLend.Services.Model.NeginHub.SanaInquieryDataInputModel()
            {
                NationalCode = nationalCode,
                Type = 1
            };

            model.BuyerId = new Guid(User.Identity!.GetUserId());
            var newRequestFacilityId = await requestFacilityService.AddRequestFacilityByBuyer(model, cancellationToken);
            var result = new ApiResult<dynamic>(true, ApiResultStatusCode.Success, newRequestFacilityId);
            return result;
        }

        [HttpPost("[action]/{nationalCode}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> FindOpenFacilityRequest(string nationalCode, CancellationToken cancellationToken)
        {
            var data = await requestFacilityService.FindOpenRequestFacility(nationalCode, cancellationToken);
            return new ApiResult<FindOpenRequestFacilityResultModel>(true, ApiResultStatusCode.Success, data);
        }


        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> CancelByUser(int requestFacilityId, CancellationToken cancellationToken)
        {
            await requestFacilityService.CancelByUser(requestFacilityId, new Guid(User.Identity!.GetUserId()), cancellationToken);
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> CancelByZarinLendAdmin(int requestFacilityId, CancellationToken cancellationToken)
        {
            await requestFacilityService.CancelByZarinLendAdmin(requestFacilityId, new Guid(User.Identity!.GetUserId()), cancellationToken);
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<RequestFacilityListModel>> GetBuyerRequests(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await requestFacilityService.GetBuyerRequests(new Guid(User.Identity!.GetUserId()), filter, cancellationToken);
        }

        //[HttpPost("[action]")]
        //[CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public virtual async Task<PagingDto<RequestFacilityListModel>> GetAllBuyerRequests( CancellationToken cancellationToken)
        //{
        //    return await requestFacilityService.GetBuyerRequests(new Guid(User.Identity!.GetUserId()), null, cancellationToken);
        //}

        #endregion Buyer Methods

        #region AdminBankLeasing Role

        public class RequestFacilityIdsForSignModel
        {
            public PagingFilterDto Filter { get; set; }
            public List<int> CheckedRequestFacilityIds { get; set; }
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetAllRequestFacilityIdsForSign(RequestFacilityIdsForSignModel model, CancellationToken cancellationToken)
        {
            var waitingRequestFacilityIDsForSignByBank = await requestFacilityService.GetAllRequestFacilityIdsForSign(User.Identity!.GetUserLeasingId(),
                model.Filter, model.CheckedRequestFacilityIds, new List<RoleEnum> { RoleEnum.AdminBankLeasing }, cancellationToken);

            return new ApiResult<dynamic>(true, ApiResultStatusCode.Success, waitingRequestFacilityIDsForSignByBank);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> ApprovedAdminLeasingWithoutSignContract(RequestStatusModel model, CancellationToken cancellationToken)
        {
            model.OpratorId = new Guid(User.Identity!.GetUserId());
            model.LeasingId = User.Identity!.GetSellerOrganizationId();
            return await requestFacilityService.ApprovedAdminLeasingWithoutSignContract(model, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> ReturnToCorrection(RequestStatusModel model, CancellationToken cancellationToken)
        {
            model.OpratorId = new Guid(User.Identity!.GetUserId());
            model.LeasingId = User.Identity!.GetSellerOrganizationId();
            return await requestFacilityService.ReturnToCorrection(model, cancellationToken);
        }

        #endregion AdminBankLeasing Role

        #region Private Methods

        private async Task<string?> GenerateEnterFacilityNumberExcel(List<PendingInEnterFacilityNumberModel>? personList, CancellationToken cancellationToken = default)
        {
            if (personList == null || personList.Count == 0)
                return null;

            DataTable dt = new DataTable();
            dt.Columns.Add("ردیف");
            dt.Columns.Add("نام");
            dt.Columns.Add("نام خانوادگی");
            dt.Columns.Add("کد ملی");
            dt.Columns.Add("مشتری");
            dt.Columns.Add("حساب متصل به تسهیلات");
            dt.Columns.Add("مبلغ(ريال)");
            dt.Columns.Add("شماره قرارداد تسهیلاتی(68xxx)");

            var rowNumber = 1;
            foreach (var item in personList)
            {
                var row = dt.NewRow();
                row["ردیف"] = rowNumber++;
                row["نام"] = item.RequesterFName;
                row["نام خانوادگی"] = item.RequesterLName;
                row["کد ملی"] = item.NationalCode;
                row["مشتری"] = item.CustomerNumber;
                row["حساب متصل به تسهیلات"] = item.AccountNumber;
                row["مبلغ(ريال)"] = item.Amount.ToString("N0");
                row["شماره قرارداد تسهیلاتی(68xxx)"] = string.Empty;

                dt.Rows.Add(row);
            }

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("درخواستهای در انتظار تنظیم شماره قرارداد");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                //var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "UpdateDate").Start.Column;
                worksheet.Column(6).Style.Numberformat.Format = "Text";
                worksheet.Column(6).Style.Numberformat.Format = "@";
                worksheet.Column(1).Width = 10;

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 25;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }

        private List<FacilityNumberResultModel>? ExtractFacilityNumberFromExcel(Stream stream)
        {
            using (var excelPackage = new ExcelPackage(stream))
            {
                var result = new List<FacilityNumberResultModel>();
                FacilityNumberResultModel? item = null;
                if (excelPackage.Workbook.Worksheets.Count > 0)
                {
                    var workSheet = excelPackage.Workbook.Worksheets[0];
                    var nationalCodeColumn = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value != null && c.Value.ToString()!.CleanString().Replace(" ", string.Empty) == "کدملی");
                    if (nationalCodeColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'کد ملی' در فایل اکسل یافت نشد");

                    var facilityNumberColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null &&
                        c.Value.ToString()!.Replace(" ", string.Empty).CleanString() == "شماره قرارداد تسهیلاتی(68xxx)".CleanString().Replace(" ", string.Empty));
                    if (facilityNumberColumn == null)
                        throw new AppException($"ستون شماره قرارداد تسهیلاتی(68xxx) یافت نشد");

                    var dataRowIndex = 2;
                    var regexNationalCode = new Regex(RegularExpression.NationalCode);
                    var regexFacilityNumber = new Regex(RegularExpression.FacilityNumber);
                    while (workSheet.Dimension.End.Row >= dataRowIndex)
                    {
                        item = new FacilityNumberResultModel();
                        //if (workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value == null ||
                        //    string.IsNullOrEmpty(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString().Replace(" ", String.Empty)))
                        //{
                        //    break;
                        //}
                        if (!regexNationalCode.IsMatch(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString()))
                        {
                            item.NationalCode = Convert.ToString(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value);
                            item.ErrorMessage = $"کد ملی وارد شده در سطر {dataRowIndex}ام با فرمت کد ملی مطابقت ندارد";
                            item.HasError = true;
                            result.Add(item);
                            break;
                        }
                        item.NationalCode = workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString();

                        if (!regexFacilityNumber.IsMatch(workSheet.Cells[dataRowIndex, facilityNumberColumn.Start.Column].Value.ToString()))
                        {
                            item.FacilityNumber = Convert.ToString(workSheet.Cells[dataRowIndex, facilityNumberColumn.Start.Column].Value);
                            item.ErrorMessage = $"شماره قرارداد/تسهیلات وارد شده در سطر {dataRowIndex}ام با فرمت شماره قراداد/تسهیلات مطابقت ندارد";
                            item.HasError = true;
                            result.Add(item);
                            break;
                        }
                        item.FacilityNumber = Convert.ToString(workSheet.Cells[dataRowIndex, facilityNumberColumn.Start.Column].Value);

                        result.Add(item);
                        dataRowIndex++;
                    }
                }
                return result;
            }
        }

        private async Task<string?> GenerateBankLeasingInquiryExcel(List<PendingInEnterPoliceNumberModel>? personList, CancellationToken cancellationToken = default)
        {
            if (personList == null || personList.Count == 0)
                return null;

            DataTable dt = new DataTable();
            dt.Columns.Add("ردیف");
            dt.Columns.Add("کد ملی");
            dt.Columns.Add("نام و نام خانوادگی");
            dt.Columns.Add("مبلغ تسهیلات");
            dt.Columns.Add("شماره مشتری");
            dt.Columns.Add("شماره تسهیلات");
            dt.Columns.Add("شماره حساب");
            dt.Columns.Add("شماره خزانه داری سفته");
            dt.Columns.Add("شماره انتظامی");
            var rowNumber = 1;
            foreach (var item in personList)
            {
                var row = dt.NewRow();
                row["ردیف"] = rowNumber++;
                row["کد ملی"] = item.NationalCode;
                row["نام و نام خانوادگی"] = item.Requester;
                row["مبلغ تسهیلات"] = item.Amount.ToString("N0");
                row["شماره مشتری"] = item.CustomerNumber;
                row["شماره تسهیلات"] = item.FacilityNumber;
                row["شماره حساب"] = item.AccountNumber;
                row["شماره خزانه داری سفته"] = item.PromissoryNumber;
                row["شماره انتظامی"] = string.Empty;
                dt.Rows.Add(row);
            }

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("درخواستهای در انتظار تنظیم شماره انتظامی");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                //var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "UpdateDate").Start.Column;
                worksheet.Column(1).Width = 10;
                worksheet.Column(6).Style.Numberformat.Format = "Text";
                worksheet.Column(6).Style.Numberformat.Format = "@";

                worksheet.Column(7).Style.Numberformat.Format = "Text";
                worksheet.Column(7).Style.Numberformat.Format = "@";

                worksheet.Column(8).Style.Numberformat.Format = "Text";
                worksheet.Column(8).Style.Numberformat.Format = "@";

                worksheet.Column(9).Style.Numberformat.Format = "Text";
                worksheet.Column(9).Style.Numberformat.Format = "@";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 30;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }

        private async Task<string?> GenerateDepositFacilityAmountExcel(List<PendingInEnterPoliceNumberModel>? personList, CancellationToken cancellationToken = default)
        {
            if (personList == null || personList.Count == 0)
                return null;

            DataTable dt = new DataTable();
            dt.Columns.Add("ردیف");
            dt.Columns.Add("نام");
            dt.Columns.Add("نام خانوادگی");
            dt.Columns.Add("کد ملی");
            dt.Columns.Add("مشتری");
            dt.Columns.Add("حساب متصل به حساب");
            dt.Columns.Add("مبلغ(ريال)");
            dt.Columns.Add("شماره قرارداد تسهیلات(68xxx)");
            dt.Columns.Add("شماره انتظامی");
            dt.Columns.Add("شماره خزانه داری سفته");
            var rowNumber = 1;
            foreach (var item in personList)
            {
                var row = dt.NewRow();
                row["ردیف"] = rowNumber++;
                row["نام"] = item.RequesterFName;
                row["نام خانوادگی"] = item.RequesterLName;
                row["کد ملی"] = item.NationalCode;
                row["مشتری"] = item.CustomerNumber;
                row["حساب متصل به حساب"] = item.AccountNumber;
                row["مبلغ(ريال)"] = item.Amount;
                row["شماره قرارداد تسهیلات(68xxx)"] = item.FacilityNumber;
                row["شماره انتظامی"] = item.PoliceNumber;
                row["شماره خزانه داری سفته"] = item.PromissoryNumber;
                dt.Rows.Add(row);
            }

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("درخواستهای در انتظار تنظیم شماره انتظامی");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                //var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "UpdateDate").Start.Column;
                worksheet.Column(1).Width = 10;
                worksheet.Column(6).Style.Numberformat.Format = "#,##0";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 30;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }

        private async Task<string?> GenerateCardRechargeExcel(List<PendingInCardRechargeModel>? personList, CancellationToken cancellationToken = default)
        {
            if (personList == null || personList.Count == 0)
                return null;

            DataTable dt = new DataTable();
            //dt.Columns.Add("ردیف");
            dt.Columns.Add("شماره حساب");
            dt.Columns.Add("مبلغ(ريال)");
            dt.Columns.Add("کد ملی");
            dt.Columns.Add("نام");
            dt.Columns.Add("نام خانوادگی");
            //dt.Columns.Add("شماره مشتری");
            dt.Columns.Add("شماره قرارداد");
            dt.Columns.Add("شماره بن کارت");

            //var rowNumber = 1;
            foreach (var item in personList)
            {
                var row = dt.NewRow();
                //row["ردیف"] = rowNumber++;
                row["شماره حساب"] = item.AccountNumber;
                row["مبلغ(ريال)"] = item.Amount;
                row["کد ملی"] = item.NationalCode;
                row["نام"] = item.RequesterFName;
                row["نام خانوادگی"] = item.RequesterLName;
                //row["شماره مشتری"] = item.CustomerNumber;
                row["شماره قرارداد"] = item.FacilityNumber;
                row["شماره بن کارت"] = item.CardNumber;

                dt.Rows.Add(row);
            }

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("درخواستهای در انتظار شارژ بن کارت");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                //var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "UpdateDate").Start.Column;
                //worksheet.Column(2).Style.Numberformat.Format = "Number";
                worksheet.Column(2).Style.Numberformat.Format = "#,##0";
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 20;
                worksheet.Column(6).Width = 20;
                worksheet.Column(7).Width = 20;
                worksheet.Column(8).Width = 20;

                //worksheet.Column(8).Style.Numberformat.Format = "Text";
                //worksheet.Column(8).Style.Numberformat.Format = "@";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 30;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }

        private List<PoliceNumberResultModel> ExtractPoliceNumberFromExcel(Stream stream)
        {
            using (var excelPackage = new ExcelPackage(stream))
            {
                var result = new List<PoliceNumberResultModel>();
                PoliceNumberResultModel item = null;
                if (excelPackage.Workbook.Worksheets.Count > 0)
                {
                    var workSheet = excelPackage.Workbook.Worksheets[0];
                    var nationalCodeColumn = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value != null && c.Value.ToString().CleanString().Replace(" ", string.Empty) == "کدملی");
                    if (nationalCodeColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'کد ملی' در فایل اکسل یافت نشد");

                    var policeNumberColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null &&
                        (c.Value.ToString().Replace(" ", string.Empty).CleanString() == "شماره انتظامی".CleanString().Replace(" ", string.Empty)));
                    if (policeNumberColumn == null)
                        throw new AppException($"ستون شماره انتظامی یافت نشد");

                    var dataRowIndex = 2;
                    var regexNationalCode = new Regex(RegularExpression.NationalCode);
                    var regexPoliceNumber = new Regex(RegularExpression.PoliceNumber);
                    while (workSheet.Dimension.End.Row >= dataRowIndex)
                    {
                        item = new PoliceNumberResultModel();
                        //if (workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value == null ||
                        //    string.IsNullOrEmpty(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString().Replace(" ", String.Empty)))
                        //{
                        //    break;
                        //}
                        if (!regexNationalCode.IsMatch(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString()))
                        {
                            item.NationalCode = Convert.ToString(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value);
                            item.ErrorMessage = $"کد ملی وارد شده در سطر {dataRowIndex}ام با فرمت کد ملی مطابقت ندارد";
                            item.HasError = true;
                            result.Add(item);
                            break;
                        }
                        item.NationalCode = workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString();

                        if (!regexPoliceNumber.IsMatch(workSheet.Cells[dataRowIndex, policeNumberColumn.Start.Column].Value.ToString()))
                        {
                            item.PoliceNumber = Convert.ToString(workSheet.Cells[dataRowIndex, policeNumberColumn.Start.Column].Value);
                            item.ErrorMessage = $"شماره انتظامی وارد شده در سطر {dataRowIndex}ام با فرمت شماره انتظامی مطابقت ندارد";
                            item.HasError = true;
                            result.Add(item);
                            break;
                        }
                        item.PoliceNumber = Convert.ToString(workSheet.Cells[dataRowIndex, policeNumberColumn.Start.Column].Value);

                        result.Add(item);
                        dataRowIndex++;
                    }
                }
                return result;
            }
        }

        private List<PoliceNumberResultModel> ExtractDepositFacilityAmountFromExcel(Stream stream)
        {
            using (var excelPackage = new ExcelPackage(stream))
            {
                var result = new List<PoliceNumberResultModel>();
                PoliceNumberResultModel? item = null;
                if (excelPackage.Workbook.Worksheets.Count > 0)
                {
                    var workSheet = excelPackage.Workbook.Worksheets[0];
                    var nationalCodeColumn = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value != null && c.Value.ToString()!.CleanString().Replace(" ", string.Empty) == "کدملی");
                    if (nationalCodeColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'کد ملی' در فایل اکسل یافت نشد");

                    var dataRowIndex = 2;
                    var regexNationalCode = new Regex(RegularExpression.NationalCode);
                    var regexPoliceNumber = new Regex(RegularExpression.PoliceNumber);
                    while (workSheet.Dimension.End.Row >= dataRowIndex)
                    {
                        item = new PoliceNumberResultModel();
                        if (!regexNationalCode.IsMatch(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString()!))
                        {
                            item.NationalCode = Convert.ToString(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value)!;
                            item.ErrorMessage = $"کد ملی وارد شده در سطر {dataRowIndex}ام با فرمت کد ملی مطابقت ندارد";
                            item.HasError = true;
                            result.Add(item);
                            break;
                        }
                        item.NationalCode = workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString()!.CleanString();

                        result.Add(item);
                        dataRowIndex++;
                    }
                }
                return result;
            }
        }

        private async Task<string> GenerateVerifyOctectExcel(List<PersonCompleteInfoModel> personList, CancellationToken cancellationToken = default)
        {
            if (personList == null || personList.Count == 0)
                return null;

            DataTable dt = new DataTable();
            dt.Columns.Add("ردیف");
            dt.Columns.Add("کد ملی");
            dt.Columns.Add("شماره شناسنامه");
            dt.Columns.Add("تاریخ تولد");
            dt.Columns.Add("نوع شخص");
            dt.Columns.Add("کد پستی");
            dt.Columns.Add("نام");
            dt.Columns.Add("نام خانوادگی");
            dt.Columns.Add("جنسیت");
            dt.Columns.Add("نام پدر");
            dt.Columns.Add("نام شرکت");
            dt.Columns.Add("تاریخ ثبت");
            dt.Columns.Add("شماره ثبت");
            dt.Columns.Add("ملیت");
            dt.Columns.Add("آدرس");
            dt.Columns.Add("شماره موبایل");
            //dt.Columns.Add("محل تولد");
            dt.Columns.Add("استعلام ثبت احوال");
            dt.Columns.Add("استعلام چک برگشتی");
            dt.Columns.Add("استعلام تسهیلات و تعهدات");
            dt.Columns.Add("استعلام کدپستی به آدرس");
            dt.Columns.Add("استعلام تحریم شورا امنیت");
            dt.Columns.Add("استعلام شاهکار");
            dt.Columns.Add("استعلام نظام وظیفه");
            dt.Columns.Add("استعلام لیست سیاه");
            var rowNumber = 1;
            foreach (var item in personList)
            {
                var row = dt.NewRow();
                row["ردیف"] = rowNumber++;
                row["کد ملی"] = item.NationalCode;
                row["شماره شناسنامه"] = item.SSID;
                row["تاریخ تولد"] = DateTimeHelper.GregorianToShamsi(item.BirthDate, string.Empty);
                row["نوع شخص"] = item.UserType;
                row["کد پستی"] = item.PostalCode;
                row["نام"] = item.FName;
                row["نام خانوادگی"] = item.LName;
                row["جنسیت"] = item.Gender == GenderEnum.Male ? "مرد" : "زن";
                row["نام پدر"] = item.FatherName;
                row["نام شرکت"] = string.Empty;
                row["تاریخ ثبت"] = string.Empty;
                row["شماره ثبت"] = string.Empty;
                row["ملیت"] = item.Nationality;
                row["آدرس"] = item.Address;
                row["شماره موبایل"] = item.Mobile;
                //row["محل تولد"] = $"{item.ProvinceOfBirth}-{item.CityOfBirth}";
                row["استعلام ثبت احوال"] = string.Empty;
                row["استعلام چک برگشتی"] = string.Empty;
                row["استعلام تسهیلات و تعهدات"] = string.Empty;
                row["استعلام کدپستی به آدرس"] = string.Empty;
                row["استعلام تحریم شورا امنیت"] = string.Empty;
                row["استعلام شاهکار"] = string.Empty;
                row["استعلام نظام وظیفه"] = string.Empty;
                row["استعلام لیست سیاه"] = string.Empty;
                dt.Rows.Add(row);
            }

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("اطلاعات هویتی درخواست دهنده");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                //var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "UpdateDate").Start.Column;
                worksheet.Cells[2, 1].Style.Numberformat.Format = "Text";
                worksheet.Cells[2, 1].Style.Numberformat.Format = "@";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 22;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }

        private async Task<string?> GenerateExcel(IEnumerable<RequestFacilityListModel> requestList, CancellationToken cancellationToken = default)
        {
            if (requestList == null || requestList.Count() == 0)
                return null;

            var result = requestList.Select(p => new
            {
                p.Amount,
                p.MonthCountTitle,
                p.RequesterFName,
                p.RequesterLName,
                p.ShamsiBirthDate,
                p.NationalCode,
                p.RequesterMobile,
                CancelByUser = p.CancelByUser ? "بله" : "خیر",
                LastStatusDescription = $"{p.LastStatusDescription}{(p.CancelByUser ? "(انصراف توسط کاربر)" : string.Empty)}",
                p.ShamsiCreateDate,
                p.ShamsiLastActionDate,
            });

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("درخواست های تسهیلات");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromCollection(result, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Amount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 22;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.SetValue(1, 1, "مبلغ درخواستی");
                worksheet.SetValue(1, 2, "بازه پرداخت");
                worksheet.SetValue(1, 3, "نام درخواست کننده");
                worksheet.SetValue(1, 4, "نام خانوادگی درخواست کننده");
                worksheet.SetValue(1, 5, "تاریخ تولد درخواست کننده");
                worksheet.SetValue(1, 6, "کد ملی/شناسه ملی");
                worksheet.SetValue(1, 7, "موبایل درخواست کننده");
                worksheet.SetValue(1, 8, "انصراف توسط کاربر");
                worksheet.SetValue(1, 9, "مرحله فعلی");
                worksheet.SetValue(1, 10, "تاریخ درخواست");
                worksheet.SetValue(1, 11, "تاریخ آخرین اقدام");

                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }

        #endregion Private Methods


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, RoleEnum.Buyer, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, RoleEnum.AdminBankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<SanaInquieryDataResultModel> SanaInquiry(SanaInquieryDataInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());

            var sanhabInquiryDataResult = await neginHubService.SanaInquiryV5(model, userId, cancellationToken);
            if (sanhabInquiryDataResult is not null && sanhabInquiryDataResult.IsSuccess)
                await personService.SetSanaTrackingCodeAsync(model.NationalCode, sanhabInquiryDataResult.TrackId, cancellationToken);

            return sanhabInquiryDataResult;
        }
    }
}
