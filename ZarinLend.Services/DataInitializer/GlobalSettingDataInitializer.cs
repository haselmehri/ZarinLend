using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System;
using System.Linq;
using System.Threading;

namespace Services.DataInitializer
{
    public class GlobalSettingDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<GlobalSetting> globaSettingRepository;
        private readonly IUserRepository userRepository;

        public GlobalSettingDataInitializer(IBaseRepository<GlobalSetting> globaSettingRepository, IUserRepository userRepository)
        {
            this.globaSettingRepository = globaSettingRepository;
            this.userRepository = userRepository;
        }

        public int Order => 10;
        public void InitializeData()
        {
            var adminUserId =
                 userRepository.GetColumnValue<Guid?>(p => p.UserName.Equals("system_admin") &&
                 p.UserRoles.Any(x => x.Role.Name == RoleEnum.SuperAdmin.ToString() || x.Role.Name == RoleEnum.Admin.ToString()));

            if (adminUserId == null) return;

            if (!globaSettingRepository.TableNoTracking.Any())
            {
                globaSettingRepository.Add(new GlobalSetting
                {
                    ValidationFee = 200000,
                    ValidityPeriodOfValidation = 30,//روز
                    FacilityInterest = 23,
                    FinancialInstitutionFacilityFee = 6,
                    LendTechFacilityFee = 9,
                    WarantyPercentage = 120,
                    CreatorId = adminUserId.Value,
                    UpdaterId = adminUserId.Value
                });
            }
        }
    }
}