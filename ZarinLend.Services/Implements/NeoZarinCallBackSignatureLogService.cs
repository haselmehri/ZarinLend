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
    public class NeoZarinCallBackSignatureLogService : INeoZarinCallBackSignatureLogService, IScopedDependency
    {
        private readonly ILogger<NeoZarinCallBackSignatureLogService> logger;
        private readonly IBaseRepository<NeoZarinCallBackSignatureLog> neoZarinCallBackSignatureLogRepository;
        private readonly IPersonRepository personRepository;

        public NeoZarinCallBackSignatureLogService(ILogger<NeoZarinCallBackSignatureLogService> logger, IBaseRepository<NeoZarinCallBackSignatureLog> neoZarinCallBackSignatureLogRepository)
        {
            this.logger = logger;
            this.neoZarinCallBackSignatureLogRepository = neoZarinCallBackSignatureLogRepository;
        }

        public async Task AddLog(NeoZarinCallBackSignatureLogModel model, CancellationToken cancellationToken)
        {
            await neoZarinCallBackSignatureLogRepository.AddAsync(new NeoZarinCallBackSignatureLog()
            {
                FileUrl = model.FileUrl,
                RequestFacilityId = model.RequestFacilityId,
                ReceiveTrackId = model.ReceiveTrackId,
                SendTrackId = model.SendTrackId,
            }, cancellationToken);
        }
    }
}
