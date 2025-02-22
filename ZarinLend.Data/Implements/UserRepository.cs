using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository, IScopedDependency
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Task<User> GetByUserAndPass(string username, string password, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(password);
            return Table.SingleOrDefaultAsync(p => p.UserName == username && p.PasswordHash == passwordHash, cancellationToken);
        }

        public Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            //user.SecurityStamp = Guid.NewGuid();
            return UpdateAsync(user, cancellationToken);
        }
        public Task UpdateLastLoginDateAsync(Guid userId, CancellationToken cancellationToken)
        {
            return TableNoTracking.Where(p => p.Id == userId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.LastLoginDate, DateTime.Now), cancellationToken);
            return UpdateCustomPropertiesAsync(new User
            {
                Id = userId,
                LastLoginDate = DateTime.Now
            }, cancellationToken, true, nameof(User.LastLoginDate));
        }

        //we can factor 'async' & 'await' 

        //public override void Update(User entity, bool saveNow = true)
        //{
        //    entity.SecurityStamp = Guid.NewGuid();
        //    base.Update(entity, saveNow);
        //}

        //public override Task UpdateAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
        //{
        //    entity.SecurityStamp = Guid.NewGuid();
        //    return base.UpdateAsync(entity, cancellationToken, saveNow);
        //}

        //public override void UpdateRange(IEnumerable<User> entities, bool saveNow = true)
        //{
        //    foreach (var user in entities)            
        //        user.SecurityStamp = Guid.NewGuid();

        //    base.UpdateRange(entities, saveNow);
        //}
        public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
        {
            //todo در لایه سرویس باید انجام شود
            var exists = await TableNoTracking.AnyAsync(p => p.UserName == user.UserName);
            if (exists)
                throw new BadRequestException("نام کاربری تکراری است");

            var passwordHash = SecurityHelper.GetSha256Hash(password);
            user.PasswordHash = passwordHash;
            await base.AddAsync(user, cancellationToken);
        }

        public async Task<bool> DeleteUserByNationalCode(string nationalCode, CancellationToken cancellationToken)
        {
            FormattableString query = @$"Declare @NationalCode as Nvarchar(15) ={nationalCode}
                                         delete from TransactionReasons where Id in (select Id from Transactions where UserId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from TransactionReasons Where PaymentId in (select Id from Payments where userid in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from Transactions where UserId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from Transactions where CreatorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from PaymentInfos where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete paymentreasons where Id in (select id from Payments where userid in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from Payments where userid in (Select Id from AspNetUsers where UserName=@NationalCode) 
                                         delete from FinotechLogs where OpratorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from NeginHubLogs where OpratorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from FinotechLogs where OpratorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from AyandehSignRequestSignatureLogs where CreatorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from UserNotifications where UserVapidId in (Select Id from UserVAPIDS where Userid in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from UserVAPIDS where Userid in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from RequestFacilityPromissories where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from RequestFacilityInstallments where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from NeoZarinRequestSignatureLogs where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from FinotechLogs where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from RequestFacilityErrors where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from RequestFacilityWorkFlowSteps where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from IranCreditScoringDocuments where IranCreditScoringId in (Select Id from IranCreditScorings where RequestFacilityId in 
                                         ( Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode)))
                                         delete from IranCreditScoringDocuments where IranCreditScoringId in (Select Id  from IranCreditScorings where CreatorId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from IranCreditScorings where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from UserIdentityDocuments where UserId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from IranCreditScorings where CreatorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from RequestFacilityGuarantors where GuarantorUserId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from WalletTransactions where CreatorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from VerifyResultExcels where CreatorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from IranCreditScoringResultRules where CreatorId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from VerifyResultExcelDetails where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in  (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from ApplicantValidationResults where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in  (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from RequestFacilityInsuranceIssuances where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in  (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from RequestFacilityCardIssuances where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in  (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from RequestFacilityWarranties where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in  (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from AyandehSignRequestSignatureLogs where RequestFacilityId in (Select Id from RequestFacilities where BuyerId in  (Select Id from AspNetUsers where UserName=@NationalCode))
                                         delete from RequestFacilities where BuyerId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from UserBankAccounts where UserId in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from PersonJobInfos where PersonId in (Select Id from People where NationalCode=@NationalCode)
                                         delete from AspNetUserRoles where Userid in (Select Id from AspNetUsers where UserName=@NationalCode)
                                         delete from AspNetUsers where UserName=@NationalCode
                                         delete from People where Nationalcode = @NationalCode";

            var result = await DbContext.Database.ExecuteSqlAsync(query, cancellationToken);
            return result > 0;
        }

    }
}
