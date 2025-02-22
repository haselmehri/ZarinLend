using Common;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model.GlobalSetting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class GlobalSettingService : IGlobalSettingService, IScopedDependency
    {
        private readonly ILogger<GlobalSettingService> logger;
        private readonly IBaseRepository<GlobalSetting> globalSettingRepository;

        public GlobalSettingService(ILogger<GlobalSettingService> logger, IBaseRepository<GlobalSetting> globalSettingRepository)
        {
            this.logger = logger;
            this.globalSettingRepository = globalSettingRepository;
        }

        public async Task<GlobalSettingViewModel?> GetActiveGlobalSetting(CancellationToken cancellationToken)
        {
            return await globalSettingRepository.TableNoTracking
                .OrderByDescending(p => p.IsActive)
                .Select(p => new GlobalSettingViewModel
                {
                    Id = p.Id,
                    FacilityInterest = p.FacilityInterest,
                    FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                    LendTechFacilityFee = p.LendTechFacilityFee,
                    LendTechFacilityForZarinpalClientFee = p.LendTechFacilityForZarinpalClientFee,
                    ValidationFee = p.ValidationFee,
                    ValidityPeriodOfValidation = p.ValidityPeriodOfValidation,
                    WarantyPercentage = p.WarantyPercentage,
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<PagingDto<GlobalSettingViewModel>> GetGlobalSettingList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var globalSettingList = await globalSettingRepository.TableNoTracking
                .OrderByDescending(p => p.IsActive)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new GlobalSettingViewModel
                {
                    Id = p.Id,
                    Creator = $"{p.Creator.Person.FName} {p.Creator.Person.LName}",
                    Updater = p.Updater != null ? $"{p.Updater.Person.FName} {p.Updater.Person.LName}" : string.Empty,
                    FacilityInterest = p.FacilityInterest,
                    FinancialInstitutionFacilityFee = p.FinancialInstitutionFacilityFee,
                    LendTechFacilityFee = p.LendTechFacilityFee,
                    LendTechFacilityForZarinpalClientFee = p.LendTechFacilityForZarinpalClientFee,
                    ValidationFee = p.ValidationFee,
                    ValidityPeriodOfValidation = p.ValidityPeriodOfValidation,
                    WarantyPercentage = p.WarantyPercentage,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    IsActive = p.IsActive
                }).ToListAsync(cancellationToken);

            var totalRowCounts = await globalSettingRepository.TableNoTracking.CountAsync();

            var pagingResult = new PagingDto<GlobalSettingViewModel>()
            {
                CurrentPage = filter.Page,
                Data = globalSettingList,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return pagingResult;
        }

        public async Task Add(GlobalSettingModel globalSettingModel, CancellationToken cancellationToken)
        {
            var activeGlobalSettings = globalSettingRepository.TableNoTracking.Where(p => p.IsActive);
            foreach (var globalSetting in activeGlobalSettings)
            {
                await globalSettingRepository.UpdateCustomPropertiesAsync(new GlobalSetting()
                {
                    Id = globalSetting.Id,
                    IsActive = false,
                    UpdaterId = globalSettingModel.UpdaterId
                }, cancellationToken, false, nameof(GlobalSetting.IsActive), nameof(GlobalSetting.UpdateDate), nameof(GlobalSetting.UpdaterId));
            }

            await globalSettingRepository.AddAsync(new GlobalSetting()
            {
                IsActive = true,
                ValidationFee = globalSettingModel.ValidationFee,
                LendTechFacilityFee = globalSettingModel.LendTechFacilityFee,
                LendTechFacilityForZarinpalClientFee = globalSettingModel.LendTechFacilityForZarinpalClientFee,
                CreatorId = globalSettingModel.CreatorId,
                FinancialInstitutionFacilityFee = globalSettingModel.FinancialInstitutionFacilityFee,
                FacilityInterest = globalSettingModel.FacilityInterest,
                ValidityPeriodOfValidation = globalSettingModel.ValidityPeriodOfValidation,
                WarantyPercentage = globalSettingModel.WarantyPercentage
            }, cancellationToken);
        }

    }
}
