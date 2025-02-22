using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class OrganizationDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<Organization> organizationRepository;

        public OrganizationDataInitializer(IBaseRepository<Organization> organizationRepository)
        {
            this.organizationRepository = organizationRepository;
        }

        public int Order => 0;
        public void InitializeData()
        {
            if (!organizationRepository.TableNoTracking.Any(p => p.Name == "زرین پال"))
            {
                organizationRepository.Add(new Organization
                {
                    Name = "زرین پال",
                    Address = "تهران، یوسف آباد، کوچه بیستم، پلاک 61، واحد ",
                    IsActive = true,
                    NationalId = 11111111111,
                    OrganizationTypeId = (short)OrganizationTypeEnum.ZarinLend,
                    Tel = "02141239",
                });
            }
            if (!organizationRepository.TableNoTracking.Any(p => p.NationalId == 10320894878))
            {
                organizationRepository.Add(new Organization
                {
                    Name = "بانک آینده سهامی عام",
                    Address = "تهران،بزرگراه مدرس جنوب به شمال،تقاطع نلسون ماندلا،ابتدای خیابان بیدار،شماره یک(برج آینده)",
                    IsActive = true,
                    NationalId = 10320894878,
                    OrganizationTypeId = (short)OrganizationTypeEnum.BankLeasing,
                    Tel = "88442211",
                });
            }

            if (!organizationRepository.TableNoTracking.Any(p => p.NationalId == 10000001))
            {
                organizationRepository.Add(new Organization
                {
                    Name = "فروشگاه زرین",
                    Address = "",
                    IsActive = true,
                    NationalId = 10000001,
                    OrganizationTypeId = (short)OrganizationTypeEnum.Shop,
                    Tel = "88996633",
                });
            }
        }
    }
}