using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Dto;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Common.Utilities.CultureHelpaer;

namespace Services
{
    public interface IPlanService
    {
        //Task<OrganizationModel> GetOrganization(int organizationId, CancellationToken cancellationToken);
        //Task<string> GetOrganizationName(int organizationId, CancellationToken cancellationToken);
        Task<PagingDto<PlanListModel>> SelectPlans(PagingFilterDto filter, CancellationToken cancellationToken = default);
        Task<List<PlanMemberModel>> GetMembers(int planId, CancellationToken cancellationToken);
        //Task Edit(OrganizationModel organizationModel, CancellationToken cancellationToken = default);
        Task Add(PlanAddModel organizationModel, IFormFile membersExcelFile, CancellationToken cancellationToken = default);
        Task<PlanModel> PrepareModelForAdd(CancellationToken cancellationToken);
        Task<List<PlanMemberModel>> VerifyPlanMembesExcel(IFormFile membersExcelFile,CancellationToken cancellation);
        //Task<OrganizationModel> PrepareModelForEdit(int Id, CancellationToken cancellationToken);
    }
}