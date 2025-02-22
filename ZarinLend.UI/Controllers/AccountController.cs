using Common;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services;
using Services.Model;
using Services.Model.ZarinPal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Configuration;
using WebFramework.Filters;

namespace Web.UI.Controllers
{

    public class AccountController(SignInManager<User> signInManager,
                             IZarinpalService zarinpalService,
                             IUserService userService,
                             IRequestFacilityRepository requestFacilityRepository,
                             IBaseRepository<RequestFacilityInstallment> requestFacilityInstallmentRepository) : BaseMvcController
    {
        private readonly string CodeVerifier = "ySTV3qffgdSIGZcNdQoDXi0rMxKQonuj0JiwPbcX9g0";

        //[AllowAnonymous]
        //public async Task<ActionResult<LoginModel>> Import()
        //{
        //    using SqlConnection con = new SqlConnection(configuration.GetConnectionString("ZarrinLendDataBase"));
        //    using SqlDataAdapter da = new SqlDataAdapter("Select * from ___ImportUserfromZarinPal Where BirthDate != 'NULL'  and NationalCode IS NOT NULL and PhoneNumber IS Not NULL", con);
        //    DataTable dt = new DataTable();
        //    await con.OpenAsync();
        //    da.Fill(dt);

        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        UserQuickRegisterModel? model = null;
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            var mobile = System.Text.RegularExpressions.Regex.Replace(dt.Rows[i]["PhoneNumber"].ToString(), "^98", "0");
        //            var nationalCode = dt.Rows[i]["NationalCode"].ToString();
        //            var birthDate = Convert.ToDateTime(dt.Rows[i]["BirthDate"]);

        //            var sqlCommand = new SqlCommand("Select Count(*) as Count from People Where NationalCode=@NationalCode ", con);
        //            sqlCommand.Parameters.AddWithValue("@NationalCode", nationalCode);
        //            var count = (int)(await sqlCommand.ExecuteScalarAsync())!;
        //            if (count > 0)
        //            {
        //                using var sqlUpdateCommand = new SqlCommand("Update ___ImportUserfromZarinPal Set IsSuccess=0,Description=N'کد ملی تکراری است!' Where NationalCode=@NationalCode", con);
        //                sqlUpdateCommand.Parameters.AddWithValue("@NationalCode", nationalCode);
        //                await sqlUpdateCommand.ExecuteNonQueryAsync();
        //                continue;
        //            }

        //            sqlCommand.Dispose();
        //            sqlCommand = new SqlCommand("Select Count(*) as Count from People Where Mobile=@Mobile ", con);
        //            sqlCommand.Parameters.AddWithValue("@Mobile", mobile);
        //            count = (int)(await sqlCommand.ExecuteScalarAsync())!;
        //            if (count > 0)
        //            {
        //                using var sqlUpdateCommand = new SqlCommand("Update ___ImportUserfromZarinPal Set IsSuccess=0,Description=N'موبایل تکراری است!' Where NationalCode=@NationalCode", con);
        //                sqlUpdateCommand.Parameters.AddWithValue("@NationalCode", nationalCode);
        //                await sqlUpdateCommand.ExecuteNonQueryAsync();
        //                continue;
        //            }

        //            var civilRegistryData = await neginHubService.GetCivilRegistryDataV4(new Services.Model.NeginHub.GetCivilRegistryDataInputModel()
        //            {
        //                NationalCode = nationalCode!,
        //                ShamsiBirthDate = birthDate.GregorianToShamsi(_separator: string.Empty)
        //            },
        //            null,
        //            CancellationToken.None);

        //            if (civilRegistryData == null || !civilRegistryData.IsSuccess)
        //            {
        //                using var sqlUpdateCommand = new SqlCommand("Update ___ImportUserfromZarinPal Set IsSuccess=0,Description=N'خطا در استعلام ثبت احوال!'", con);
        //                sqlUpdateCommand.Parameters.AddWithValue("@NationalCode", nationalCode);
        //                await sqlUpdateCommand.ExecuteNonQueryAsync();
        //                continue;
        //            }

        //            var user = await userService.QuickRegister(new UserQuickRegisterModel()
        //            {
        //                FatherName = civilRegistryData.FatherName,
        //                FName = civilRegistryData.FirstName,
        //                LName = civilRegistryData.LastName,
        //                Gender = civilRegistryData.Gender.ToUpper() == "MALE" ? GenderEnum.Male : GenderEnum.Female,
        //                SSID = civilRegistryData.IdentityId,
        //                IdentificationSeri = civilRegistryData.IdentificationSeri,
        //                IdentificationSerial = civilRegistryData.IdentificationSerial,
        //                Mobile = mobile,
        //                NationalCode = nationalCode,
        //                PlaceOfBirth = civilRegistryData.PlaceOfBirth,
        //                ShamsiBirthDate = birthDate.GregorianToShamsi(),
        //                UserName = nationalCode,
        //                BirthDate = birthDate,
        //                Password = nationalCode,
        //                ConfirmPassword = nationalCode
        //            });
        //            if (user != null)
        //            {
        //                using var sqlUpdateCommand = new SqlCommand("Update ___ImportUserfromZarinPal Set IsSuccess=1,Description=N'Ok!' Where NationalCode=@NationalCode", con);
        //                sqlUpdateCommand.Parameters.AddWithValue("@NationalCode", nationalCode);
        //                await sqlUpdateCommand.ExecuteNonQueryAsync();
        //                continue;
        //            }

        //        }
        //    }

        //    return Content("Import Completed!");
        //}

        #region Login by Zarinpal

        [AllowAnonymous]
        [Route("/Account/LoginByZarinPal")]
        //[Route("/Account/LoginByZarinLend")]
        public async Task<ActionResult> LoginByZarinPal(string code = null, string error = null, string state = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(code))
            {
                string codeVerifier = zarinpalService.MakeCodeVerifier();
                CookieManager.Set(HttpContext, CookieManager.CookieKeys.CodeVerifier, codeVerifier, SameSiteMode.Unspecified, false, false, false, DateTime.Now.AddHours(1));

                string challengeCode = zarinpalService.MakeCodeChallenge(codeVerifier);
                var url = zarinpalService.GenerateAuthorizationUrl(challengeCode);
                return Redirect(url);
            }
            else
            {
                if (CookieManager.Get(HttpContext, CookieManager.CookieKeys.CodeVerifier) == null)
                    return Redirect($"{BaseUrl()}{Request.QueryString.Value}");
                var accessToken = await zarinpalService.GetAccessToken(code, CookieManager.Get(HttpContext, CookieManager.CookieKeys.CodeVerifier));
                var userData = await zarinpalService.GetUserData(accessToken);
                if (userData != null && userData.Data != null && userData.Data.Me != null)
                {
                    ZarinPalUserAddress? firstAddress = userData.Data.Addresses != null && userData.Data.Addresses.Any() ? userData.Data.Addresses.First() : null;
                    var model = new UserRegisterByZarinpalModel()
                    {
                        ZP_Id = long.Parse(userData.Data.Me.id),
                        FName = userData.Data.Me.first_name,
                        LName = userData.Data.Me.last_name,
                        NationalCode = userData.Data.Me.ssn,
                        Email = userData.Data.Me.email,
                        Mobile = userData.Data.Me.cell_number,
                        PostalCode = firstAddress != null ? firstAddress.postal_code : string.Empty,
                        VerifiedPostCode = firstAddress != null ? firstAddress.is_postal_code_verified : null,
                        PhoneNumber = firstAddress != null ? firstAddress.landline : string.Empty,
                        Address = firstAddress != null ? firstAddress.address : string.Empty,
                        BirthDate = Convert.ToDateTime(userData.Data.Me.birthday),
                        UserName = userData.Data.Me.ssn,
                        Password = userData.Data.Me.ssn,
                        ConfirmPassword = userData.Data.Me.ssn,
                        LogiFromZarinpal = true,
                    };
                    var result = await userService.RegisterByZarinpal(model, cancellationToken);

                    var token = await userService.Token(result.User.UserName, cancellationToken);
                    if (token != null)
                    {
                        await signInManager.SignInAsync(result.User, true);
                        //CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, token.Token, SameSiteMode.Strict, true, true, false, CookieManager.ExpireTimeMode.Day, 14);
                        CookieManager.Set(HttpContext, CookieManager.CookieKeys.JwtToken, token.Token, SameSiteMode.Lax, true, true, false, CookieManager.ExpireTimeMode.Day, 14);
                        if (!result.ExistUser)
                            TempData["RegisterFromZarinpal"] = true;

                        var a = CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken);
                        return Redirect("/RequestFacility/Add");
                        //return RedirectToAction("Add", "RequestFacility");
                    }
                }
                //TempData["ZarinPlaUserData"] = JsonConvert.SerializeObject(userData);
                //return RedirectToRoute("register");

                return RedirectToRoute("login");
            }
        }

        #endregion Login by Zarinpal

        [AllowAnonymous]
        public async Task<ActionResult<LoginModel>> Login(string? returnUrl = null)
        {
            //var list = await requestFacilityRepository.TableNoTracking.Where(p => p.CancelByUser == false &&
            //                                                                      p.RequestFacilityWorkFlowSteps.Any(x => (x.WorkFlowStepId == 100025 /*|| x.WorkFlowStepId == 1000251*/)) &&
            //                                                                      !p.RequestFacilityInstallments.Any())
            //    .Select(p => new
            //    {
            //        p.Id,
            //        p.Buyer.Person.NationalCode,
            //        FullName = $"{p.Buyer.Person.FName} {p.Buyer.Person.LName}",
            //        p.FacilityType.MonthCount,
            //        p.GlobalSetting.FacilityInterest,
            //        p.Amount,
            //        DepositDate = p.RequestFacilityWorkFlowSteps.Where(x => x.WorkFlowStepId == 100025).Select(x => x.CreatedDate).FirstOrDefault()
            //    })
            //    .ToListAsync();

            //foreach (var item in list)
            //{
            //    //if (item.NationalCode != "0019802595" && item.NationalCode != "0070439788" && item.NationalCode != "0067408656")
            //    //    continue;

            //    var amountInstallment = InstallmentCalculator.CalculateAmountInstallment(item.Amount, item.MonthCount, item.FacilityInterest);
            //    var installmetList = new List<RequestFacilityInstallment>();
            //    var currentDate = item.DepositDate;
            //    for (int i = 1; i <= item.MonthCount; i++)
            //    {
            //        installmetList.Add(new RequestFacilityInstallment()
            //        {
            //            RequestFacilityId = item.Id,
            //            DueDate = currentDate.AddMonthsBaseShamsi(i),
            //            Amount = (long)amountInstallment
            //        });
            //    }

            //    await requestFacilityInstallmentRepository.AddRangeAsync(installmetList, CancellationToken.None);
            //}

            //var user = await userManager.FindByNameAsync("0075661993");
            //user.PasswordHash = "";
            //var hashPass = userManager.PasswordHasher.HashPassword(user, "123456Aa1!");
            //user.PasswordHash = hashPass;
            //await userManager.UpdateAsync(user);


            //var text = "09126964896:5689:سعید";
            //var key = "z@r!nL3nD";
            //var a = SecurityHelper.Base64ToString(text);
            //var b = SecurityHelper.StringToBase64(a);
            //var c1 = await SecurityHelper.EncryptAsync(text, key);
            //var c2 = await SecurityHelper.EncryptAsStringAsync(text, key);
            //var b1 = await SecurityHelper.DecryptAsync(c1, key);
            //var b2 = await SecurityHelper.DecryptAsync(c2, key);


            //using SqlConnection con = new SqlConnection("Data Source=172.16.155.171;Initial Catalog=ZarrinLendDB;Persist Security Info=True; User ID=sa;Password=123qaz!@#QAZ;MultipleActiveResultSets=true;TrustServerCertificate=True");
            //using SqlDataAdapter da = new SqlDataAdapter("Select * from People", con);
            //DataTable dt = new DataTable();
            //con.Open();
            //da.Fill(dt);

            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        if (dt.Rows[i]["CardNumber"] != DBNull.Value && dt.Rows[i]["CardNumber"].ToString() != string.Empty)
            //        {
            //            SqlCommand cmd = new SqlCommand("Update People Set HashCardNumber=@HashCardNumber Where Id = @Id", con);
            //            cmd.Parameters.AddWithValue("@Id", dt.Rows[i]["Id"]);
            //            cmd.Parameters.AddWithValue("@HashCardNumber", SecurityHelper.GetSha256Hash(dt.Rows[i]["CardNumber"].ToString()));

            //            var result = await cmd.ExecuteNonQueryAsync();
            //        }
            //    }
            //}

            Response.Cookies.Append(
               CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("fa")),
               new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
           );

            var model = new LoginModel
            {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home")
            };

            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken)))
                await signInManager.SignOutAsync();
            else if (User.Identity.IsAuthenticated)
                return Redirect(model.ReturnUrl);

            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult<ForgotPasswordModel>> ForgotPassword(string? returnUrl = null)
        {
            var model = new ForgotPasswordModel
            {
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home")
            };

            if (User.Identity!.IsAuthenticated && string.IsNullOrEmpty(CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken)))
                await signInManager.SignOutAsync();

            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult<ResendEmailModel>> ResendEmailConfirmation()
        {
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken)))
                await signInManager.SignOutAsync();

            return View(new ResendEmailModel());
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest();

            var user = signInManager.UserManager.FindByIdAsync(userId).Result;

            var decodedToken = Encoding.Default.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await signInManager.UserManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded || signInManager.UserManager.IsEmailConfirmedAsync(user).Result)
                return RedirectToAction("Login", "Account");

            user.EmailConfirmed = true;

            await signInManager.UserManager.UpdateAsync(user);

            return View();
        }

        [AllowAnonymous]
        public IActionResult Confirmation()
        {
            return View();
        }



        [CustomAuthorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            CookieManager.RemoveAllCookie(HttpContext);

            return RedirectToAction("Login");
        }
    }
}