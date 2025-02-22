using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class OrganizationTypeDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<OrganizationType> organizationTypeRepository;

        public OrganizationTypeDataInitializer(IBaseRepository<OrganizationType> organizationTypeRepository)
        {
            this.organizationTypeRepository = organizationTypeRepository;
        }

        public int Order => -1;
        public void InitializeData()
        {
            if (!organizationTypeRepository.TableNoTracking.Any(p => p.Id == (short)OrganizationTypeEnum.BankLeasing))
            {
                organizationTypeRepository.Add(new OrganizationType
                {
                    Id = (short)OrganizationTypeEnum.BankLeasing,
                    Name = "بانک/لیزینگ",
                    Description = "بانک ها/شرکت های تامین کننده اعتبارات"
                });
            }
            if (!organizationTypeRepository.TableNoTracking.Any(p => p.Id == (short)OrganizationTypeEnum.Company))
            {
                organizationTypeRepository.Add(new OrganizationType
                {
                    Id = (short)OrganizationTypeEnum.Company,
                    Name = "حقوقی",
                    Description = "خریدارن حقوقی"
                });
            }
            //if (!organizationTypeRepository.TableNoTracking.Any(p => p.Id == (short)OrganizationTypeEnum.Person))
            //{
            //    organizationTypeRepository.Add(new OrganizationType
            //    {
            //        Id = (short)OrganizationTypeEnum.Person,
            //        Name = "حقیقی",
            //        Description = "خریدارن حقیقی"
            //    });
            //}
            if (!organizationTypeRepository.TableNoTracking.Any(p => p.Id == (short)OrganizationTypeEnum.Shop))
            {
                organizationTypeRepository.Add(new OrganizationType
                {
                    Id = (short)OrganizationTypeEnum.Shop,
                    Name = "فروشگاه",
                    Description = "فروشندگان/فروشگاه"
                });
            }
            if (!organizationTypeRepository.TableNoTracking.Any(p => p.Id == (short)OrganizationTypeEnum.ZarinLend))
            {
                organizationTypeRepository.Add(new OrganizationType
                {
                    Id = (short)OrganizationTypeEnum.ZarinLend,
                    Name = "زرین لند",
                    Description = "زرین لند"
                });
            }
        }
    }
}