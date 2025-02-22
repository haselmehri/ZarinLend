using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly IBaseRepository<Location> locationRepository;
        private readonly IBaseRepository<Organization> organizationRepository;

        public UserDataInitializer(UserManager<User> userManager, IBaseRepository<Location> locationRepository, IBaseRepository<Organization> organizationRepository)
        {
            this.userManager = userManager;
            this.locationRepository = locationRepository;
            this.organizationRepository = organizationRepository;
        }

        public int Order => 2;

        public void InitializeData()
        {
            var user = new User();

            #region Add 'system_admin' - for automatic works
            if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "system_admin"))
            {
                user = new User
                {
                    UserName = "system_admin",
                    Email = "system_admin@mail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    PhoneNumber = "02188994477",
                    IsActive = false,
                    Person = new Person()
                    {
                        FName = "سیستم",
                        LName = "اکانت",
                        FatherName = "-",
                        NationalCode = "-",
                        BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
                        Mobile = "-",
                        MobileConfirmed = true,
                        PostalCode = "-",
                        CountryId = 1,
                        CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
                        Address = "-",
                    }
                };

                var result = userManager.CreateAsync(user, "#@!$%^&&%$#@").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    userManager.AddToRolesAsync(user, new[] { RoleEnum.SuperAdmin.ToString() }).GetAwaiter().GetResult();
                }
            }
            #endregion

            #region Add 'haseli2684' - SuperAdmin User
            if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "haseli2684"))
            {
                user = new User
                {
                    UserName = "haseli2684",
                    Email = "haseli2684@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    PhoneNumber = "02188994477",
                    Person = new Person()
                    {
                        FName = "یداله",
                        LName = "حاصل مهری",
                        FatherName = "محمدرضا",
                        NationalCode = "0075661993",
                        BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
                        Mobile = "09126964896",
                        MobileConfirmed = true,
                        PostalCode = "1777919993",
                        OrganizationId = organizationRepository.GetByCondition(p => p.Name == "زرین پال").Id,
                        CountryId = 1,
                        CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
                        Address = "تهران-اتوبان شهید محلاتی-خ میثم-مجتمع عباسپور",
                    }
                };

                var result = userManager.CreateAsync(user, "2684897").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    userManager.AddToRolesAsync(user, new[] { RoleEnum.SuperAdmin.ToString() }).GetAwaiter().GetResult();
                }
            }
            #endregion

            #region Add 'zarinlendexpert' - ZarinLend Expert User
            if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "zarinlendexpert"))
            {
                user = new User
                {
                    UserName = "zarinlendexpert",
                    Email = "zarinlendexpert@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    PhoneNumber = "021889944077",
                    Person = new Person()
                    {
                        FName = "کارشناس",
                        LName = "زرین لند",
                        FatherName = "محمدرضا",
                        NationalCode = "0075661922",
                        BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
                        Mobile = "09126964889",
                        SSID = "8798",
                        MobileConfirmed = true,
                        PostalCode = "1777919023",
                        OrganizationId = organizationRepository.GetByCondition(p => p.Name == "زرین پال").Id,
                        CountryId = 1,
                        CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
                        Address = "تهران-اتوبان شهید محلاتی-خ میثم-مجتمع عباسپور",
                    }
                };

                var result = userManager.CreateAsync(user, "2684897").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    userManager.AddToRolesAsync(user, new[] { RoleEnum.ZarinLendExpert.ToString() }).GetAwaiter().GetResult();
                }
            }
            #endregion

            #region Add 'ayandeh1' -  کاربر کارشناس بانک آینده
            //if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "ayandeh1"))
            //{
            //    user = new User
            //    {
            //        UserName = "ayandeh1",
            //        Email = "ayandeh1@gmail.com",
            //        EmailConfirmed = true,
            //        PhoneNumberConfirmed = false,
            //        PhoneNumber = "0218234234",
            //        Person = new Person()
            //        {
            //            FName = "کارشناس",
            //            LName = "بانک آینده",
            //            FatherName = "آینده",
            //            NationalCode = "0021412487",
            //            BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
            //            Mobile = "09121234596",
            //            MobileConfirmed = true,
            //            PostalCode = "1777919995",
            //            OrganizationId = organizationRepository.GetByCondition(p => p.NationalId == 10320894878).Id,
            //            CountryId = 1,
            //            CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            BirthLocationId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            Address = "تهران",
            //        }
            //    };

            //    var result = userManager.CreateAsync(user, "2684897").GetAwaiter().GetResult();

            //    if (result.Succeeded)
            //    {
            //        userManager.AddToRolesAsync(user, new[] { RoleEnum.BankLeasing.ToString() }).GetAwaiter().GetResult();
            //    }
            //}
            #endregion

            #region Add 'ayandeh2' -  کاربر کارشناس بانک آینده
            //if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "ayandeh2"))
            //{
            //    user = new User
            //    {
            //        UserName = "ayandeh2",
            //        Email = "ayandeh2@gmail.com",
            //        EmailConfirmed = true,
            //        PhoneNumberConfirmed = false,
            //        PhoneNumber = "0218234234",
            //        Person = new Person()
            //        {
            //            FName = "کارشناس",
            //            LName = "بانک آینده",
            //            FatherName = "آینده",
            //            NationalCode = "0000000001",
            //            BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
            //            Mobile = "09121234533",
            //            BirthCertificateSerial = "000000",
            //            SSID = "0000",
            //            MobileConfirmed = true,
            //            PostalCode = "1777919995",
            //            OrganizationId = organizationRepository.GetByCondition(p => p.NationalId == 10320894878).Id,
            //            CountryId = 1,
            //            CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            CityOfIssueId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            BirthLocationId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            Address = "تهران",
            //        }
            //    };

            //    var result = userManager.CreateAsync(user, "2684897").GetAwaiter().GetResult();

            //    if (result.Succeeded)
            //    {
            //        userManager.AddToRolesAsync(user, new[] { RoleEnum.BankLeasing.ToString() }).GetAwaiter().GetResult();
            //    }
            //}
            #endregion

            #region Add 'Supervisor' -  کاربر سرپرست بانک آینده
            //if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "Supervisor"))
            //{
            //    user = new User
            //    {
            //        UserName = "Supervisor",
            //        Email = "Supervisor@gmail.com",
            //        EmailConfirmed = true,
            //        PhoneNumberConfirmed = false,
            //        PhoneNumber = "0218234234",
            //        Person = new Person()
            //        {
            //            FName = "سرپرست",
            //            LName = "بانک آینده",
            //            FatherName = "آینده",
            //            NationalCode = "0000000002",
            //            BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
            //            Mobile = "09121234500",
            //            BirthCertificateSerial = "000000",
            //            SSID = "0000",
            //            MobileConfirmed = true,
            //            PostalCode = "1777919999",
            //            OrganizationId = organizationRepository.GetByCondition(p => p.NationalId == 10320894878).Id,
            //            CountryId = 1,
            //            CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            CityOfIssueId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            BirthLocationId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            Address = "تهران",
            //        }
            //    };

            //    var result = userManager.CreateAsync(user, "2684897").GetAwaiter().GetResult();

            //    if (result.Succeeded)
            //    {
            //        userManager.AddToRolesAsync(user, new[] { RoleEnum.SupervisorLeasing.ToString() }).GetAwaiter().GetResult();
            //    }
            //}
            #endregion

            #region Add 'admin_ayandeh' -  کاربر ادمین بانک آینده
            //if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "admin_ayandeh"))
            //{
            //    user = new User
            //    {
            //        UserName = "admin_ayandeh",
            //        Email = "admin_ayandeh@gmail.com",
            //        EmailConfirmed = true,
            //        PhoneNumberConfirmed = false,
            //        PhoneNumber = "0218234234",
            //        Person = new Person()
            //        {
            //            FName = "ادمین",
            //            LName = "بانک آینده",
            //            FatherName = "آینده",
            //            NationalCode = "0021412477",
            //            BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
            //            Mobile = "09121234597",
            //            MobileConfirmed = true,
            //            PostalCode = "1777919999",
            //            BirthCertificateSerial = "000000",
            //            SSID = "0000",
            //            OrganizationId = organizationRepository.GetByCondition(p => p.NationalId == 10320894878).Id,
            //            CountryId = 1,
            //            CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            BirthLocationId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            Address = "تهران",
            //        }
            //    };

            //    var result = userManager.CreateAsync(user, "2684897").GetAwaiter().GetResult();

            //    if (result.Succeeded)
            //    {
            //        userManager.AddToRolesAsync(user, new[] { RoleEnum.AdminBankLeasing.ToString() }).GetAwaiter().GetResult();
            //    }
            //}
            #endregion

            #region Add 'seller1' -  کاربر فروشگاه
            //if (!userManager.Users.AsNoTracking().Any(p => p.UserName == "seller1"))
            //{
            //    user = new User
            //    {
            //        UserName = "seller1",
            //        //Email = "ayandeh1@gmail.com",
            //        EmailConfirmed = true,
            //        PhoneNumberConfirmed = false,
            //        PhoneNumber = "0218234234",
            //        Person = new Person()
            //        {
            //            FName = "محمد",
            //            LName = "احمدی",
            //            SSID=string.Empty,
            //            FatherName = "آینده",
            //            NationalCode = "0021412400",
            //            BirthDate = DateTimeHelper.ShamsiToGregorian("1364/02/22"),
            //            Mobile = "09121234000",
            //            MobileConfirmed = true,
            //            PostalCode = "1777919900",
            //            OrganizationId = organizationRepository.GetByCondition(p => p.NationalId == 10000001).Id,
            //            CountryId = 1,
            //            CityId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            BirthLocationId = locationRepository.GetByCondition(p => p.LocationType == LocationTypeEnum.City && p.Name == "تهران").Id,
            //            Address = "تهران",
            //        }
            //    };

            //    var result = userManager.CreateAsync(user, "2684897").GetAwaiter().GetResult();

            //    if (result.Succeeded)
            //    {
            //        userManager.AddToRolesAsync(user, new[] { RoleEnum.Seller.ToString() }).GetAwaiter().GetResult();
            //    }
            //}
            #endregion
        }
    }
}