using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class RoleDataInitializer : IDataInitializer
    {
        private readonly RoleManager<Role> roleManager;

        public RoleDataInitializer(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
        }

        public int Order => 1;

        public void InitializeData()
        {
            var superAdminRole = new Role { Name = RoleEnum.SuperAdmin.ToString(), Title = "مدیر ارشد زرین لند", Description = "SuperAdmin Role-this role can create 'Admin Role'", Order = 1 };
            var adminRole = new Role { Name = RoleEnum.Admin.ToString(), Title = "مدیر زرین لند", Description = "Admin Role-کاربر زرین لند", Order = 2 };
            var zarinLendExpertRole = new Role { Name = RoleEnum.ZarinLendExpert.ToString(), Title = "کارشناس زرین لند", Description = "ZarinLend Expert Role-کارشناس زرین لند", Order = 3 };
            var adminBankLeasingRole = new Role { Name = RoleEnum.AdminBankLeasing.ToString(), Title = "مدیر بانک", Description = "کاربر ادمین بانک/لیزینگ-دسترسی کامل،تخصیص درخواست به کارشناسان و سوپروایزر و امضاء", Order = 4 };
            var supervisorleasingRole = new Role { Name = RoleEnum.SupervisorLeasing.ToString(), Title = "کارشناس ارشد بانک", Description = "کاربر سرپرست بانک/لیزینگ(دسترسی هر کاری غیر از امضاء و کارهای مربوط به مدیر)", Order = 5 };
            var leasingRole = new Role { Name = RoleEnum.BankLeasing.ToString(), Title = "کارشناس بانک", Description = "کاربر بانک/لیزینگ-اقدام روی درخواست ها", Order = 6 };
            var buyerRole = new Role { Name = RoleEnum.Buyer.ToString(), Title = "تسهیلات گیرنده", Description = "Buyer Role-خریدار", Order = 7 };
            var selleRole = new Role { Name = RoleEnum.Seller.ToString(), Title = "کارشناس فروشگاه", Description = "Seller Role-فروشنده", Order = 8 };

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.Admin.ToString()))
                roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.SuperAdmin.ToString()))
                roleManager.CreateAsync(superAdminRole).GetAwaiter().GetResult();

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.ZarinLendExpert.ToString()))
                roleManager.CreateAsync(zarinLendExpertRole).GetAwaiter().GetResult();

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.AdminBankLeasing.ToString()))
                roleManager.CreateAsync(adminBankLeasingRole).GetAwaiter().GetResult();

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.SupervisorLeasing.ToString()))
                roleManager.CreateAsync(supervisorleasingRole).GetAwaiter().GetResult();

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.BankLeasing.ToString()))
                roleManager.CreateAsync(leasingRole).GetAwaiter().GetResult();

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.Buyer.ToString()))
                roleManager.CreateAsync(buyerRole).GetAwaiter().GetResult();

            if (!roleManager.Roles.AsNoTracking().Any(p => p.Name == RoleEnum.Seller.ToString()))
                roleManager.CreateAsync(selleRole).GetAwaiter().GetResult();
        }
    }
}