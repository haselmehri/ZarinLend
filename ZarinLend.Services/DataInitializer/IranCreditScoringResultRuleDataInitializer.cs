using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataInitializer
{
    public class IranCreditScoringResultRuleDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository;
        private readonly IUserRepository userRepository;

        public int Order => 10;

        public IranCreditScoringResultRuleDataInitializer(IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository,
                                                          IUserRepository userRepository)
        {
            this.iranCreditScoringResultRuleRepository = iranCreditScoringResultRuleRepository;
            this.userRepository = userRepository;
        }

        public void InitializeData()
        {
            var system_admin_userId =
                userRepository.GetColumnValue<Guid>(p => p.UserName.Equals("system_admin") &&
                p.UserRoles.Any(x => x.Role.Name == RoleEnum.SuperAdmin.ToString() || x.Role.Name == RoleEnum.Admin.ToString()));

            if (!iranCreditScoringResultRuleRepository.TableNoTracking.Any())
            {
                iranCreditScoringResultRuleRepository.AddRange(new List<IranCreditScoringResultRule>()
                {
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = false,
                        IsActive=true,
                        MaximumAmount=500000000,
                        MinimumAmount = null,
                        Risk = "A",
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacility
                    },
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = false,
                        IsActive=true,
                        MaximumAmount=500000000,
                        MinimumAmount = null,
                        Risk = "B",
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacility
                    },
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = true,
                        IsActive=true,
                        MaximumAmount=300000000,
                        MinimumAmount = null,
                        Risk = "C",
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacility
                    },
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = true,
                        IsActive=true,
                        MaximumAmount=300000000,
                        MinimumAmount = null,
                        Risk = "D",
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacility
                    },
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = false,
                        IsActive=true,
                        MaximumAmount=0,
                        MinimumAmount = 0,
                        Risk = "E",
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacility
                    },
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = false,
                        IsActive=true,
                        MaximumAmount=200000000,
                        MinimumAmount = 0,
                        Risk = null,
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacility
                    },
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = true,
                        IsActive=true,
                        MaximumAmount=500000000,
                        MinimumAmount = 0,
                        Risk = null,
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacility
                    },
                    new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = false,
                        IsActive=true,
                        MaximumAmount=0,
                        MinimumAmount = 0,
                        Risk = "A",
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacilityGuarantor
                    },
                     new()
                    {
                        CreatorId = system_admin_userId,
                        GuarantorIsRequired = false,
                        IsActive=true,
                        MaximumAmount=0,
                        MinimumAmount = 0,
                        Risk = "B",
                        IranCreditScoringResultRuleType = IranCreditScoringResultRuleType.ForRequestFacilityGuarantor
                    }
                });
            }
        }
    }
}