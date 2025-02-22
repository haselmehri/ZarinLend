using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class RequestFacilityCardIssuanceService : IRequestFacilityCardIssuanceService, IScopedDependency
    {
        private readonly ILogger<RequestFacilityCardIssuanceService> logger;
        private readonly IBaseRepository<RequestFacilityCardIssuance> requestFacilityCardIssuanceRepository;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;

        public RequestFacilityCardIssuanceService(ILogger<RequestFacilityCardIssuanceService> logger,
            IBaseRepository<RequestFacilityCardIssuance> requestFacilityCardIssuanceRepository,
        IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository)
        {
            this.logger = logger;
            this.requestFacilityCardIssuanceRepository = requestFacilityCardIssuanceRepository;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
        }

        public async Task CardIssuance(RequestFacilityCardIssuanceModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                logger.LogError("'model' is null");
            }

            if (await requestFacilityCardIssuanceRepository.TableNoTracking.AnyAsync(p => p.CardNumber == model.CardNumber && !p.RequestFacilityId.Equals(model.RequestFacilityId)))
                throw new AppException("شماره بن کارت تکراری است!");

            if (await requestFacilityCardIssuanceRepository.TableNoTracking.AnyAsync(p => p.AccountNumber == model.AccountNumber && !p.RequestFacilityId.Equals(model.RequestFacilityId)))
                throw new AppException("شماره حساب بن کارت تکراری است!");

            var cardIssuance = await requestFacilityCardIssuanceRepository.GetByConditionAsync(p => p.RequestFacilityId.Equals(model.RequestFacilityId), cancellationToken);
            if (cardIssuance == null)
            {
                await requestFacilityCardIssuanceRepository.AddAsync(new RequestFacilityCardIssuance()
                {
                    RequestFacilityId = model.RequestFacilityId,
                    AccountNumber = model.AccountNumber,
                    CardNumber = model.CardNumber,
                    ExpireMonth = model.ExpireMonth,
                    ExpireYear = model.ExpireYear,
                    SecondPassword = model.SecondPassword,
                    Cvv = model.Cvv
                }, cancellationToken);
            }
            else
            {
                cardIssuance.AccountNumber = model.AccountNumber;
                cardIssuance.CardNumber = model.CardNumber;
                cardIssuance.SecondPassword = model.SecondPassword;
                cardIssuance.Cvv = model.Cvv;
                cardIssuance.ExpireYear = model.ExpireYear;
                cardIssuance.ExpireMonth = model.ExpireMonth;
                await requestFacilityCardIssuanceRepository.UpdateAsync(cardIssuance, cancellationToken);
            }

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
              WorkFlowFormEnum.CardIssuance,
              StatusEnum.Approved,
              opratorId: model.CreatorId,
              "صدور بن کارت",
              cancellationToken);

        }

        public async Task EditBonCard(RequestFacilityEditCardIssuanceModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                logger.LogError("'model' is null");
            }

            var cardIssuance = await requestFacilityCardIssuanceRepository.GetByConditionAsync(p => p.Id == model.Id && p.RequestFacilityId.Equals(model!.RequestFacilityId), cancellationToken);
            if (cardIssuance == null)
                throw new AppException("اطلاعات بن کارت یافت نشد!");

            cardIssuance.SecondPassword = model!.SecondPassword;
            cardIssuance.Cvv = model.Cvv;
            cardIssuance.ExpireYear = model.ExpireYear;
            cardIssuance.ExpireMonth = model.ExpireMonth;
            await requestFacilityCardIssuanceRepository.UpdateAsync(cardIssuance, cancellationToken);

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
              WorkFlowFormEnum.CompleteBonCardInfo,
              StatusEnum.Approved,
              opratorId: model.CreatorId,
              "تکمیل اطلاعات بن کارت و استعلام موجودی",
              cancellationToken);
        }

        public async Task<RequestFacilityCardIssuanceModel> GetCardIssuance(int requestFacilityId, CancellationToken cancellationToken = default)
        {
            var cardIssuance = await requestFacilityCardIssuanceRepository.GetByConditionAsync(p => p.RequestFacilityId.Equals(requestFacilityId), cancellationToken);
            Assert.NotEmpty(cardIssuance, "بن کارت");

            long cardNumber;
            return new RequestFacilityCardIssuanceModel()
            {
                AccountNumber = cardIssuance.AccountNumber,
                CardNumber = !string.IsNullOrEmpty(cardIssuance.CardNumber) && cardIssuance.CardNumber.Length == 16 && long.TryParse(cardIssuance.CardNumber, out cardNumber)
                            ? cardNumber.ToString("####-####-####-####")
                            : null,
                CreatedDate = cardIssuance.CreatedDate,
            };
        }
    }
}
