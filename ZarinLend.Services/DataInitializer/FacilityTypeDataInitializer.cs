using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class FacilityTypeDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<FacilityType> facilityTypeRepository;
        private readonly IOrganizationRepository organizationRepository;

        public FacilityTypeDataInitializer(IBaseRepository<FacilityType> facilityTypeRepository, IOrganizationRepository organizationRepository)
        {
            this.facilityTypeRepository = facilityTypeRepository;
            this.organizationRepository = organizationRepository;
        }

        public int Order => 1;      
        public void InitializeData()
        {
            var organization = organizationRepository.GetByCondition(p => p.NationalId == 10320894878);
            if (organization == default(Organization)) return;

            if (!facilityTypeRepository.TableNoTracking.Any(p => p.MonthCount == 6))
            {
                facilityTypeRepository.Add(new FacilityType
                {
                    OrganizationId = organization.Id,
                    MonthCount = 6,
                    MonthCountTitle = "6 ماه",
                    Description = "6 ماهه"
                });
            }

            if (!facilityTypeRepository.TableNoTracking.Any(p => p.MonthCount == 12))
            {
                facilityTypeRepository.Add(new FacilityType
                {
                    OrganizationId = organization.Id,
                    MonthCount = 12,
                    MonthCountTitle = "12 ماه",
                    Description ="12 ماهه"
                });
            }
            if (!facilityTypeRepository.TableNoTracking.Any(p => p.MonthCount == 18))
            {
                facilityTypeRepository.Add(new FacilityType
                {
                    OrganizationId = organization.Id,
                    MonthCount = 18,
                    MonthCountTitle = "18 ماه",
                    Description = "18 ماهه"
                });
            }
            if (!facilityTypeRepository.TableNoTracking.Any(p => p.MonthCount == 24))
            {
                facilityTypeRepository.Add(new FacilityType
                {
                    OrganizationId = organization.Id,
                    MonthCount = 24,
                    MonthCountTitle = "24 ماه",
                    Description = "24 ماهه"
                });
            }
        }
    }
}