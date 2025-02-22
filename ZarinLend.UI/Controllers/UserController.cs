using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using Services.Model;
using Services.Model.ZarinPal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Configuration;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class UserController : BaseMvcController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService userService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;

        public UserController(SignInManager<User> signInManager, IUserService userService, IRequestFacilityService requestFacilityService,
            IRequestFacilityGuarantorService requestFacilityGuarantorService)
        {
            _signInManager = signInManager;
            this.userService = userService;
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
        }

        [CustomAuthorize(RoleEnum.SuperAdmin)]
        public async Task<ActionResult<UserAddEditModelByAdmin>> AddUser(CancellationToken cancellationToken)
        {
            var model = await userService.PrepareModelForAddByAdmin(cancellationToken);
            return View("AddEditUser", model);
        }

        [CustomAuthorize(RoleEnum.SuperAdmin)]
        public async Task<ActionResult<UserAddEditModelByAdmin>> EditUser(Guid Id, CancellationToken cancellationToken)
        {
            var userModel = await userService.PrepareModelForEditByAdmin(Id, cancellationToken);
            ViewBag.BirthDate = userModel.BirthDate.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(userModel.BirthDate);
            userModel.IsEditMode = true;
            return View(viewName: "AddEditUser", model: userModel);
        }

        [CustomAuthorize(RoleEnum.SuperAdmin)]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var model = await userService.PrepareFilterModel(cancellationToken);
            return View(model);
        }

        [CustomAuthorize(RoleEnum.Seller)]
        public async Task<ActionResult<UserSearchModel>> SearchUser(CancellationToken cancellationToken)
        {
            ViewBag.Error = TempData["Error"]?.ToString();
            return View(new UserSearchModel());
        }

        [AllowAnonymous]
        public async Task<ActionResult<UserRegisterFirstStepModel>> RegisterStep1(CancellationToken cancellationToken, string returnUrl = null)
        {
            var model = new UserRegisterFirstStepModel
            {
                ReturnUrl = Url.Action(nameof(RegisterStep2))
            };
            ViewBag.BirthDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(DateTime.Now);
            ViewBag.Title = "ثبت نام-تایید شماره موبایل";
            if (TempData["ZarinPlaUserData"] != null)
            {
                throw new NotImplementedException("این قسمت هنوز پیاده سازی نشده است");
                //var data = JsonConvert.DeserializeObject<ZarinpalUserData>(TempData["ZarinPlaUserData"].ToString());
                //if (data != null && data.Data != null)
                //{
                //    if (data.Data.Me != null)
                //    {
                //        model.LoginFromZarinpal = true;
                //        model.FName = data.Data.Me.first_name;
                //        model.LName = data.Data.Me.last_name;
                //        model.NationalCode = data.Data.Me.ssn;
                //        model.UserName = data.Data.Me.ssn;
                //        model.Email = data.Data.Me.email;
                //        model.Mobile = data.Data.Me.cell_number;
                //        if (!string.IsNullOrEmpty(data.Data.Me.gender))
                //            model.Gender = data.Data.Me.gender.ToLower() == "MALE" ? GenderEnum.Male : GenderEnum.Female;
                //        model.BirthDate = Convert.ToDateTime(data.Data.Me.birthday);
                //        ViewBag.PersianBirthDate = model.BirthDate.GregorianToShamsi();
                //        ViewBag.BirthDate = model.BirthDate.ToString("yyyy/MM/dd"); ;
                //    }
                //    if (data.Data.Addresses != null && data.Data.Addresses.Any())
                //    {
                //        model.Address = data.Data.Addresses[0].address;
                //        model.PostalCode = data.Data.Addresses[0].postal_code;
                //        model.PhoneNumber = data.Data.Addresses[0].landline;
                //    }
                //}
            }
            if (IsAuthenticated)
                return Redirect(model.ReturnUrl);
            else if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken)))
                await _signInManager.SignOutAsync();

            return View("QuickRegister_Step1", model);
        }

        [AllowAnonymous]
        public async Task<ActionResult<UserQuickRegisterModel>> RegisterStep2(CancellationToken cancellationToken, string returnUrl = null)
        {
            if (CookieManager.Get(HttpContext, CookieManager.CookieKeys.QuickRegisterData) == null)
                return RedirectToRoute("Register_Step1");

            var decryptyData = await SecurityHelper.DecryptAsync(CookieManager.Get(HttpContext, CookieManager.CookieKeys.QuickRegisterData), "Z@r!nL3nD");
            var model = new UserQuickRegisterModel()
            {
                Mobile = decryptyData.Split('|')[0]
            };
            model.ReturnUrl = returnUrl ?? Url.Content("~/");
            ViewBag.BirthDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(DateTime.Now);
            ViewBag.Title = "ثبت نام-تکمیل اطلاعات";
            if (TempData["ZarinPlaUserData"] != null)
            {
                throw new NotImplementedException("این قسمت هنوز پیاده سازی نشده است");
                //var data = JsonConvert.DeserializeObject<ZarinpalUserData>(TempData["ZarinPlaUserData"].ToString());
                //if (data != null && data.Data != null)
                //{
                //    if (data.Data.Me != null)
                //    {
                //        model.LoginFromZarinpal = true;
                //        model.FName = data.Data.Me.first_name;
                //        model.LName = data.Data.Me.last_name;
                //        model.NationalCode = data.Data.Me.ssn;
                //        model.UserName = data.Data.Me.ssn;
                //        model.Email = data.Data.Me.email;
                //        model.Mobile = data.Data.Me.cell_number;
                //        if (!string.IsNullOrEmpty(data.Data.Me.gender))
                //            model.Gender = data.Data.Me.gender.ToLower() == "MALE" ? GenderEnum.Male : GenderEnum.Female;
                //        model.BirthDate = Convert.ToDateTime(data.Data.Me.birthday);
                //        ViewBag.PersianBirthDate = model.BirthDate.GregorianToShamsi();
                //        ViewBag.BirthDate = model.BirthDate.ToString("yyyy/MM/dd"); ;
                //    }
                //    if (data.Data.Addresses != null && data.Data.Addresses.Any())
                //    {
                //        model.Address = data.Data.Addresses[0].address;
                //        model.PostalCode = data.Data.Addresses[0].postal_code;
                //        model.PhoneNumber = data.Data.Addresses[0].landline;
                //    }
                //}
            }
            if (IsAuthenticated)
                return Redirect(model.ReturnUrl);
            else if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken)))
                await _signInManager.SignOutAsync();

            return View("QuickRegister_Step2", model);
        }

        [AllowAnonymous]
        public async Task<ActionResult<UserAddModel>> Register(CancellationToken cancellationToken, string returnUrl = null)
        {
            var model = await userService.PrepareModelForAdd(cancellationToken);
            model.ReturnUrl = returnUrl ?? Url.Content("~/");
            ViewBag.BirthDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(DateTime.Now);
            if (TempData["ZarinPlaUserData"] != null)
            {
                var data = JsonConvert.DeserializeObject<ZarinpalUserData>(TempData["ZarinPlaUserData"].ToString());
                if (data != null && data.Data != null)
                {
                    if (data.Data.Me != null)
                    {
                        model.LoginFromZarinpal = true;
                        model.FName = data.Data.Me.first_name;
                        model.LName = data.Data.Me.last_name;
                        model.NationalCode = data.Data.Me.ssn;
                        model.UserName = data.Data.Me.ssn;
                        model.Email = data.Data.Me.email;
                        model.Mobile = data.Data.Me.cell_number;
                        if (!string.IsNullOrEmpty(data.Data.Me.gender))
                            model.Gender = data.Data.Me.gender.ToLower() == "MALE" ? GenderEnum.Male : GenderEnum.Female;
                        model.BirthDate = Convert.ToDateTime(data.Data.Me.birthday);
                        ViewBag.PersianBirthDate = model.BirthDate.GregorianToShamsi();
                        ViewBag.BirthDate = model.BirthDate.ToString("yyyy/MM/dd"); ;
                    }
                    if (data.Data.Addresses != null && data.Data.Addresses.Any())
                    {
                        model.Address = data.Data.Addresses[0].address;
                        model.PostalCode = data.Data.Addresses[0].postal_code;
                        model.PhoneNumber = data.Data.Addresses[0].landline;
                    }
                }
            }
            if (IsAuthenticated)
                return Redirect(model.ReturnUrl);
            else if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken)))
                await _signInManager.SignOutAsync();

            return View(model);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<UserEditModel>> Edit(CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var waitingRequestFacilityId = await requestFacilityService
               .GetRequestFacilityIdWaitingSpecifiedStepAndRole(userId, new List<RoleEnum> { RoleEnum.Buyer },
               WorkFlowFormEnum.RegisterIdentityInfo, cancellationToken);
            var userModel = await userService.PrepareModelForEdit(userId, cancellationToken);
            ViewBag.BirthDate = userModel.BirthDate.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(userModel.BirthDate);
            userModel.RequestFacilityId = waitingRequestFacilityId;
            if (userModel == default(UserEditModel))
                throw new NotFoundException();

            userModel.UserId = User.Identity.GetUserId();

            return View(model: userModel);
        }

        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert)]
        [HttpGet("[controller]/[action]/{userId:guid}")]
        public async Task<ActionResult<UserEditModel>> Edit(Guid userId, CancellationToken cancellationToken)
        {
            var userModel = await userService.PrepareModelForEdit(userId, cancellationToken);
            ViewBag.BirthDate = userModel.BirthDate.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(userModel.BirthDate);
            if (userModel == default(UserEditModel))
                throw new NotFoundException();

            return View(model: userModel);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<UserEditModel>> EditGuarantorInfo(CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var waitingRequestFacilityGuarantorId = await requestFacilityGuarantorService
               .GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(userId, new List<RoleEnum> { RoleEnum.Buyer },
               WorkFlowFormEnum.EditGuarantorInfo, cancellationToken);
            var userModel = await userService.PrepareModelForEditGuarantor(userId, cancellationToken);
            ViewBag.BirthDate = userModel.BirthDate.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(userModel.BirthDate);
            userModel.RequestFacilityGuarantorId = waitingRequestFacilityGuarantorId;
            if (userModel == default(UserEditModel))
                throw new NotFoundException();

            return View(model: userModel);
        }

        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert)]
        [HttpGet("[controller]/[action]/{userId:guid}")]
        public async Task<ActionResult<UserEditModel>> EditGuarantorInfo(Guid userId, CancellationToken cancellationToken)
        {
            var userModel = await userService.PrepareModelForEditGuarantor(userId, cancellationToken);
            ViewBag.BirthDate = userModel.BirthDate.ToString("yyyy/MM/dd");
            ViewBag.PersianBirthDate = DateTimeHelper.GregorianToShamsi(userModel.BirthDate);
            if (userModel == default(UserEditModel))
                throw new NotFoundException();

            return View(model: userModel);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<UserIdentityDocumentModel>> UploadIdentityDocuments(CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            var identityDocuments = await userService.GetUserIdentityDocument(userId, cancellationToken);
            var waitingRequestFacilityId = await requestFacilityService
              .GetRequestFacilityIdWaitingSpecifiedStepAndRole(userId, new List<RoleEnum> { RoleEnum.Buyer },
              WorkFlowFormEnum.UploadIdentityDocuments, cancellationToken);
            if (identityDocuments != null)
                identityDocuments.IsEditMode = true;
            else
                identityDocuments = new UserIdentityDocumentModel();

            identityDocuments.RequestFacilityId = waitingRequestFacilityId;
            return View(model: identityDocuments);
        }


        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<UserIdentityDocumentModel>> UploadIdentityDocumentsGuarantor(CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            var identityDocuments = await userService.GetUserIdentityDocument(userId, cancellationToken);
            var waitingRequestFacilityGurantorId = await requestFacilityGuarantorService
              .GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(userId, new List<RoleEnum> { RoleEnum.Buyer },
              WorkFlowFormEnum.UploadIdentityDocumentsGuarantor, cancellationToken);
            if (identityDocuments != null)
                identityDocuments.IsEditMode = true;
            else
                identityDocuments = new UserIdentityDocumentModel();

            identityDocuments.RequestFacilityGuarantorId = waitingRequestFacilityGurantorId;
            return View(model: identityDocuments);
        }
    }
}