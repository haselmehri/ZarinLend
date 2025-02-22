using Asp.Versioning;
using AutoMapper;
using Common;
using Common.Exceptions;
using Common.Utilities;
using Common.Utilities.Email;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Services;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Configuration;
using WebFramework.Filters;
using ZarinLend.Common.LocalizationResource;
using ZarinLend.Services.Model.Ayandeh.BankAccount;
using Microsoft.CodeAnalysis;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class UserController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<User> _signInManager;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly IFinnotechService finnotechService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UserController> _logger;

        public UserController(IMapper mapper,
                              IUserService userService,
                              UserManager<User> userManager,
                              IRequestFacilityService requestFacilityService,
                              IEmailSender emailSender, SignInManager<User> signInManager,
                              IRequestFacilityGuarantorService requestFacilityGuarantorService,
                              IFinnotechService finnotechService,
                              IGlobalSettingService globalSettingService,
                              IWebHostEnvironment webHostEnvironment,
                              ILogger<UserController> logger)
        {
            _mapper = mapper;
            _userService = userService;
            _userManager = userManager;
            this.requestFacilityService = requestFacilityService;
            _emailSender = emailSender;
            _signInManager = signInManager;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.finnotechService = finnotechService;
            this.globalSettingService = globalSettingService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, RoleEnum.Buyer, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task SaveVapid(UserVAPIDModel model, CancellationToken cancellationToken)
        {
            model.UserId = new Guid(User.Identity.GetUserId());
            await _userService.SaveVAPID(model, cancellationToken);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult<TokenModel>> Login(LoginWithUserPassModel model, CancellationToken cancellationToken)
        {
            var token = await _userService.Token(model, cancellationToken);
            if (token != null)
            {
                #region Login

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    //CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, token.Token, SameSiteMode.Strict, true, true, false, DateTime.Parse(token.Expire));
                    CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, token.Token, SameSiteMode.Lax, true, true, true, DateTime.Parse(token.Expire));
                    return new ApiResult<TokenModel>(true, ApiResultStatusCode.Success, token);
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                //}
                if (result.IsLockedOut)
                {
                    throw new LogicException(ResourceFile.AccountIsDisable);
                }

                #endregion
            }
            return new ApiResult<TokenModel>(false, ApiResultStatusCode.Success, null);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult> SendOtpForLogin(OtpModel model, CancellationToken cancellationToken)
        {
            await _userService.SendOtpForLogin(model, cancellationToken);

            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult> SendOtpForRegister(OtpModel model, CancellationToken cancellationToken)
        {
            var otp = await _userService.SendOtpForRegister(model, cancellationToken);
            var encryptData = await SecurityHelper.EncryptAsStringAsync($"{model.Mobile}|{otp}|{DateTime.Now.AddMinutes(5).Ticks}", "Z@r!nL3nD");
            CookieManager.Set(HttpContext,
                              CookieManager.CookieKeys.QuickRegisterData,
                              encryptData,
                              httpOnly: true,
                              sameSite: SameSiteMode.Strict,
                              isEssential: true,
                              secure: true,
                              expireTime: DateTime.Now.AddMinutes(5));
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult> ValidateMobileAndOtp(UserRegisterFirstStepModel model, CancellationToken cancellationToken)
        {
            if (CookieManager.Get(HttpContext, CookieManager.CookieKeys.QuickRegisterData) == null)
                return new ApiResult<dynamic>(false, ApiResultStatusCode.BadRequest, new { MobileAndOtpNotFound = false }, message: "موبایل یافت نشد!");

            var decryptyData = await SecurityHelper.DecryptAsync(CookieManager.Get(HttpContext, CookieManager.CookieKeys.QuickRegisterData), "Z@r!nL3nD");
            if (decryptyData.Split('|')[0] != model.Mobile)
                return new ApiResult<dynamic>(false, ApiResultStatusCode.BadRequest, new { MobileAndOtpNotFound = false }, message: "موبایل یافت نشد!");
            if (decryptyData.Split('|')[1] != model.Otp)
                return new ApiResult<dynamic>(false, ApiResultStatusCode.BadRequest, new { OtpIsCorrect = false }, message: "کد تایید وارد شده صحیح نمی باشد!");

            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<bool> QuickRegister([FromForm] UserQuickRegisterModel userModel, CancellationToken cancellationToken)
        {
            var newUser = await _userService.QuickRegister(userModel, cancellationToken);
            if (newUser != null)
            {
                var token = await _userService.Token(new LoginWithUserPassModel { Username = userModel.UserName, Password = userModel.Password }, cancellationToken);
                if (token != null)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(userModel.UserName, userModel.Password, true, true);
                    if (signInResult.Succeeded)
                    {
                        CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, token.Token, SameSiteMode.Strict, true, true, false, CookieManager.ExpireTimeMode.Day, 14);
                        return true;
                    }
                    //else //if (signInResult.IsLockedOut)
                    //    return false;
                }

            }
            return false;
        }


        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult<TokenModel>> LoginWithOtp(LoginWithOtpModel model, CancellationToken cancellationToken)
        {
            var result = await _userService.GenerateTokenWithMobile(model, cancellationToken);
            if (result != null)
            {
                #region Login
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                await _signInManager.SignInAsync(result.User, true);

                CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, result.Token.Token, SameSiteMode.Strict, true, true, false, DateTime.Parse(result.Token.Expire));
                return new ApiResult<TokenModel>(true, ApiResultStatusCode.Success, result.Token);

                #endregion
            }
            return new ApiResult<TokenModel>(false, ApiResultStatusCode.Success, null);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult> SendResetPasswordOtp(ResetPasswordModel model, CancellationToken cancellationToken)
        {
            await _userService.GenerateResetPasswordOtp(model, cancellationToken);

            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult> ResetPassword(ForgotPasswordModel model, CancellationToken cancellationToken)
        {
            var result = await _userService.ResetPassword(model, cancellationToken);

            if (result.Succeeded)
                return new ApiResult(true, ApiResultStatusCode.Success);

            throw new AppException(string.Join("<br/>", result.Errors.Select(p => p.Description)));
        }

        [HttpPost("[action]")]
        [CustomAuthorize]
        public async Task<bool> ChangePassword(SetNewPasswordModel model, CancellationToken cancellationToken)
        {
            //todo notusing
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                throw new AppException("رمزعبور فعلی اشتباه است!");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
            return result.Succeeded;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<UserListModel>> UserList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var users = await _userService.SelectUsers(filter, cancellationToken);

            return users;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string> GenerateUserExcelList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var list = (await _userService.SelectUsers(filter, cancellationToken)).Data;
            var excelFile = await GenerateExcel(list);

            return excelFile;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> AddUser([FromForm] UserAddEditModelByAdmin userModel, CancellationToken cancellationToken)
        {
            return await _userService.AddUser(userModel, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> EditUser([FromForm] UserAddEditModelByAdmin userModel, CancellationToken cancellationToken)
        {
            return await _userService.EditUser(userModel, cancellationToken);
        }

        [HttpGet("{id:guid}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<UserEditModel>> Get(Guid id, CancellationToken cancellationToken)
        {
            var userModel = await _userService.Get(id, cancellationToken);

            if (userModel == null)
                return NotFound();

            return userModel;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<bool> ExternalRegisterBuyer([FromForm] UserAddModel userModel, CancellationToken cancellationToken, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(userModel.CardNumber))
                userModel.CardNumber = userModel.CardNumber.Replace("-", String.Empty);
            var customerNumber = await finnotechService.CifInquiry(userModel.NationalCode, null, cancellationToken);
            if (customerNumber != userModel.CustomerNumber)
                throw new AppException($"شماره مشتری وارد شده با شماره مشتری دریافتی از بانک مطابقت ندارد! شمار مشتری دریافتی از بانک : {customerNumber}");

            var newUser = await _userService.RegisterBuyer(userModel, cancellationToken);
            if (newUser != null)
                return true;
            else
                return false;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<bool> RegisterBuyer([FromForm] UserAddModel userModel, CancellationToken cancellationToken, string returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home");
            if (!string.IsNullOrEmpty(userModel.CardNumber))
                userModel.CardNumber = userModel.CardNumber.Replace("-", String.Empty);
            userModel.CustomerNumber = await finnotechService.CifInquiry(userModel.NationalCode, null, cancellationToken);
            var newUser = await _userService.RegisterBuyer(userModel, cancellationToken);
            if (newUser != null)
            {
                //await SendEmail(new ResendEmailModel
                //{
                //    Email = newUser.Email
                //}, cancellationToken);

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    //todo not complete this code

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    null,
                    //    new { area = "Identity", userId = newUser.Id, code, returnUrl },
                    //    Request.Scheme);

                    //await _emailSender.SendEmailAsync(new MailRequest
                    //{
                    //    ToEmail = userDto.Email,
                    //    Subject = "Confirm your email",
                    //    Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                    //});
                    //return RedirectToPage("RegisterConfirmation", new { email = userDto.Email, returnUrl = returnUrl });

                    //todo not complete this code
                }
                else
                {
                    //var token = await  jwtService.GenerateAsync(newUser);
                    var token = await _userService.Token(new LoginWithUserPassModel { Username = userModel.UserName, Password = userModel.Password }, cancellationToken);
                    if (token != null)
                    {
                        var signInResult = (await _signInManager.PasswordSignInAsync(userModel.UserName, userModel.Password, true, true));
                        if (signInResult.Succeeded)
                        {
                            CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, token.Token, SameSiteMode.Strict, true, true, false, CookieManager.ExpireTimeMode.Day, 14);
                            return true;
                        }
                        //else //if (signInResult.IsLockedOut)
                        //    return false;
                    }
                }
            }
            return false;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> EditBuyer([FromForm] UserEditModel userModel, CancellationToken cancellationToken)
        {
            userModel.Id = new Guid(User.Identity.GetUserId());
            //if (!string.IsNullOrEmpty(userModel.CardNumber))
            //    userModel.CardNumber = userModel.CardNumber.Replace("-", String.Empty);
            var waitingRequestFacilityId = await requestFacilityService
                .GetRequestFacilityIdWaitingSpecifiedStepAndRole(userModel.Id, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.RegisterIdentityInfo, cancellationToken);
            if (waitingRequestFacilityId.HasValue)
                userModel.RequestFacilityId = waitingRequestFacilityId.Value;

            var result = await _userService.Update(userModel, cancellationToken);
            if (!string.IsNullOrEmpty(result.Token))
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync((await _userManager.FindByIdAsync(User.Identity!.GetUserId()))!, true);
                CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, result.Token, SameSiteMode.Strict, true, true, false, CookieManager.ExpireTimeMode.Day, 14);
                return true;
            }
            return false;
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> GoToNextStepFromProfileToUploadDocuments(int requestFacilityId, CancellationToken cancellationToken)
        {
            var waitingRequestFacilityId = await requestFacilityService
                .GetRequestFacilityIdWaitingSpecifiedStepAndRole(new Guid(User.Identity!.GetUserId()), new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.RegisterIdentityInfo, cancellationToken);
            if (waitingRequestFacilityId.HasValue)
                return await _userService.GoToNextStepFromProfileToUploadDocuments(new Guid(User.Identity!.GetUserId()), waitingRequestFacilityId.Value, cancellationToken);
            else
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
        }


        [HttpPost("[action]/{requestFacilityGuarantorId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> GoToNextStepFromProfileToUploadDocumentsInGuarantor(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            var waitingRequestFacilityGuarantorId = await requestFacilityGuarantorService
                .GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(new Guid(User.Identity!.GetUserId()), new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.EditGuarantorInfo, cancellationToken);
            if (waitingRequestFacilityGuarantorId.HasValue)
                return await _userService.GoToNextStepFromProfileToUploadDocumentsInGuarantor(new Guid(User.Identity!.GetUserId()), waitingRequestFacilityGuarantorId.Value, cancellationToken);
            else
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> GoToNextStepFromUploadDocumentsToVerifyZarinLend(int requestFacilityId, CancellationToken cancellationToken)
        {
            var waitingRequestFacilityId = await requestFacilityService
                .GetRequestFacilityIdWaitingSpecifiedStepAndRole(new Guid(User.Identity.GetUserId()), new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.UploadIdentityDocuments, cancellationToken);
            if (waitingRequestFacilityId.HasValue)
            {
                var requestFacilityRequiredGuarantor = await requestFacilityService.CheckGuarantorIsRequired(waitingRequestFacilityId.Value, cancellationToken);
                var hasGaurantorWithApprovedWorkFlow = requestFacilityRequiredGuarantor ? await requestFacilityGuarantorService.HasGaurantorWithApprovedWorkFlow(waitingRequestFacilityId.Value, cancellationToken) : false;

                if (requestFacilityRequiredGuarantor && !hasGaurantorWithApprovedWorkFlow)
                {
                    await requestFacilityService.UpdateAwaitingIntroductionGuarantor(waitingRequestFacilityId.Value, awaitingIntroductionGuarantor: true, saveNow: true, cancellationToken);
                    throw new AppException($"درخواست تسهیلات شما نیاز به معرفی ضامن دارد تا زمانی که فرآیند معرفی،ثبت درخواست ضامن و تکمیل پرونده انجام نشود پرونده شما به کارشناسان ما ارسال نمیشود{Environment.NewLine}در صورتی تکمیل فرآیند ضامن تسهیلات شما بصورت اتوماتیک به کارشناسان ما ارسال خواهد شد");
                }

                return await _userService.GoToNextStepFromUploadDocumentsToVerifyZarinLend(new Guid(User.Identity.GetUserId()), waitingRequestFacilityId.Value, cancellationToken);
            }
            else
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
        }

        [HttpPost("[action]/{requestFacilityGuarantorId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> GoToNextStepFromUploadDocumentsToVerifyPaymentInGuarantor(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            var waitingRequestFacilityGuarantorId = await requestFacilityGuarantorService
                .GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(new Guid(User.Identity!.GetUserId()), new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.UploadIdentityDocumentsGuarantor, cancellationToken);
            if (waitingRequestFacilityGuarantorId.HasValue)
                return await _userService.GoToNextStepFromUploadDocumentsToVerifyZarinLendInGuarantor(new Guid(User.Identity.GetUserId()), waitingRequestFacilityGuarantorId.Value, cancellationToken);
            else
                throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> UpdateUserInfo([FromForm] UserInfoEditModel userModel, CancellationToken cancellationToken)
        {
            userModel.Id = new Guid(User.Identity!.GetUserId());
            var waitingRequestFacilityId = await requestFacilityService
                .GetRequestFacilityIdWaitingSpecifiedStepAndRole(userModel.Id, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.RegisterIdentityInfo, cancellationToken);
            if (waitingRequestFacilityId.HasValue)
                userModel.RequestFacilityId = waitingRequestFacilityId.Value;

            var result = await _userService.UpdateUserInfo(userModel, cancellationToken);
            if (!string.IsNullOrEmpty(result.Token))
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync((await _userManager.FindByIdAsync(User.Identity!.GetUserId()))!, true);
                CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, result.Token, SameSiteMode.Strict, true, true, false, CookieManager.ExpireTimeMode.Day, 14);
            }
            return true;
        }

        [HttpPost("[action]/{userId:guid}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> UpdateUserInfo(Guid userId, [FromForm] UserInfoEditModel userModel, CancellationToken cancellationToken)
        {
            userModel.Id = userId;
            var result = await _userService.UpdateUserInfo(userModel, cancellationToken);
            return true;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> UpdateUserLocation([FromForm] UserLocationEditModel userModel, CancellationToken cancellationToken)
        {
            userModel.Id = new Guid(User.Identity!.GetUserId());
            var result = await _userService.UpdateUserLocation(userModel, cancellationToken);
            if (!string.IsNullOrEmpty(result.Token))
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync((await _userManager.FindByIdAsync(User.Identity!.GetUserId()))!, true);
                CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, result.Token, SameSiteMode.Strict, true, true, false, CookieManager.ExpireTimeMode.Day, 14);
            }
            return true;
        }

        [HttpPost("[action]/{userId:guid}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> UpdateUserLocation(Guid userId, [FromForm] UserLocationEditModel userModel, CancellationToken cancellationToken)
        {
            userModel.Id = userId;
            var result = await _userService.UpdateUserLocation(userModel, cancellationToken);
            return true;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<string>> GetCustomerNumber(CancellationToken cancellationToken)
        {
            var result = await _userService.GetCustomerNumber(new Guid(User.Identity!.GetUserId()!), cancellationToken);
            return new ApiResult<string>(true, ApiResultStatusCode.Success, data: result);
        }

        [HttpPost("[action]/{userId:guid}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<string>> GetCustomerNumber(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _userService.GetCustomerNumber(userId, cancellationToken);
            return new ApiResult<string>(true, ApiResultStatusCode.Success, data: result);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<BankAccountModel?>> AddCardNumber(BankAccountAddModel model, CancellationToken cancellationToken)
        {
            var result = await _userService.AddCardNumber(new Guid(User.Identity!.GetUserId()!), model, cancellationToken);
            return new ApiResult<BankAccountModel?>(true, ApiResultStatusCode.Success, message: "اطلاعات حساب با موفقیت دریافت و ثبت شد", data: result);
        }

        [HttpPost("[action]/{userId:guid}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<BankAccountModel?>> AddCardNumber(Guid userId, BankAccountAddModel model, CancellationToken cancellationToken)
        {
            var result = await _userService.AddCardNumber(userId, model, cancellationToken);
            return new ApiResult<BankAccountModel?>(true, ApiResultStatusCode.Success, message: "اطلاعات حساب با موفقیت دریافت و ثبت شد", data: result);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<dynamic>> UploadIdentityDocuments([FromForm] UploadIdentityDocumentPostModel data,
            CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());

            //await _userService.UploadIdentityDocuments(userId, data.bankId, data.BirthCertificatePage1, data.BirthCertificateDescription, data.NationalCardFront,
            //    data.NationalCardBack, data.AccountStatement, data.JobDocument, data.AddressDocument, data.DeleteFileIds, cancellationToken);
            await _userService.UploadIdentityDocuments(userId, data.NationalCardFront, data.NationalCardBack, cancellationToken);
            var waitingRequestFacilityId = await requestFacilityService
               .GetRequestFacilityIdWaitingSpecifiedStepAndRole(userId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.UploadIdentityDocuments, cancellationToken);
            if (waitingRequestFacilityId.HasValue)
            {
                var requestFacilityRequiredGuarantor = await requestFacilityService.CheckGuarantorIsRequired(waitingRequestFacilityId.Value, cancellationToken);
                var hasGaurantorWithApprovedWorkFlow = requestFacilityRequiredGuarantor ? await requestFacilityGuarantorService.HasGaurantorWithApprovedWorkFlow(waitingRequestFacilityId.Value, cancellationToken) : false;

                if (!requestFacilityRequiredGuarantor || hasGaurantorWithApprovedWorkFlow)
                    await _userService.GoToNextStepFromUploadDocumentsToVerifyZarinLend(userId, waitingRequestFacilityId.Value, cancellationToken);
                else if (requestFacilityRequiredGuarantor && !hasGaurantorWithApprovedWorkFlow)
                    await requestFacilityService.UpdateAwaitingIntroductionGuarantor(waitingRequestFacilityId.Value, awaitingIntroductionGuarantor: true, saveNow: true, cancellationToken);

                return new ApiResult<dynamic>(true, ApiResultStatusCode.Success,
                    new
                    {
                        RequestFacilityRequiredGuarantor = requestFacilityRequiredGuarantor,
                        HasGaurantorWithApprovedWorkFlow = hasGaurantorWithApprovedWorkFlow
                    });
            }

            return new ApiResult<dynamic>(true, ApiResultStatusCode.Success,
                new
                {
                    RequestFacilityRequiredGuarantor = false,
                    HasGaurantorWithApprovedWorkFlow = false
                });
        }

        [HttpDelete("[action]/{nationalCode}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ApiResult> DeleteUserByNationalCode(string nationalCode, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteUserByNationalCode(nationalCode, cancellationToken);
            return new ApiResult(result, result ? ApiResultStatusCode.Success : ApiResultStatusCode.ServerError);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> UploadIdentityDocumentsGuarantor([FromForm] UploadIdentityDocumentPostModel data, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var waitingRequestFacilityGuarantorId = await requestFacilityGuarantorService
               .GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(userId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.UploadIdentityDocumentsGuarantor, cancellationToken);
            //await _userService.UploadIdentityDocumentsGuarantor(userId, waitingRequestFacilityGuarantorId, data.BirthCertificatePage1, data.BirthCertificateDescription, data.NationalCardFront,
            //    data.NationalCardBack, data.JobDocument, data.AddressDocument, data.DeleteFileIds, cancellationToken);
            await _userService.UploadIdentityDocumentsGuarantor(userId, waitingRequestFacilityGuarantorId, data.NationalCardFront, data.NationalCardBack, cancellationToken);
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<dynamic>> SearchUser(UserFilterModel model, CancellationToken cancellationToken)
        {
            var userDetail = await _userService.SearchUser(model, cancellationToken);
            if (userDetail != null)
            {
                if (userDetail.WalletBalanceBaseCardNumber.Any(p => p.Balance > 0))
                {
                    var activeGlobalSetting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
                    var feePercentage = activeGlobalSetting.LendTechFacilityFee + activeGlobalSetting.FinancialInstitutionFacilityFee;
                    var balance = userDetail.WalletBalanceBaseCardNumber.FirstOrDefault(p => p.Balance > 0).Balance;
                    var cardNumber = userDetail.WalletBalanceBaseCardNumber.FirstOrDefault(p => p.Balance > 0).CardNumber;
                    var fee = feePercentage * balance / 100.0;
                    return new ApiResult<dynamic>(true, ApiResultStatusCode.Success,
                        new
                        {
                            Balance = balance,
                            UserDetail = userDetail,
                            FeePercentage = feePercentage,
                            Fee = fee,
                            ShopShare = balance - fee,
                            cardNumber = cardNumber
                        });
                }
                else
                {
                    return new ApiResult<dynamic>(true, ApiResultStatusCode.Success,
                        new
                        {
                            Balance = 0,
                            UserDetail = userDetail,
                            FeePercentage = 0,
                            Fee = 0,
                            ShopShare = 0
                        });
                }
            }
            return null;
        }
        [HttpPut]
        //[CustomAuthorize("Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Update(UserDto userDto, CancellationToken cancellationToken)
        {
            await _userService.Update(userDto, cancellationToken);

            return Ok();
        }

        //[CustomAuthorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id:guid}")]
        public virtual async Task<ApiResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _userService.Delete(id, cancellationToken);

            return Ok();
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<UserSelectDto> UpdateFullname(dynamic dynamicDto, CancellationToken cancellationToken)
        {
            UserDto userDto = UtilConvertor.ToObject<UserDto>(dynamicDto);
            List<string> properties = UtilConvertor.GetPropertiesName(dynamicDto);
            userDto.UpdateProperties = properties;
            var userSelectDto = await _userService.UpdateCustomProperties(userDto, cancellationToken);

            return userSelectDto;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ApiResult<string>> SendEmail(ResendEmailModel model, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new BadRequestException("حساب کاربری با ایمیل وارد شده یافت نشد!");


            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Action("ConfirmEmail", "Account", new
            {
                userId = user.Id,
                token = code
            }, HttpContext.Request.Scheme);

            var encodedLink = HtmlEncoder.Default.Encode(callbackUrl);

            await _emailSender.SendEmailAsync(new MailRequest
            {
                ToEmail = model.Email,
                Subject = "تایید ایمیل هدف سنج",
                Body = await EmailCreator.Create(new EmailCreatorModel
                {
                    BodyText = "برای تایید ایمیل خودتون بر روی دکمه زیر کلیک کنید",
                    Link = encodedLink,
                    Name = $"{user.Email.Split('@')[0]} عزیز ، سلام"
                }, _webHostEnvironment.WebRootPath, cancellationToken)
            });

            return encodedLink;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ApiResult> UpdateBankAccount([FromBody] UpdateBankAccountModel model, CancellationToken cancellationToken) 
        {
            await _userService.UpdateBankAccount(model, cancellationToken);

            return Ok();
        }

        private async Task<string?> GenerateExcel(IEnumerable<UserListModel> userList, CancellationToken cancellationToken = default)
        {
            if (userList == null || userList.Count() == 0)
                return null;

            var result = userList.Select(s => new 
            { 
                s.FName
                , s.LName
                , s.Mobile
                , s.UserName
                , s.OrganizationName
                , s.RoleName
                , s.IsActive
                , s.CreatedDatePersian
                , s.IranCardScore
            });

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("لیست کاربران");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromCollection(result, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "FName").Start.Column;
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


                worksheet.SetValue(1, 1, "نام");
                worksheet.SetValue(1, 2, "نام خانوادگی");
                worksheet.SetValue(1, 3, "نام کاربری");
                worksheet.SetValue(1, 4, "موبایل");
                worksheet.SetValue(1, 5, "نام سازمان(نوع)");
                worksheet.SetValue(1, 6, "نقش(ها)");
                worksheet.SetValue(1, 7, "وضعیت");
                worksheet.SetValue(1, 8, "تاریخ ایجاد");
                worksheet.SetValue(1, 9, "امتیاز");

                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
            }
        }
    }
}
