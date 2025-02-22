using Core.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Common;
using System.Threading.Tasks;
using Services.Model;
using Common.Utilities;
using System.Linq;

namespace Services
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly SiteSettings siteSettings;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public JwtService(IOptionsSnapshot<SiteSettings> siteSetting, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            siteSettings = siteSetting.Value;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public async Task<TokenModel> GenerateAsync(User user)
        {
            var securityKey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.SecretKey);//longer than 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.Encryptkey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = await getClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);
            DateTime expireDate;
            if (userRoles.Any() && userRoles.Any(p => p == RoleEnum.AdminBankLeasing.ToString() ||
                                                      p == RoleEnum.SupervisorLeasing.ToString() ||
                                                      p == RoleEnum.BankLeasing.ToString() ||
                                                      p == RoleEnum.Seller.ToString()))
                expireDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);    //TODO : Cookie Expire        
            else
                expireDate = DateTime.Now.AddDays(14);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = siteSettings.JwtSettings.Issuer,
                Audience = siteSettings.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                //NotBefore = DateTime.Now.AddMinutes(5),
                NotBefore = DateTime.Now.AddMinutes(0),
                Expires = expireDate,
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);

            var jwt = tokenHandler.WriteToken(securityToken);

            return new TokenModel()
            {
                Token = jwt,
                Expire = expireDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ExpireBaseTimeStamp = DateTimeHelper.ConvertDatetTimeToUnixTimeStamp(expireDate),
            };
        }

        private async Task<IEnumerable<Claim>> getClaimsAsync(User user)
        {
            var claims = (await signInManager.ClaimsFactory.CreateAsync(user)).Claims;
            //add custom claims
            var claimsList = new List<Claim>(claims);
            //claimsList.Add(new Claim(ClaimTypes.MobilePhone, "09123456987"));

            return claimsList;

            #region custom authentications
            ////JwtRegisteredClaimNames.Sub
            //var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;

            //var list = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, user.UserName),
            //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //    new Claim(ClaimTypes.MobilePhone, "09123456987"),
            //    new Claim("FullName",user.FullName),
            //    new Claim(securityStampClaimType, user.SecurityStamp.ToString())
            //};

            //var roles = new Role[]
            //{
            //    new Role { Name = "Admin" },
            //    new Role { Name = "SuperAdmin" }
            //};
            //foreach (var role in roles)
            //    list.Add(new Claim(ClaimTypes.Role, role.Name));

            //return list;
            #endregion
        }
    }
}
