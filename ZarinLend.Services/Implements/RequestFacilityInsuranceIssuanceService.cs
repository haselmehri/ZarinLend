using Common;
using Common.Exceptions;
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
using static Common.Enums;

namespace Services
{
    public class RequestFacilityInsuranceIssuanceService : IRequestFacilityInsuranceIssuanceService, IScopedDependency
    {
        private readonly ILogger<RequestFacilityInsuranceIssuanceService> logger;
        private readonly IRequestFacilityRepository requestFacilityRepository;
        private readonly IBaseRepository<RequestFacilityInsuranceIssuance> requestFacilityInsuranceIssuanceRepository;
        private readonly IBaseRepository<RequestFacilityInsuranceIssuanceDocument> requestFacilityInsuranceIssuanceDocumentRepository;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public RequestFacilityInsuranceIssuanceService(ILogger<RequestFacilityInsuranceIssuanceService> logger,
            IRequestFacilityRepository requestFacilityRepository,
            IBaseRepository<RequestFacilityInsuranceIssuance> requestFacilityInsuranceIssuanceRepository,
            IBaseRepository<RequestFacilityInsuranceIssuanceDocument> requestFacilityInsuranceIssuanceDocumentRepository,
        IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.requestFacilityRepository = requestFacilityRepository;
            this.requestFacilityInsuranceIssuanceRepository = requestFacilityInsuranceIssuanceRepository;
            this.requestFacilityInsuranceIssuanceDocumentRepository = requestFacilityInsuranceIssuanceDocumentRepository;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        //public async Task<DocumentModel> GetRequestFacilityWarantyDocument(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        //{
        //    return (await requestFacilityWarrantyRepository.SelectByAsync(p => p.RequestFacilityId.Equals(requestFacilityId) &&
        //     p.RequestFacility.BuyerId.Equals(userId) &&
        //     p.DocumentType == DocumentType.Cheque &&
        //     !p.IsDeleted,
        //       p => new DocumentModel()
        //       {
        //           Id = p.Id,
        //           CreatedDate = p.CreatedDate,
        //           FilePath = p.FilePath,
        //           Version = p.Version,
        //           FileType = p.FileType
        //       }, cancellationToken))
        //       .FirstOrDefault();
        //}
        //public async Task<RequestFacilityWarrantyModel> GetRequestFacilityWaranty(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        //{
        //    RequestFacilityWarrantyModel model;
        //    model = (await requestFacilityWarrantyRepository.SelectByAsync(p => p.RequestFacilityId.Equals(requestFacilityId) &&
        //        p.RequestFacility.BuyerId.Equals(userId) &&
        //        p.DocumentType == DocumentType.Cheque &&
        //        !p.IsDeleted,
        //        p => new RequestFacilityWarrantyModel()
        //        {
        //            Id = p.Id,
        //            RequestFacilityId = requestFacilityId,
        //            UserId = userId,
        //            ChequeAmount = 
        //            Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(p.RequestFacility.Amount, p.RequestFacility.FacilityType.MonthCount))),
        //            LeasingTitle = p.RequestFacility.Organization.Name,
        //            LeasingNationalId = p.RequestFacility.Organization.NationalId,
        //            CreatedDate = p.CreatedDate,
        //            ChequeDocument = new DocumentModel()
        //            {
        //                FilePath = p.FilePath,
        //                Version = p.Version,
        //                FileType = p.FileType
        //            }

        //        }, cancellationToken))
        //        .FirstOrDefault();

        //    if (model == null)
        //    {
        //        model = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) &&
        //            p.BuyerId.Equals(userId),
        //            p => new RequestFacilityWarrantyModel()
        //            {
        //                RequestFacilityId = p.Id,
        //                UserId = p.BuyerId,
        //                ChequeAmount =
        //                Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(p.Amount, p.FacilityType.MonthCount))),
        //                LeasingTitle = p.Organization.Name,
        //                LeasingNationalId = p.Organization.NationalId
        //            }, cancellationToken))
        //            .FirstOrDefault();
        //    }

        //    return model;
        //}

        public async Task InsuranceIssuance(RequestFacilityInsuranceIssuanceModel model, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                logger.LogError("'model' is null");
            }
            if (await requestFacilityInsuranceIssuanceRepository.TableNoTracking.AnyAsync(p => p.InsuranceNumber.Equals(model.InsuranceNumber.Trim())))
                throw new AppException("شماره بیمه وارد شده تکراری می باشد!");

            #region Save File On Disk & Insert Record In Database
            RequestFacilityInsuranceIssuanceDocument document = null;
            if (file != null && file.Length > 0)
            {
                var userRequestFacilityWarranty = @"UploadFiles\RequestFacilityInsuranceIssuance";
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userRequestFacilityWarranty);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var relativePath = $"/UploadFiles/RequestFacilityInsuranceIssuance/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    #region Add File Info To DB
                    document = new RequestFacilityInsuranceIssuanceDocument()
                    {
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Image,
                        DocumentType = DocumentType.InsuranceIssuance
                    };
                    #endregion
                }
            }
            var requestFacilityInsuranceIssuance = new RequestFacilityInsuranceIssuance()
            {
                CreatorId = model.CreatorId,
                RequestFacilityId = model.RequestFacilityId,
                Description = model.Description,
                InsuranceNumber = model.InsuranceNumber,
                RequestFacilityInsuranceIssuanceDocuments = document != null ? new List<RequestFacilityInsuranceIssuanceDocument> { document } : null,
            };
            await requestFacilityInsuranceIssuanceRepository.AddAsync(requestFacilityInsuranceIssuance, cancellationToken);

            #endregion

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
              WorkFlowFormEnum.InsuranceIssuance,
              StatusEnum.Approved,
              opratorId: model.CreatorId,
              "صدور بیمه و اطلاع رسانی به مشتری",
              cancellationToken);

        }
    }
}
