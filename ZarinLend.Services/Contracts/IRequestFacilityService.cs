using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model;
using ZarinLend.Services.Model.Ayandeh.BankAccount;

namespace Services
{
    public interface IRequestFacilityService
    {

        #region Apis for External usings
        Task<bool> MoveToVerifyStepFromUploadDocuments(int requestFacilityId, Guid userId, CancellationToken cancellationToken);

        #endregion Apis for External usings

        Task<bool?> GetValidationMustBeRepeated(int id, CancellationToken cancellationToken);
        Task AssignRequestFacilityToUser(AssignRequestFacilityToUserModel model, CancellationToken cancellationToken);
        Task<List<int>> GetAllRequestFacilityIdsForSign(int leasingId, PagingFilterDto filter, List<int> checkedRequestFacilityIds, List<RoleEnum> roles, CancellationToken cancellationToken);
        #region singning by bank admin
        Task<string> SaveSignedContractByBankAndMoveOnWorkFlow(int requestFacilityId, int leasingId, Guid opratorId, byte[] signContract, string digest, string certificate, string signature, 
            CancellationToken cancellationToken = default);
        Task<bool> ApprovedAdminLeasingWithoutSignContract(RequestStatusModel model, CancellationToken cancellationToken = default);
        Task<bool> ReturnToCorrection(RequestStatusModel model, CancellationToken cancellationToken = default);
        Task SaveSignedContractByAdminAndMoveOnWorkFlow(int requestFacilityId, int leasingId, byte[] signContract, CancellationToken cancellationToken = default);

        #endregion singning by bank admin

