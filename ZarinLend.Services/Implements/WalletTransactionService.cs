using AutoMapper;
using Common;
using Core.Data.Repositories;
using Core.Entities.Business.Payment;
using Core.Entities.Business.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Model.WalletTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class WalletTransactionService : IWalletTransactionService, IScopedDependency
    {
        private readonly ILogger<WalletTransactionService> logger;
        private readonly IWalletTransactionRepository walletTransactionRepository;
        private readonly ISamanInternetPaymentService samanInternetPaymentService;

        public WalletTransactionService(ILogger<WalletTransactionService> logger,
                                        IWalletTransactionRepository walletTransactionRepository,
                                        ISamanInternetPaymentService samanInternetPaymentService)
        {
            this.logger = logger;
            this.walletTransactionRepository = walletTransactionRepository;
            this.samanInternetPaymentService = samanInternetPaymentService;
        }

        public async Task<long> GetBalance(Guid userId, CancellationToken cancellationToken)
        {
            return await walletTransactionRepository.GetBalance(userId, cancellationToken);
        }

        public async Task<WalletBalanceBaseCardNumber> GetWalletBalanceBaseCard(string cardNumber, Guid userId, CancellationToken cancellationToken)
        {
            var walletBalanceBaseCardNumber = await walletTransactionRepository.TableNoTracking
                                .Where(p => p.UserId == userId &&
                                            p.RequestFacilityId.HasValue &&
                                            p.RequestFacility.RequestFacilityCardIssuance.CardNumber == cardNumber &&
                                            p.WalletTransactionType == WalletTransactionTypeEnum.Deposit &&
                                            p.RequestFacility.BuyerId.Equals(userId))
                                .Select(p => new WalletBalanceBaseCardNumber
                                {
                                    CardNumber = p.RequestFacility.RequestFacilityCardIssuance.CardNumber,
                                    Deposit = p.Amount,
                                    RequestFacilityId = p.RequestFacilityId.Value
                                })
                                .FirstOrDefaultAsync(cancellationToken);

            walletBalanceBaseCardNumber.Withdraw = await samanInternetPaymentService.GetTotalWithdrawBaseCard(cardNumber, userId, cancellationToken);
            return walletBalanceBaseCardNumber;
        }

        public async Task<List<WalletBalanceBaseCardNumber>> GetWalletBalanceBaseCards(Guid userId, CancellationToken cancellationToken)
        {
            var result = await walletTransactionRepository.TableNoTracking
                                .Where(p => p.UserId == userId &&
                                            p.RequestFacilityId.HasValue &&
                                            p.WalletTransactionType == WalletTransactionTypeEnum.Deposit &&
                                            p.RequestFacility.BuyerId.Equals(userId))
                                .Select(p => new
                                {
                                    p.RequestFacility.RequestFacilityCardIssuance.CardNumber,
                                    Deposit = p.Amount,
                                    RequestFacilityId = p.RequestFacilityId.Value
                                })
                                .ToListAsync(cancellationToken);
            return result.Select(p => new WalletBalanceBaseCardNumber()
            {
                CardNumber = p.CardNumber,
                Deposit = p.Deposit,
                RequestFacilityId = p.RequestFacilityId,
                Withdraw = samanInternetPaymentService.GetTotalWithdrawBaseCard(p.CardNumber, userId, cancellationToken).GetAwaiter().GetResult()
            }).ToList();
        }

        //public virtual async Task TransferCreditAmountFromBuyerToSeller(SendSmsVerificationModel model, CancellationToken cancellationToken)
        //{
        //    var amount = Convert.ToInt64(model.AmountThousandSeparator.Replace(",", string.Empty));
        //    var smsValidation = await smsVerificationRepository.TableNoTracking
        //        .FirstOrDefaultAsync(p => p.SenderId.Equals(model.SellerId) &&
        //                       p.ReceptorId.Equals(model.BuyerId) &&
        //                       p.VerficationCode.Equals(model.VerificationCode) &&
        //                       !p.IsUsed &&
        //                       p.Amount.Equals(amount) &&
        //                       p.ExpireTime > DateTime.Now,
        //                       cancellationToken);

        //    if (smsValidation == null)
        //        throw new AppException("کد تایید اشتباه است|کد تایید منقضی شده است|کد تایید استفاده شده است|مغایرت مبلغ با مبلغ ارسالی در پیامک تایید");

        //    var buyerBalnace = await GetBalance(model.BuyerId, cancellationToken);
        //    if (amount > buyerBalnace)
        //        throw new AppException("مبلغ بیشتر از اعتبار خریدار می باشد");

        //    #region Withdraw from Buyer & Deposit to Seller
        //    await walletTransactionRepository.AddAsync(new WalletTransaction()
        //    {
        //        Amount = -amount,
        //        CreatorId = model.SellerId,
        //        UserId = model.BuyerId,
        //        Description = $"برداشت از حساب بابت خرید از فروشگاه-کد تایید : {model.VerificationCode} - شناسه فروشگاه : {model.OrganizationId}",
        //        WalletTransactionType = WalletTransactionTypeEnum.Withdraw,
        //        Child = new WalletTransaction()
        //        {
        //            Amount = amount,
        //            CreatorId = model.SellerId,
        //            UserId = model.SellerId,
        //            OrganizationId = model.OrganizationId,
        //            Description = $"برداشت از حساب خریدار-کد تایید : {model.VerificationCode} - شناسه خریدار : {model.BuyerId}",
        //            WalletTransactionType = WalletTransactionTypeEnum.Deposit,
        //        }
        //    }, cancellationToken, false);

        //    await smsVerificationRepository.UpdateCustomPropertiesAsync(new SmsVerification()
        //    {
        //        Id = smsValidation.Id,
        //        IsUsed = true,
        //    }, cancellationToken, true, nameof(SmsVerification.IsUsed), nameof(SmsVerification.UpdateDate));
        //    #endregion
        //}
    }
}
