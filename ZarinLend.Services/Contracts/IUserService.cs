using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.Ayandeh.BankAccount;

namespace Services
{
    public interface IUserService
    {
        Task<TokenModel> Token(LoginWithUserPassModel model, CancellationToken cancellationToken = default);
        Task<TokenModel> Token(string userName, CancellationToken cancellationToken = default);
        Task SaveVAPID(UserVAPIDModel model, CancellationToken cancellationToken);
        Task SendOtpForLogin(OtpModel model, CancellationToken cancellationToken = default);
        Task<string> SendOtpForRegister(OtpModel model, CancellationToken cancellationToken = default);
        Task GenerateResetPasswordOtp(ResetPasswordModel model, CancellationToken cancellationToken = default);
        Task<TokenModeWithUser> GenerateTokenWithMobile(LoginWithOtpModel model, CancellationToken cancellationToken = default);
        Task<List<SelectListItem>> GetOrganizationUsers(int organizationId, CancellationToken cancellationToken);
        Task<string> GetMobile(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> EditUser(UserAddEditModelByAdmin userModel, CancellationToken cancellationToken = default);
        Task<bool> AddUser(UserAddEditModelByAdmin userModel, CancellationToken cancellationToken = default);
        Task<PagingDto<UserListModel>> SelectUsers(PagingFilterDto filter, CancellationToken cancellationToken = default);
        Task<UserSearchResultModel> SearchUser(UserFilterModel filter, CancellationToken cancellationToken = default);
        //Task<string> GeneratePasswordResetToken(ForgotPasswordModel model, CancellationToken cancellationToken = default);
        Task<IdentityResult> ResetPassword(ForgotPasswordModel model, CancellationToken cancellationToken = default);
        Task<List<UserSelectDto>> Get(CancellationToken cancellationToken = default);
        Task<UserEditModel> Get(Guid id, CancellationToken cancellationToken = default);
        Task<UserRegisterByZarinpalResultModel> RegisterByZarinpal(UserRegisterByZarinpalModel userModel, CancellationToken cancellationToken = default);
        Task<User> RegisterBuyer(UserAddModel userModel, CancellationToken cancellationToken = default);
        Task<User> QuickRegister(UserQuickRegisterModel userModel, CancellationToken cancellationToken = default);
        Task<UserEditResult> Update(UserEditModel userModel, CancellationToken cancellationToken = default);
        Task<bool> GoToNextStepFromProfileToUploadDocuments(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default);
        Task<bool> GoToNextStepFromProfileToUploadDocumentsInGuarantor(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default);
        Task<bool> GoToNextStepFromUploadDocumentsToVerifyZarinLend(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default);
        Task<bool> GoToNextStepFromUploadDocumentsToVerifyZarinLendIfPossible(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default);
        Task<bool> GoToNextStepFromUploadDocumentsToVerifyZarinLendInGuarantor(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken = default);
        Task<UserEditResult> UpdateUserInfo(UserInfoEditModel userModel, CancellationToken cancellationToken = default);
        Task<UserEditResult> UpdateUserLocation(UserLocationEditModel userModel, CancellationToken cancellationToken = default);
        //Task<bool> UpdateBankAccount(UserBankAccountEditModel userModel, CancellationToken cancellationToken = default);
        Task<string> GetCustomerNumber(Guid userId, CancellationToken cancellationToken = default);
        Task<BankAccountModel?> AddCardNumber(Guid userId, [NotNull] BankAccountAddModel model, CancellationToken cancellationToken = default);
        Task<UserIdentityDocumentModel?> GetUserIdentityDocument(Guid userId, CancellationToken cancellationToken = default);
        Task UploadIdentityDocuments(Guid userId,int? bankId, IFormFile BirthCertificatePage1, IFormFile BirthCertificateDescription,
            IFormFile NationalCardFront, IFormFile NationalCardBack,IFormFile AccountStatement, List<IFormFile> JobDocument, List<IFormFile> AddressDocument, List<int> deleteFileIDs,
            CancellationToken cancellationToken = default);
        Task UploadIdentityDocuments(Guid userId, IFormFile NationalCardFront, IFormFile NationalCardBack, CancellationToken cancellationToken = default);

        Task UploadIdentityDocumentsGuarantor(Guid userId, int? waitingRequestFacilityGuarantorId, IFormFile BirthCertificatePage1, IFormFile BirthCertificateDescription,
           IFormFile NationalCardFront, IFormFile NationalCardBack, List<IFormFile> JobDocument, List<IFormFile> AddressDocument, List<int> deleteFileIDs,
           CancellationToken cancellationToken = default);
        Task UploadIdentityDocumentsGuarantor(Guid userId, int? waitingRequestFacilityGuarantorId, IFormFile NationalCardFront, IFormFile NationalCardBack, CancellationToken cancellationToken = default);
        Task Update(UserDto userDto, CancellationToken cancellationToken = default);
        Task Delete(Guid id, CancellationToken cancellationToken = default);
        Task<UserSelectDto> UpdateCustomProperties(UserDto userDto, CancellationToken cancellationToken = default);
        Task UpdateFullname(UserDto user, CancellationToken cancellationToken = default);
        Task<UserDto> UpdateProperty(Guid id, PropertyValueDto dto, CancellationToken cancellationToken = default);
        Task<UserDto> AvatarUploader(string userId, IFormFile file, CancellationToken cancellationToken = default);
        Task<UserAddModel> PrepareModelForAdd(CancellationToken cancellationToken = default);
        Task<UserAddEditModelByAdmin> PrepareModelForAddByAdmin(CancellationToken cancellationToken);
        Task<UserEditModel> PrepareModelForEdit(Guid userId, CancellationToken cancellationToken = default);
        Task<UserEditModel> PrepareModelForEditGuarantor(Guid userId, CancellationToken cancellationToken);
        Task<UserAddEditModelByAdmin> PrepareModelForEditByAdmin(Guid userId, CancellationToken cancellationToken);
        Task<UserFilterModel> PrepareFilterModel(CancellationToken cancellationToken);
        Task<bool> DeleteUserByNationalCode(string nationalCode, CancellationToken cancellationToken);
        Task<bool> UpdateBankAccount(UpdateBankAccountModel model, CancellationToken cancellationToken);
    }
}