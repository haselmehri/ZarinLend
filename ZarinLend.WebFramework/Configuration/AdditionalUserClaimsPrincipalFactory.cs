using Common;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Services;
using Services.Model;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebFramework.Configuration
{
    public class AdditionalUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<User>, IScopedDependency
    {
        private readonly IPersonService personService;

        public AdditionalUserClaimsPrincipalFactory(
            UserManager<User> userManager,
           IPersonService personService,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            this.personService = personService;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            var userRoles = await UserManager.GetRolesAsync(user);
            var personModel = new PersonModel();
            if (user.Person == null || (user.Person.OrganizationId.HasValue && user.Person.Organization == null))
                personModel = await personService.GetPerson(user.PersonId, cancellationToken: default);
            else
                personModel = new PersonModel()
                {
                    FName = user.Person.FName,
                    LName = user.Person.LName,
                    Mobile = user.Person.Mobile,
                    OrganizationId = user.Person.OrganizationId,
                    OrganizationName = user.Person.OrganizationId.HasValue ? user.Person.Organization.Name : null,
                };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.MobilePhone, personModel.Mobile),
                new Claim(ClaimTypes.Surname, $"{personModel.FName} {personModel.LName}")
            };

            if (userRoles.Any(p => p == RoleEnum.AdminBankLeasing.ToString() || 
                                   p == RoleEnum.SupervisorLeasing.ToString() || 
                                   p == RoleEnum.BankLeasing.ToString() || 
                                   p == RoleEnum.Seller.ToString()))
            {
                claims.Add(new Claim(ClaimTypes.UserData, personModel.OrganizationId.ToString()));
                claims.Add(new Claim("OrganizationName", personModel.OrganizationName));
            }

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            identity.AddClaims(claims);

            return principal;
        }
    }
}
