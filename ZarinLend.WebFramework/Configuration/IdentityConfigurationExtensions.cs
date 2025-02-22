using Common;
using Core.Data;
using Core.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WebFramework.Configuration
{
    public static class IdentityConfigurationExtensions
    {
        public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
        {
            services.AddIdentity<User, Role>(identityOptions =>
            {
                //Password Settings
                identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
                identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
                identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumic; //#@!
                identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
                identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

                //UserName Settings
                identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;

                //Singin Settings
                identityOptions.SignIn.RequireConfirmedEmail = settings.RequireConfirmedEmail;
                identityOptions.SignIn.RequireConfirmedPhoneNumber = settings.RequireConfirmedPhoneNumber;

                //Lockout Settings
                //identityOptions.Lockout.MaxFailedAccessAttempts = 5;
                //identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //identityOptions.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddClaimsPrincipalFactory<AdditionalUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();

            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-3.1#globally-require-all-users-to-be-authenticated
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/401";
                options.LogoutPath = "/login";
                options.LoginPath = "/login";
                options.Cookie.Name = CookieManager.CookieKeys.AuthZL.ToString();
                options.ExpireTimeSpan = TimeSpan.FromDays(14);//DateTime.Now.Date.AddDays(1).AddTicks(-1).TimeOfDay;
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
                //TODO : Cookie Expire
            });
        }

        public static void AddGoogleAuthentication(this IServiceCollection services, GoogleAuth googleAuth)
        {
            services.AddAuthentication(
            //    options =>
            //{
            //    options.DefaultScheme = "Application";
            //    options.DefaultSignInScheme = "External";
            //}
            )
                //.AddCookie("Application")
                //.AddCookie("External")
                .AddGoogle("google", opt =>
                {
                    opt.ClientId = googleAuth.ClientId;
                    opt.ClientSecret = googleAuth.ClientSecret;
                    opt.SignInScheme = IdentityConstants.ExternalScheme;
                });
        }

        public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
        {
            private const string TicketIssuedTicks = nameof(TicketIssuedTicks);

            public override async Task SigningIn(CookieSigningInContext context)
            {
                context.Properties.SetString(TicketIssuedTicks, DateTimeOffset.UtcNow.Ticks.ToString());

                await base.SigningIn(context);
            }

            public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
            {
                //var ticketIssuedTicksValue = context.Properties.GetString(TicketIssuedTicks);

                //if (ticketIssuedTicksValue is null ||
                //    !long.TryParse(ticketIssuedTicksValue, out var ticketIssuedTicks))
                //{
                //    await RejectPrincipalAsync(context);
                //    return;
                //}

                //var ticketIssuedUtc = new DateTimeOffset(ticketIssuedTicks, TimeSpan.FromMinutes(210));

                //if (DateTimeOffset.Now - ticketIssuedUtc > TimeSpan.FromDays(3))
                //{
                //    //await RejectPrincipalAsync(context);
                //    //return;
                //}

                if (string.IsNullOrEmpty(CookieManager.Get(context.HttpContext, CookieManager.CookieKeys.GlobalExpireTime)))
                {
                    //await RejectPrincipalAsync(context);
                    //return;
                    //context.HttpContext.Response.Cookies.Delete("ZarrinLendIdentityCookie");
                    CookieManager.Remove(context.HttpContext, CookieManager.CookieKeys.AuthZL);
                    await context.HttpContext.SignOutAsync();
                }

                await base.ValidatePrincipal(context);
            }

            private static async Task RejectPrincipalAsync(CookieValidatePrincipalContext context)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync();
            }
        }
    }
}