        Task SaveSignedContractAndMoveOnWorkFlow(int requestFacilityId, byte[] signContract, CancellationToken cancellationToken = default);
        Task TestSaveSignedContractAndMoveOnWorkFlow(int requestFacilityId, CancellationToken cancellationToken = default);
        Task<bool> UserHasOpenRequest(Guid userId, CancellationToken cancellationToken);
        Task<long> GetUserSumAmountApprovedFacilities(Guid userId, CancellationToken cancellationToken);
        Task<FindOpenRequestFacilityResultModel> FindOpenRequestFacility(string nationalCode, CancellationToken cancellationToken);
        Task<bool> IsOpenRequestFacility(int requestFacilityId, CancellationToken cancellationToken);
        Task<string> GetSignedContractByUserFileName(int id, CancellationToken cancellationToken);
        Task<string> GetAyandehSigningToken(int id, CancellationToken cancellationToken);
        Task<string> GetAyandehSignSigningTokenForAdminBank(int id, CancellationToken cancellationToken);
        Task<bool> VerifyCheckByLeasing(RequestStatusModel statusModel, int leasingId, CancellationToken cancellationToken);
        Task<bool> VerifyCheckByZarinLend(RequestStatusModel statusModel, CancellationToken cancellationToken);
        Task<bool> GoToNextStepFromVerifyToPendingResultVerify(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> GoToNextStepFromEnterFacilityToPendingEnterFacility(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> GoToNextStepFromBankLeasingInquiryToPendingBankLeasingInquiry(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> GoToNextStepFromDepositFacilityAmountToPendingResultDepositFacilityAmount(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> GoToNextStepFromChargeCardToPendingResultChargeCard(int requestFacilityId, int leasingId, Guid userId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> VerifyByLeasing(PrimaryVerifyModel statusModel, ApplicantValidationResultModel applicantValidationResultModel, int leasingId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> EnterFacilityNumber(EnterFacilityNumberModel statusModel, int leasingId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> VerifyAndAllocationByLeasing(VerifyAndPoliceNumberModel statusModel, int leasingId, bool autoSave, CancellationToken cancellationToken);
        Task<bool> SetGuarantorIsRequired(RequestGuarantorIsRequiredModel statusModel, CancellationToken cancellationToken);
        Task<bool> CheckGuarantorIsRequired(int requestFacilityId, CancellationToken cancellationToken);
        Task UpdateAwaitingIntroductionGuarantor(int requestFacilityId, bool awaitingIntroductionGuarantor, bool saveNow, CancellationToken cancellationToken);
        Task<bool> VerifyByZarinLend(VerifyByZarinLendRequestStatusModel statusModel, CancellationToken cancellationToken);
        Task DepositFacilityAmount(RequestFacilityDepositDocumentModel model, IFormFile? file, int leasingId, CancellationToken cancellationToken = default);
        Task CardRecharge(int requestFacilityId, Guid creatorId, int leasingId, CancellationToken cancellationToken = default);
        Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, Guid userId, CancellationToken cancellationToken);
        Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, CancellationToken cancellationToken);
        Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, WorkFlowFormEnum workFlowForm, CancellationToken cancellationToken);
        Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, int leasingId, CancellationToken cancellationToken);
        Task<RequestFacilityInfoModel> GetRequestFacilityCompleteInfo(int requestFacilityId, int leasingId, WorkFlowFormEnum workFlowForm, CancellationToken cancellationToken);
        Task<List<SelectListItem>> GetFacilityTypes(CancellationToken cancellationToken);
        Task<List<SelectListItem>> GetUsagePlaces(CancellationToken cancellationToken);
        Task<RequestFacilityModel> PrepareModelForAdd(Guid userId, string userRisk, CancellationToken cancellationToken);
        Task<int> AddRequestFacilityByBuyer(RequestFacilityAddModel requestFacilityModel, CancellationToken cancellationToken);
        Task CancelByUser(int requestFacilityId, Guid userId, CancellationToken cancellationToken);
        Task CancelByZarinLendAdmin(int requestFacilityId, Guid userId, CancellationToken cancellationToken);
        Task<PagingDto<RequestFacilityListModel>> GetBuyerRequests(Guid userId, PagingFilterDto filter, CancellationToken cancellationToken);
        Task<PagingDto<RequestFacilityListModel>> GetAllLeasingRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, CancellationToken cancellationToken);
        Task<PagingDto<RequestFacilityListModel>> SearchLeasingRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken);
        Task<PagingDto<RequestFacilityListModel>> SearchRequest(PagingFilterDto filter, List<RoleEnum> roles, bool executeForExport, CancellationToken cancellationToken);
        Task<PagingDto<PendingInEnterFacilityNumberModel>> SearchPendingInEnterFacilityNumberRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken);
        Task<PagingDto<PendingInEnterPoliceNumberModel>> SearchPendingInEnterPoliceNumberRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken);
        Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(Guid userId, int requestFacilityId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<PagingDto<PendingInEnterPoliceNumberModel>> SearchPendingInDepositFacilityAmountRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken);
        Task<PagingDto<PendingInCardRechargeModel>> SearchPendingInChargeCardRequest(int leasingId, List<RoleEnum> roles, PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken);
        Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, int leasingId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<bool> CheckRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, int leasingId, Guid operatorId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<int?> GetRequestFacilityIdWaitingSpecifiedStepAndRole(int requestFacilityId, Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<int?> GetRequestFacilityIdWaitingSpecifiedStepAndRole(Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<RequestFacilityDetailModel> GetRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<RequestFacilityDetailModel> GetRequestFacilityWaitingSpecifiedStepAndRole(Guid userId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<RequestFacilityDetailModel> GetRequestFacilityWaitingSpecifiedStepAndRole(int requestFacilityId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<List<CompletedWorkFlowStepModel>> GetRequestFacilitySteps(int requestFacilityId, CancellationToken cancellationToken);
        Task<List<RequestFacilityWorkFlowStepHistoryModel>> GetRequestFacilityStepsHistory(int requestFacilityId, CancellationToken cancellationToken);
        Task<List<RequestFacilityWorkFlowStepHistoryModel>> GetRequestFacilityStepsHistory(int requestFacilityId, Guid buyerId, CancellationToken cancellationToken);
        Task<List<RequestFacilityWorkFlowStepHistoryModel>> GetRequestFacilityStepsHistory(int requestFacilityId, int organizationId, CancellationToken cancellationToken);
        Task<PersonModel> GetPersonInfo(int requestFacilityId, CancellationToken cancellationToken);
        Task<PersonCompleteInfoModel> GetCompletePersonInfo(int requestFacilityId, CancellationToken cancellationToken);
        Task<RequestFacilityInfoContractModel> GetRequestFacilityInfoForContract(int requestFacilityId, CancellationToken cancellationToken);
        Task<int?> GetRequestFacilityId(string nationalCode, int orgaizationId, List<RoleEnum> roles, WorkFlowFormEnum workFlowFormEnum, CancellationToken cancellationToken);
        Task<List<PersonCompleteInfoModel>> GetPersonListWaitngInSpecialStep(PagingFilterDto filter, int organizationId, List<RoleEnum> roles, CancellationToken cancellationToken);
        Task<List<SelectListItem>> SelectApprovalFacility(Guid userId, CancellationToken cancellationToken);
        Task<RequestFacilityFilterModel> PrepareFilterModelForSearch(List<RoleEnum> roles, CancellationToken cancellationToken);
    }
}