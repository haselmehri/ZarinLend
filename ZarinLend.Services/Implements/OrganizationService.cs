using AutoMapper;
using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class OrganizationService : IOrganizationService, IScopedDependency
    {
        private readonly IMapper mapper;
        private readonly ILogger<OrganizationService> logger;
        private readonly IOrganizationRepository organizationRepository;
        private readonly IBaseRepository<OrganizationType> organizationTypeRepository;

        public OrganizationService(IMapper mapper, ILogger<OrganizationService> logger, IOrganizationRepository organizationRepository,
            IBaseRepository<OrganizationType> organizationTypeRepository)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.organizationRepository = organizationRepository;
            this.organizationTypeRepository = organizationTypeRepository;
        }

        public async Task<List<SelectListItem>> SelectOrganizationByOganizationType(short organizationTypeId, CancellationToken cancellationToken)
        {
            var cities = (await organizationRepository.SelectByAsync(p => p.OrganizationTypeId.Equals(organizationTypeId) && p.IsActive.Equals(true),
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList();

            return cities;
        }

        public async Task<OrganizationModel> GetOrganization(int organizationId, CancellationToken cancellationToken)
        {
            return (await organizationRepository.SelectByAsync(p => p.Id.Equals(organizationId),
                p => new OrganizationModel()
                {
                    Id = p.Id,
                    Address = p.Address,
                    Name = p.Name,
                    NationalId = p.NationalId,
                    OrganizationTypeId = p.OrganizationTypeId,
                    OrganizationTypeName = p.OrganizationType.Name,
                    SiteUrl = p.SiteUrl,
                    Tel = p.Tel
                }, cancellationToken))
                .FirstOrDefault();
        }

        public virtual async Task<PagingDto<OrganizationModel>> SelectOrganization(PagingFilterDto filter, CancellationToken cancellationToken = default)
        {
            var result = await organizationRepository.SelectByAsync(p => new OrganizationModel()
            {
                Id = p.Id,
                Name = p.Name,
                OrganizationTypeId = p.OrganizationTypeId,
                OrganizationTypeName = p.OrganizationType.Name,
                SiteUrl = p.SiteUrl,
                Tel = p.Tel,
                NationalId = p.NationalId,
                Address = p.Address,
                IsActive = p.IsActive
            },
            cancellationToken, pageNumber: filter.Page, pageSize: filter.PageSize);

            var totalRowCount = await organizationRepository.TableNoTracking.CountAsync();

            return new PagingDto<OrganizationModel>()
            {
                CurrentPage = filter.Page,
                Data = result,
                TotalRowCount = totalRowCount,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCount) / filter.PageSize)
            };
        }

        public virtual async Task Add(OrganizationModel organizationModel, CancellationToken cancellationToken = default)
        {
            if (await organizationRepository.TableNoTracking.AnyAsync(p => p.Name == organizationModel.Name))
                throw new AppException("نام سامان/لیزینگ/فروشگاه تکراری می باشد");

            if (await organizationRepository.TableNoTracking.AnyAsync(p => p.NationalId == organizationModel.NationalId))
                throw new AppException("شناسه ملی تکراری می باشد");

            var organization = new Organization()
            {
                Name = organizationModel.Name,
                Address = organizationModel.Address,
                SiteUrl = organizationModel.SiteUrl,
                IsActive = organizationModel.IsActive,
                Tel = organizationModel.Tel,
                OrganizationTypeId = organizationModel.OrganizationTypeId
            };
            await organizationRepository.AddAsync(organization, cancellationToken);
        }

        public virtual async Task Edit(OrganizationModel organizationModel, CancellationToken cancellationToken = default)
        {
            if (await organizationRepository.TableNoTracking.AnyAsync(p => p.Name == organizationModel.Name && !p.Id.Equals(organizationModel.Id)))
                throw new AppException("نام سامان/لیزینگ/فروشگاه تکراری می باشد");

            if (await organizationRepository.TableNoTracking.AnyAsync(p => p.NationalId == organizationModel.NationalId && !p.Id.Equals(organizationModel.Id)))
                throw new AppException("شناسه ملی تکراری می باشد");

            //var user = await userManager.FindByIdAsync(userModel.Id.ToString());
            var organization = await organizationRepository.GetByIdAsync(cancellationToken, organizationModel.Id);
            if (organization == default(Organization))
                throw new NotFoundException();

            organization.Name = organizationModel.Name;
            organization.Address = organizationModel.Address;
            organization.SiteUrl = organizationModel.SiteUrl;
            organization.IsActive = organizationModel.IsActive;
            organization.Tel = organizationModel.Tel;
            organization.OrganizationTypeId = organizationModel.OrganizationTypeId;

            await organizationRepository.UpdateAsync(organization, cancellationToken);
        }

        public async Task<string> GetOrganizationName(int organizationId, CancellationToken cancellationToken)
        {
            return await organizationRepository.GetColumnValueAsync<string>(p => p.Id.Equals(organizationId), cancellationToken, "Name");
        }

        public async Task<OrganizationModel> PrepareModelForAdd(CancellationToken cancellationToken)
        {
            return new OrganizationModel()
            {
                OrganizationTypes = (await organizationTypeRepository.SelectByAsync(p => p.IsActive.Equals(true),
                     p => new SelectListItem
                     {
                         Text = p.Name,
                         Value = p.Id.ToString()
                     }, cancellationToken))
                     .ToList()
            };
        }

        public async Task<OrganizationModel> PrepareModelForEdit(int Id, CancellationToken cancellationToken)
        {
            var organization = await organizationRepository.GetByIdAsync(cancellationToken, Id);

            if (organization == default(Organization))
                return null;

            return new OrganizationModel()
            {
                Id = organization.Id,
                Address = organization.Address,
                Name = organization.Name,
                Tel = organization.Tel,
                SiteUrl = organization.SiteUrl,
                NationalId = organization.NationalId,
                IsActive = organization.IsActive,
                OrganizationTypeId = organization.OrganizationTypeId,
                OrganizationTypes = (await organizationTypeRepository.SelectByAsync(p => p.IsActive.Equals(true),
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString(),
                        Selected = p.Id == organization.OrganizationTypeId
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList(),
                IsEditMode = true
            };
        }
    }
}
