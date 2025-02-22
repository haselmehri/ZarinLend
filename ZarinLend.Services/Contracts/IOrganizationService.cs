using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Dto;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Common.Utilities.CultureHelpaer;

namespace Services
{
    public interface IOrganizationService
    {
        Task<List<SelectListItem>> SelectOrganizationByOganizationType(short organizationTypeId, CancellationToken cancellationToken);
        Task<OrganizationModel> GetOrganization(int organizationId, CancellationToken cancellationToken);
        Task<string> GetOrganizationName(int organizationId, CancellationToken cancellationToken);
        Task<PagingDto<OrganizationModel>> SelectOrganization(PagingFilterDto filter, CancellationToken cancellationToken = default);
        Task Edit(OrganizationModel organizationModel, CancellationToken cancellationToken = default);
        Task Add(OrganizationModel organizationModel, CancellationToken cancellationToken = default);
        Task<OrganizationModel> PrepareModelForAdd(CancellationToken cancellationToken);
        Task<OrganizationModel> PrepareModelForEdit(int Id, CancellationToken cancellationToken);
    }
}