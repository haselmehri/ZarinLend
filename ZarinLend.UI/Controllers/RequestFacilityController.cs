using Common;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.Model;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class RequestFacilityController : BaseMvcController
    {
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IUserService userService;
        private readonly ISamatService samatService;
        private readonly ITransactionService transactionService;
        private readonly IZarinPalInternetPaymentService zarinPalInternetPaymentService;
        private readonly ISamanInternetPaymentService samanInternetPaymentService;
        private readonly IIranCreditScoringService iranCreditScoringService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly IBaseRepository<WorkFlowStepRejectionReason> bankRepository;

        public RequestFacilityController(IRequestFacilityService requestFacilityService,
                                         IUserService userService, ISamatService samatService,
                                         ITransactionService transactionService,
                                         IZarinPalInternetPaymentService zarinPalInternetPaymentService,
                                         ISamanInternetPaymentService samanInternetPaymentService,
                                         IIranCreditScoringService iranCreditScoringService,
                                         IGlobalSettingService globalSettingService,
                                         IBaseRepository<WorkFlowStepRejectionReason> bankRepository)
        {
            this.requestFacilityService = requestFacilityService;
            this.userService = userService;
            this.samatService = samatService;
            this.transactionService = transactionService;
            this.zarinPalInternetPaymentService = zarinPalInternetPaymentService;
            this.samanInternetPaymentService = samanInternetPaymentService;
            this.iranCreditScoringService = iranCreditScoringService;
            this.globalSettingService = globalSettingService;
            this.bankRepository = bankRepository;
        }

        #region Buyer/Facility Requester

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult> Add(CancellationToken cancellationToken)
        {
            var setting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            var userId = new Guid(User.Identity!.GetUserId());
            IranCreditScoringModel iranCreditScoringResult;
            if (/*(await samanInternetPaymentService.ExistSuccessfulPayment(userId, PaymentType.PayValidationFee, cancellationToken) ||
                 await zarinPalInternetPaymentService.ExistSuccessfulPayment(userId, PaymentType.PayValidationFee, cancellationToken)) 
                 await transactionService.GetBalance(userId, cancellationToken) >= setting.ValidationFee ||&&*/
                 await iranCreditScoringService.ExistVerifyResult(userId, expireAfterFewDays: setting.ValidityPeriodOfValidation, cancellationToken))
            {
                iranCreditScoringResult = await iranCreditScoringService.GetVerifyResult(userId, cancellationToken);
                var model = await requestFacilityService.PrepareModelForAdd(userId, iranCreditScoringResult.Risk!, cancellationToken);
                model.IranCreditScoringResult = iranCreditScoringResult;
                model.RegisterFromZarinpal = TempData["RegisterFromZarinpal"] != null && Convert.ToBoolean(TempData["RegisterFromZarinpal"]) == true;

                return View(model: model);
            }
            else
                return RedirectToAction("InternetPayment", "Payment");
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public ActionResult List(CancellationToken cancellationToken)
        {
            return View();
        }

        #endregion

        #region Leasing/Bank/AdminBank Roles Actions

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]")]
        public ActionResult VerifyResultExcelHistory()
        {
            ViewBag.Title = "تاریخچه استعلامات گروهی";
            return View(nameof(VerifyResultExcelHistory));
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingCardRechargeRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار شارژ بن کارت";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();
            ViewBag.OrganizationUsers = await userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken);

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100025;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert)]
        public async Task<ActionResult> PendingCompleteBonCardRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار تکمیل اطلاعات بن کارت";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 1000251;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingDepositFacilityAmountRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار واریز تسهیلات";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();
            ViewBag.OrganizationUsers = await userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken);

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100024;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingResultDepositFacilityAmountRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار نتیجه واریز تسهیلات";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 1000241;
            ViewBag.OrganizationUsers = userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken).Result;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing)]
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> AssignRequestFacilityToUser(CancellationToken cancellationToken)
        {
            ViewBag.Title = "اختصاص درخواست های تسهیلات به کارشناسان";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.OrganizationUsers = await userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken);
            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing }
            , cancellationToken);
            return View(nameof(RequestFacilityController.AssignRequestFacilityToUser), model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> AdminBankLeasingSignature(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی پرونده-امضاء قرارداد توسط مدیر بانک";
            var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, User.Identity.GetUserLeasingId(), new List<RoleEnum> { RoleEnum.AdminBankLeasing }, WorkFlowFormEnum.AdminBankLeasingSignature, cancellationToken);
            if (status)
            {
                var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(), WorkFlowFormEnum.AdminBankLeasingSignature, cancellationToken);
                var rejectionReasons = model.RequestFacilityWorkFlowStepList.FirstOrDefault(f => f.StatusId == null).WorkFlowStepRejectionReasons;
                ViewBag.RejectionReasons = rejectionReasons.Select(s => new SelectListItem()
                {
                    Text = s.RejectionReason.Name,
                    Value = s.Id.ToString()
                });
                return View("VerifyBuyerByLeasing", model);
            }

            return RedirectToAction(nameof(SearchLeasingRequest));
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingInquiryRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار استعلام";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();
            ViewBag.OrganizationUsers = await userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken);

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 10009;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingResultInquiryRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار نتیجه استعلام";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100010;
            ViewBag.OrganizationUsers = userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken).Result;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingEnterFacilityNumberRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار شماره قرارداد";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();
            ViewBag.OrganizationUsers = await userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken);

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100011;
            ViewBag.OrganizationUsers = await userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken);

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingResultEnterFacilityNumberRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار ثبت شماره قرارداد";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100012;
            ViewBag.OrganizationUsers = userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken).Result;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingBankLeasingInquiryRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار ثبت شماره انتظامی";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();
            ViewBag.OrganizationUsers = await userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken);

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100021;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public async Task<ActionResult> PendingResultBankLeasingInquiryRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار نتیجه ثبت شماره انتظامی";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100022;
            ViewBag.OrganizationUsers = userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken).Result;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.BankLeasing }, cancellationToken);

            return View(model);
        }


        [CustomAuthorize(RoleEnum.AdminBankLeasing)]
        public async Task<ActionResult> PendingConfirmAndSignatureRequests(CancellationToken cancellationToken)
        {
            ViewBag.Title = "درخواست های در انتظار تایید و امضاء";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            ViewBag.PendingWorkFlowId = 100023;

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.AdminBankLeasing }, cancellationToken);

            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public ActionResult LeasingRequests(CancellationToken cancellationToken)
        {
            return View();
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> VerifyCheckByLeasing(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی تصویر چک توسط نهاد مالی";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(), WorkFlowFormEnum.VerifyCheckByLeasing, cancellationToken);
            return View("VerifyBuyerByLeasing", model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> VerifyBuyerByLeasing(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی توسط کارشناس نهاد مالی(اخذ استعلام 8 گانه)";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(),
                WorkFlowFormEnum.VerifyLeasing, cancellationToken);
            return View(model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> EnterFacilityNumber(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "ثبت شماره قرارداد/تسهیلات";

            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(), WorkFlowFormEnum.EnterFacilityNumber, cancellationToken);
            var rejectionReasons = model.RequestFacilityWorkFlowStepList.FirstOrDefault(f => f.StatusId == null).WorkFlowStepRejectionReasons;
            ViewBag.RejectionReasons = rejectionReasons.Select(s => new SelectListItem() 
            {
                Text = s.RejectionReason.Name,
                Value = s.Id.ToString()
            });

            return View("VerifyBuyerByLeasing", model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> PendingEnterFacilityNumber(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "ثبت شماره قرارداد/تسهیلات";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(), WorkFlowFormEnum.PendingEnterFacilityNumber, cancellationToken);
            var rejectionReasons = model.RequestFacilityWorkFlowStepList.FirstOrDefault(f => f.StatusId == null).WorkFlowStepRejectionReasons;
            ViewBag.RejectionReasons = rejectionReasons.Select(s => new SelectListItem()
            {
                Text = s.RejectionReason.Name,
                Value = s.Id.ToString()
            });
            return View("VerifyBuyerByLeasing", model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> PendingForVerifyResult(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "در انتظار نتیجه استعلام 8 گانه";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(), WorkFlowFormEnum.PendingForVerifyResult, cancellationToken);
            return View("VerifyBuyerByLeasing", model);
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> BankLeasingInquiry(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی پرونده-اخذ استعلام های لازم-تخصیص اعتبار";

            var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, User.Identity.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.BankLeasingInquiry, cancellationToken);
            if (status)
            {
                var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(), WorkFlowFormEnum.BankLeasingInquiry, cancellationToken);
                var rejectionReasons = model.RequestFacilityWorkFlowStepList.FirstOrDefault(f => f.StatusId == null).WorkFlowStepRejectionReasons;
                ViewBag.RejectionReasons = rejectionReasons.Select(s => new SelectListItem()
                {
                    Text = s.RejectionReason.Name,
                    Value = s.Id.ToString()
                });

                return View("VerifyBuyerByLeasing", model);
            }

            return RedirectToAction(nameof(SearchLeasingRequest));
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> PendingBankLeasingInquiry(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی پرونده-اخذ استعلام های لازم-تخصیص اعتبار";
            var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.PendingBankLeasingInquiry, cancellationToken);
            if (status)
            {
                var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity!.GetUserLeasingId(), WorkFlowFormEnum.PendingBankLeasingInquiry, cancellationToken);
                var rejectionReasons = model.RequestFacilityWorkFlowStepList.FirstOrDefault(f => f.StatusId == null).WorkFlowStepRejectionReasons;
                ViewBag.RejectionReasons = rejectionReasons.Select(s => new SelectListItem()
                {
                    Text = s.RejectionReason.Name,
                    Value = s.Id.ToString()
                });
                return View("VerifyBuyerByLeasing", model);
            }

            return RedirectToAction(nameof(SearchLeasingRequest));
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> DepositFacilityAmount(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "واریز مبلغ تسهیلات به حساب زرین لند";
            var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.DepositFacilityAmount, cancellationToken);
            if (status)
            {
                var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity!.GetUserLeasingId(), WorkFlowFormEnum.DepositFacilityAmount, cancellationToken);
                var rejectionReasons = model.RequestFacilityWorkFlowStepList.FirstOrDefault(f => f.StatusId == null).WorkFlowStepRejectionReasons;
                ViewBag.RejectionReasons = rejectionReasons.Select(s => new SelectListItem()
                {
                    Text = s.RejectionReason.Name,
                    Value = s.Id.ToString()
                });

                return View("VerifyBuyerByLeasing", model);
            }

            return RedirectToAction(nameof(SearchLeasingRequest));
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> PendingDepositFacilityAmount(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "واریز مبلغ تسهیلات به حساب زرین لند";
            var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, WorkFlowFormEnum.PendingDepositFacilityAmount, cancellationToken);
            if (status)
            {
                var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity!.GetUserLeasingId(), WorkFlowFormEnum.PendingDepositFacilityAmount, cancellationToken);
                var rejectionReasons = model.RequestFacilityWorkFlowStepList.FirstOrDefault(f => f.StatusId == null).WorkFlowStepRejectionReasons;
                ViewBag.RejectionReasons = rejectionReasons.Select(s => new SelectListItem()
                {
                    Text = s.RejectionReason.Name,
                    Value = s.Id.ToString()
                });
                return View("VerifyBuyerByLeasing", model);
            }

            return RedirectToAction(nameof(SearchLeasingRequest));
        }

        #endregion Leasing/Bank Actions

        #region SuperAdmin/Admin Roles Actions

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert)]
        public ActionResult SearchRequest(CancellationToken cancellationToken)
        {
            //ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            //ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            //ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            //ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            //var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert }, cancellationToken);
            return View();
        }

        [CustomAuthorize(RoleEnum.ZarinLendExpert, RoleEnum.SuperAdmin, RoleEnum.Admin)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> CompleteBonCardInfo(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "تکمیل اطلاعات بن کارت";
            var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
                new List<RoleEnum> { RoleEnum.SuperAdmin, RoleEnum.Admin }, WorkFlowFormEnum.CompleteBonCardInfo, cancellationToken);
            if (status)
            {
                var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, WorkFlowFormEnum.CompleteBonCardInfo, cancellationToken);
                return View("VerifyBuyerByZarinLend", model);
            }
            return RedirectToAction(nameof(SearchRequest));
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> VerifyCheckByZarinLend(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی تصویر چک توسط زرین لند";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, WorkFlowFormEnum.VerifyCheckByZarinLend, cancellationToken);
            return View("VerifyBuyerByZarinLend", model);
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> InsuranceIssuance(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "صدور بیمه و اطلاع رسانی به مشتری";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, WorkFlowFormEnum.InsuranceIssuance, cancellationToken);
            return View("VerifyBuyerByZarinLend", model);
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> CardIssuance(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "صدور بن کارت";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, WorkFlowFormEnum.CardIssuance, cancellationToken);
            return View("VerifyBuyerByZarinLend", model);
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> VerifyBuyerByZarinLend(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی درخواست توسط زرین لند";
            var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, WorkFlowFormEnum.VerifyZarrinLend, cancellationToken);
            return View(model);
        }

        #endregion SuperAdmin/Admin Roles Actions

        #region All Roles Actions

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public ActionResult RenderRequestFacilityRenderStepsHistoryPartial(int requestFacilityId)
        {
            return PartialView(viewName: "_RequestFacilityWorkFlowStepsHistory", model: requestFacilityId);
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.BankLeasing, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> Detail(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "جزئیات درخواست";
            if (User.IsInRole(RoleEnum.BankLeasing.ToString()) || User.IsInRole(RoleEnum.AdminBankLeasing.ToString()) || User.IsInRole(RoleEnum.SupervisorLeasing.ToString()))
                return View("VerifyBuyerByLeasing", await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity.GetUserLeasingId(), cancellationToken));
            else if (User.IsInRole(RoleEnum.Buyer.ToString()))
                return View("Detail", await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, new Guid(User.Identity.GetUserId()), cancellationToken));
            else
                return View("VerifyBuyerByZarinLend", await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, cancellationToken));
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.BankLeasing, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing)]
        public ActionResult SearchLeasingRequest(CancellationToken cancellationToken)
        {
            ViewBag.Title = "جستجوی پیشرفته";
            ViewBag.IsInSearchLeasingRequest = true;
            ViewBag.OrganizationUsers = userService.GetOrganizationUsers(User.Identity!.GetUserLeasingId(), cancellationToken).Result;

            //ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            //ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            //ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            //ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            //RequestFacilityFilterModel model;
            //if (User.IsInRole(RoleEnum.BankLeasing.ToString()) || User.IsInRole(RoleEnum.AdminBankLeasing.ToString()) || User.IsInRole(RoleEnum.SupervisorLeasing.ToString()))
            //    model = await requestFacilityService
            //        .PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing }, cancellationToken);
            //else
            //    model = await requestFacilityService.PrepareFilterModelForSearch(null, cancellationToken);

            return View();
        }

        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> CardRecharge(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "شارژ بن کارت";
            if (User.IsInRole(RoleEnum.BankLeasing.ToString()) || User.IsInRole(RoleEnum.AdminBankLeasing.ToString()) || User.IsInRole(RoleEnum.SupervisorLeasing.ToString()))
            {
                var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, }, WorkFlowFormEnum.CardRecharge, cancellationToken);
                if (status)
                {
                    var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, User.Identity!.GetUserLeasingId(), WorkFlowFormEnum.CardRecharge, cancellationToken);
                    return View("VerifyBuyerByLeasing", model);
                }
                return RedirectToAction(nameof(SearchLeasingRequest));
            }
            else
            {
                var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
                    new List<RoleEnum> { RoleEnum.SuperAdmin, RoleEnum.Admin }, WorkFlowFormEnum.CardRecharge, cancellationToken);
                if (status)
                {
                    var model = await requestFacilityService.GetRequestFacilityCompleteInfo(requestFacilityId, WorkFlowFormEnum.CardRecharge, cancellationToken);
                    return View("VerifyBuyerByZarinLend", model);
                }
                return RedirectToAction(nameof(SearchRequest));
            }
        }

        #endregion All Roles Actions

        #region اعتبار سنجی سمات و شاهکار

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult<RequestFacilityDetailModel>> InquiryShahkarAndSamatService(int requestFacilityId, CancellationToken cancellationToken)
        {
            //var userId = new Guid(User.Identity.GetUserId());
            if (await samatService.InquiryDone(requestFacilityId, cancellationToken))
                return View(model: requestFacilityId);

            return RedirectToAction("List");
        }

        //[CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.Buyer)]
        //[HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        //public async Task<ActionResult<RequestFacilityDetailModel>> VerifyShahkarAndSamatService(int requestFacilityId, CancellationToken cancellationToken)
        //{
        //    var userId = new Guid(User.Identity.GetUserId());
        //    RequestFacilityDetailModel requestFacilityDetail;
        //    var requestListUrl = Url.Action("SearchRequest", "RequestFacility");
        //    if (User.IsInRole(RoleEnum.SuperAdmin.ToString()) || User.IsInRole(RoleEnum.Admin.ToString()))
        //    {
        //        requestFacilityDetail = await requestFacilityService.GetRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
        //           new List<RoleEnum> { RoleEnum.SuperAdmin, RoleEnum.Admin },
        //           WorkFlowFormEnum.VerifyShahkarAndSamatService, cancellationToken);
        //    }
        //    else
        //    {
        //        requestFacilityDetail = await requestFacilityService.GetRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, userId,
        //           new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.VerifyShahkarAndSamatService, cancellationToken);
        //        requestListUrl = Url.Action("List", "RequestFacility");
        //    }

        //    if (requestFacilityDetail != null)
        //    {
        //        if (await samatService.GetUserFacilitiesFromCentralBank(requestFacilityId, requestFacilityDetail.BuyerId, userId, cancellationToken) &&
        //            await samatService.GetUserBackChequesFromCentralBank(requestFacilityId, requestFacilityDetail.BuyerId, userId, cancellationToken))
        //        {
        //            await transactionService.Withdrawal(-PaymentController.VERIFY_PAYMENT_FEE_AMOUNT, requestFacilityDetail.BuyerId, userId, requestFacilityId);
        //            await zarinPalInternetPaymentService.ApprovedVerifyShahkarAndSamatServiceStep(requestFacilityDetail.BuyerId, userId, requestFacilityId, cancellationToken);
        //            return View(model: requestFacilityDetail);
        //        }
        //    }

        //    return Redirect(requestListUrl);
        //}

        //[CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin)]
        //[HttpGet("[controller]/[action]/{userId:guid}/{requestFacilityId:int}")]
        //public async Task<ActionResult<RequestFacilityDetailModel>> GetFacilityHistoryFromCentralBank(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        //{
        //    await samatService.GetUserFacilitiesFromCentralBank(userId, creatorId: new Guid(User.Identity.GetUserId()), cancellationToken);
        //    await samatService.GetUserBackChequesFromCentralBank(userId, creatorId: new Guid(User.Identity.GetUserId()), cancellationToken);
        //    return RedirectToAction("VerifyShahkarAndSamatService", "RequestFacility", new { requestFacilityId });
        //}
        #endregion
    }
}