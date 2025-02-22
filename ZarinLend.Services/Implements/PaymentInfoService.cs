using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities.Business.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Model;
using Services.Model.Payment;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentInfoService : IPaymentInfoService, IScopedDependency
    {
        private readonly ILogger<PaymentInfoService> logger;
        private readonly IBaseRepository<PaymentInfo> paymentInfoRepository;

        public PaymentInfoService(ILogger<PaymentInfoService> logger, 
                                  IBaseRepository<PaymentInfo> paymentInfoRepository)
        {
            this.logger = logger;
            this.paymentInfoRepository = paymentInfoRepository;
        }

        /// <summary>
        /// 'PaymentInfo' model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>add entity and return primary key of item added</returns>
        public async Task<Guid> Add(PaymentInfoModel model, CancellationToken cancellationToken)
        {
            var entity = new PaymentInfo()
            {
                IsActive = true,
                CreatorId = model.CreatorId,
                SellerId = model.SellerId,
                OrganizationId = model.OrganizationId,
                BuyerId = model.BuyerId,
                BuyerMobile = model.BuyerMobile,
                Otp = model.Otp,
                Amount = model.Amount,
                CardNumber = model.CardNumber,
                ExpireDate = model.ExpireDate,
                IpgType = model.IpgType,
                MessageId = model.MessageId
            };
            await paymentInfoRepository.AddAsync(entity, cancellationToken);

            return entity.Id;
        }

        public async Task<PaymentInfoModel> Get(Guid id,Guid sellerId, CancellationToken cancellationToken)
        {
            var item = await paymentInfoRepository.TableNoTracking
                .Where(p => p.Id == id && p.SellerId == sellerId && p.IsActive && !p.IsUsed)
                .Select(p => new PaymentInfoModel()
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    CardNumber= p.CardNumber,
                    BuyerId = p.BuyerId,
                    SellerId = p.SellerId,
                    IpgType = p.IpgType,
                    BuyerMobile = p.Buyer.Person.Mobile,
                })
                .FirstOrDefaultAsync(cancellationToken);

            return item;
        }

        public async Task<Guid> OtpIsVerify(VerifyOtpForTransferWalletBallanceModel model, CancellationToken cancellationToken)
        {            
            var id = await paymentInfoRepository.TableNoTracking
                .Where(p => p.SellerId.Equals(model.SellerId) &&
                               p.BuyerId.Equals(model.BuyerId) &&
                               p.BuyerMobile.Equals(model.BuyerMobile) &&
                               p.Otp.Equals(model.VerificationCode) &&
                               !p.IsUsed &&
                               p.Amount.Equals(model.Amount) &&
                               p.ExpireDate > DateTime.Now)
                .Select(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (id == Guid.Empty)
                throw new AppException("کد تایید اشتباه است|کد تایید منقضی شده است|کد تایید استفاده شده است|مغایرت مبلغ با مبلغ ارسالی در پیامک تایید");

            return id;
        }
    }
}
