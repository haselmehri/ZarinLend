using Core.Entities;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IRequestFacilityGuarantorService
    {
        Task<RequestFacilityGuarantorAddModel> PrepareModelForAdd(string userRisk, CancellationToken cancellationToken);
        Task<int> RegisterGuarantor(Guid guarantorUserId, int requestFacilityId, CancellationToken cancellationToken);
        Task<List<CompletedWorkFlowStepModel>> GetRequestFacilityGuarantorSteps(int requestFacilityGuarantorId, CancellationToken cancellationToken);
        Task<bool> HasGaurantorWithApprovedWorkFlow(int requestFacilityId, CancellationToken cancellationToken);
        Task CancelByUser(int requestFacilityGuarantorId, Guid userId, CancellationToken cancellationToken);
        Task<PagingDto<RequestFacilityGuarantorListModel>> GetRequests(PagingFilterDto filter, List<RoleEnum> roles, CancellationToken cancellationToken);
        Task<RequestFacilityGuarantorInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityGuarantorId, WorkFlowFormEnum workFlowForm, CancellationToken cancellationToken);
        Task<RequestFacilityGuarantorInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityGuarantorId, Guid? guarantorUserId = null, CancellationToken cancellationToken = default);
        Task<int?> GetRequestFacilityId(int id, CancellationToken cancellationToken);
        Task<bool> VerifyGuarantorByZarinLend(RequestFacilityGuarantorStatusModel statusModel, CancellationToken cancellationToken);
        Task<bool> VerifyGuaranteesByZarinLend(RequestFacilityGuarantorStatusModel statusModel, CancellationToken cancellationToken);

        #region Get/Check RequestFacilityGuarantorId Waiting in Specified Step And Role
        Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(Guid userId, int requestFacilityGuarantorId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
         Task<int?> GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(int id, Guid userId, List<RoleEnum> roles,
            WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<int?> GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum,
            CancellationToken cancellationToken);

        #endregion  Get RequestFacilityGuarantorId Waiting in Specified Step And Role
    }
}