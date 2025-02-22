using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model.IranCreditScoring;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class IranCreditScoringResultRuleService : IIranCreditScoringResultRuleService, IScopedDependency
    {
        private readonly ILogger<IranCreditScoringResultRuleService> logger;
        private readonly IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository;

        public IranCreditScoringResultRuleService(ILogger<IranCreditScoringResultRuleService> logger, IBaseRepository<IranCreditScoringResultRule> iranCreditScoringResultRuleRepository)
        {
            this.logger = logger;
            this.iranCreditScoringResultRuleRepository = iranCreditScoringResultRuleRepository;
        }

        public async Task<IranCreditScoringResultRuleAddEditModel> Get(int id, CancellationToken cancellationToken)
        {
            return await iranCreditScoringResultRuleRepository.TableNoTracking
                .Where(x => x.Id == id)
                .Select(p => new IranCreditScoringResultRuleAddEditModel
                {
                    Id = p.Id,
                    GuarantorIsRequired = p.GuarantorIsRequired,
                    IsActive = p.IsActive,
                    IranCreditScoringResultRuleType = p.IranCreditScoringResultRuleType,
                    MaximumAmount = p.MaximumAmount,
                    MinimumAmount = p.MinimumAmount,
                }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagingDto<IranCreditScoringResultRuleListModel>> GetList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await iranCreditScoringResultRuleRepository.TableNoTracking
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.CreatedDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new IranCreditScoringResultRuleListModel
                {
                    Id = p.Id,
                    GuarantorIsRequired = p.GuarantorIsRequired,
                    IranCreditScoringResultRuleType = p.IranCreditScoringResultRuleType,
                    MaximumAmount = p.MaximumAmount,
                    MinimumAmount = p.MinimumAmount,
                    Risk = p.Risk,
                    Creator = $"{p.Creator.Person.FName} {p.Creator.Person.LName}",
                    Updater = p.Updater != null ? $"{p.Updater.Person.FName} {p.Updater.Person.LName}" : string.Empty,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    IsActive = p.IsActive
                }).ToListAsync(cancellationToken);

            var totalRowCounts = await iranCreditScoringResultRuleRepository.TableNoTracking.CountAsync();

            var pagingResult = new PagingDto<IranCreditScoringResultRuleListModel>()
            {
                CurrentPage = filter.Page,
                Data = result,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return pagingResult;
        }

        public async Task Add(IranCreditScoringResultRuleAddEditModel model, CancellationToken cancellationToken)
        {
            if (iranCreditScoringResultRuleRepository.TableNoTracking
                .Any(p => p.Risk == model.Risk.Trim() &&
                          p.GuarantorIsRequired == model.GuarantorIsRequired &&
                          p.MinimumAmount == model.MinimumAmount &&
                          p.MaximumAmount == model.MaximumAmount &&
                          p.IranCreditScoringResultRuleType == model.IranCreditScoringResultRuleType))
            {
                throw new AppException("اطلاعات تکراری می باشد!");
            }

            await iranCreditScoringResultRuleRepository.AddAsync(new IranCreditScoringResultRule()
            {
                Risk = model.Risk,
                MinimumAmount = model.MinimumAmount,
                MaximumAmount = model.MaximumAmount,
                GuarantorIsRequired = model.GuarantorIsRequired,
                IranCreditScoringResultRuleType = model.IranCreditScoringResultRuleType,
                IsActive = model.IsActive,
                CreatorId = model.CreatorId,
            }, cancellationToken);
        }

        public async Task Edit(IranCreditScoringResultRuleAddEditModel model, CancellationToken cancellationToken)
        {
            if (iranCreditScoringResultRuleRepository.TableNoTracking
                .Any(p => p.Id != model.Id &&
                          p.Risk == model.Risk.Trim() &&
                          p.GuarantorIsRequired == model.GuarantorIsRequired &&
                          p.MinimumAmount == model.MinimumAmount &&
                          p.MaximumAmount == model.MaximumAmount &&
                          p.IranCreditScoringResultRuleType == model.IranCreditScoringResultRuleType))
            {
                throw new AppException("اطلاعات تکراری می باشد!");
            }

            await iranCreditScoringResultRuleRepository.UpdateCustomPropertiesAsync(new IranCreditScoringResultRule()
            {
                Id = model.Id,
                Risk = model.Risk,
                MinimumAmount = model.MinimumAmount,
                MaximumAmount = model.MaximumAmount,
                GuarantorIsRequired = model.GuarantorIsRequired,
                IranCreditScoringResultRuleType = model.IranCreditScoringResultRuleType,
                IsActive = model.IsActive,
                UpdaterId = model.UpdaterId,
            }, cancellationToken, saveNow: true,
            nameof(IranCreditScoringResultRule.Risk),
            nameof(IranCreditScoringResultRule.MinimumAmount),
            nameof(IranCreditScoringResultRule.MaximumAmount),
            nameof(IranCreditScoringResultRule.GuarantorIsRequired),
            nameof(IranCreditScoringResultRule.IranCreditScoringResultRuleType),
            nameof(IranCreditScoringResultRule.IsActive),
            nameof(IranCreditScoringResultRule.UpdaterId));
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            if (await iranCreditScoringResultRuleRepository.TableNoTracking.CountAsync(p => p.Id == id && p.RequestFacilities.Any(), cancellationToken) > 0 ||
                await iranCreditScoringResultRuleRepository.TableNoTracking.CountAsync(p => p.Id == id && p.RequestFacilityGuarantors.Any(), cancellationToken) > 0)
                throw new AppException("بدلیل استفاده از رکورد فوق در ثبت درخواست تسهیلات یا ثبت ضامن امکان حذف وجود ندارد");

            await iranCreditScoringResultRuleRepository.DeleteByIdAsync(id, cancellationToken);
        }
    }
}
