using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Common.Enums;

namespace Services
{
    public class RequestFacilityWarrantyService : IRequestFacilityWarrantyService, IScopedDependency
    {
        private readonly ILogger<RequestFacilityWarrantyService> logger;
        private readonly IRequestFacilityRepository requestFacilityRepository;
        private readonly IBaseRepository<RequestFacilityWarranty> requestFacilityWarrantyRepository;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public RequestFacilityWarrantyService(ILogger<RequestFacilityWarrantyService> logger, IRequestFacilityRepository requestFacilityRepository,
            IBaseRepository<RequestFacilityWarranty> requestFacilityWarrantyRepository, IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.requestFacilityRepository = requestFacilityRepository;
            this.requestFacilityWarrantyRepository = requestFacilityWarrantyRepository;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<DocumentModel>> GetRequestFacilityWarantyDocument(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            return await requestFacilityWarrantyRepository.SelectByAsync(p => p.RequestFacilityId.Equals(requestFacilityId) &&
             p.RequestFacility.BuyerId.Equals(userId) &&
             !p.IsDeleted,
               p => new DocumentModel()
               {
                   Id = p.Id,
                   CreatedDate = p.CreatedDate,
                   FilePath = p.FilePath,
                   Version = p.Version,
                   FileType = p.FileType,
                   DocumentType = p.DocumentType,
                   DocumentNumber = p.DocumentNumber,
               }, cancellationToken);
        }
        public async Task<List<RequestFacilityWarrantyModel>> GetRequestFacilityWaranty(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            List<RequestFacilityWarrantyModel> model;
            model = await requestFacilityWarrantyRepository.SelectByAsync(p => p.RequestFacilityId.Equals(requestFacilityId) &&
                p.RequestFacility.BuyerId.Equals(userId) &&
                !p.IsDeleted,
                p => new RequestFacilityWarrantyModel()
                {
                    Id = p.Id,
                    RequestFacilityId = requestFacilityId,
                    UserId = userId,
                    //WarantyAmount =
                    //Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(p.RequestFacility.Amount, p.RequestFacility.FacilityType.MonthCount))),
                    WarantyAmount = p.WarantyAmount,
                    WarantyDate = p.WarantyDate,
                    LeasingTitle = p.RequestFacility.Organization.Name,
                    LeasingNationalId = p.RequestFacility.Organization.NationalId,
                    CreatedDate = p.CreatedDate,
                    DocumentType = p.DocumentType,
                    File = new DocumentModel()
                    {
                        FilePath = p.FilePath,
                        Version = p.Version,
                        FileType = p.FileType,
                        DocumentType = p.DocumentType
                    }

                }, cancellationToken);

            if (model == null || !model.Any())
            {
                var baseWarantyData = (await requestFacilityRepository.SelectByAsync(p => p.Id.Equals(requestFacilityId) &&
                    p.BuyerId.Equals(userId),
                    p => new
                    {
                        RequestFacilityId = p.Id,
                        UserId = p.BuyerId,
                        LeasingTitle = p.Organization.Name,
                        LeasingNationalId = p.Organization.NationalId,
                        p.Amount,
                        p.FacilityType.MonthCount,
                        p.GlobalSetting.WarantyPercentage,
                        p.GlobalSetting.FacilityInterest
                    }, cancellationToken))
                    .FirstOrDefault();

                model = new List<RequestFacilityWarrantyModel>()
                {
                    new RequestFacilityWarrantyModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        UserId = baseWarantyData.UserId,
                        LeasingNationalId = baseWarantyData.LeasingNationalId,
                        LeasingTitle = baseWarantyData.LeasingTitle,
                        WarantyAmount = Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(baseWarantyData.Amount, baseWarantyData.MonthCount,baseWarantyData.FacilityInterest,baseWarantyData.WarantyPercentage))),
                        WarantyDate = DateTime.Now.AddMonths(6),
                        DocumentType= DocumentType.Cheque,
                        File = null,
                    },
                    new RequestFacilityWarrantyModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        UserId = baseWarantyData.UserId,
                        LeasingNationalId = baseWarantyData.LeasingNationalId,
                        LeasingTitle = baseWarantyData.LeasingTitle,
                        WarantyAmount = Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(baseWarantyData.Amount, baseWarantyData.MonthCount,baseWarantyData.FacilityInterest,baseWarantyData.WarantyPercentage))),
                        WarantyDate = DateTime.Now.AddMonths(6),
                        DocumentType= DocumentType.PromissoryNoteDocument,
                        File=null
                    },
                };
            }

            return model;
        }
        public async Task UploadWaranties(RequestFacilityWarrantyAddModel model, CancellationToken cancellationToken = default)
        {
            if (requestFacilityWarrantyRepository.TableNoTracking.Any(p => p.DocumentNumber == model.DocumentNumber && p.RequestFacilityId == model.RequestFacilityId))
                throw new AppException("شماره سفته تکراری است!");

            var facilityInfo = await requestFacilityRepository.TableNoTracking
                .Where(p => p.Id == model.RequestFacilityId)
                .Select(p => new
                {
                    //OrganizationName = p.Organization.Name,
                    p.Amount,
                    p.FacilityNumber,
                    p.FacilityType.MonthCount,
                    p.GlobalSetting.WarantyPercentage,
                    p.GlobalSetting.FacilityInterest
                })
                .FirstOrDefaultAsync();
            //TODO:check this RequestFacilityId is waiting for 'RegisterAndUploadCheck' Step
            var userRequestFacilityWarranty = @"UploadFiles\RequestFacilityWarranty";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userRequestFacilityWarranty);

            #region Save Cheque File On Disk & Insert/Update Record In Database

            var chequeDocument = await requestFacilityWarrantyRepository
                .GetByConditionAsync(p => p.RequestFacilityId.Equals(model.RequestFacilityId) &&
                    p.RequestFacility.BuyerId.Equals(model.UserId) &&
                    p.DocumentType == DocumentType.Cheque &&
                    !p.IsDeleted, cancellationToken);
            if (chequeDocument != null) //Edit Mode
            {
                var fileName = Path.GetFileName(chequeDocument.FilePath);
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ChequeFile.CopyToAsync(fileStream);

                    chequeDocument.Version += 1;
                    await requestFacilityWarrantyRepository
                        .UpdateCustomPropertiesAsync(chequeDocument, cancellationToken, saveNow: true,
                        nameof(chequeDocument.Version));
                }
            }
            else
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ChequeFile.FileName)}";
                var relativePath = $"/UploadFiles/RequestFacilityWarranty/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ChequeFile.CopyToAsync(fileStream);
                    #region Add File Info To DB
                    await requestFacilityWarrantyRepository.AddAsync(new RequestFacilityWarranty()
                    {
                        RequestFacilityId = model.RequestFacilityId,
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Image,
                        DocumentType = DocumentType.Cheque,
                        WarantyDate = DateTime.Now.AddMonths(6),
                        WarantyAmount = Convert.ToInt64(Math.Round(InstallmentCalculator
                                .CalculateChequeAmountWarranty(facilityInfo.Amount, 
                                                               facilityInfo.MonthCount, 
                                                               facilityInfo.FacilityInterest, 
                                                               facilityInfo.WarantyPercentage))),
                    }, cancellationToken, saveNow: false);
                    #endregion
                }
            }

            #endregion

            #region Save Cheque File On Disk & Insert/Update Record In Database

            var promissoryNoteDocument = await requestFacilityWarrantyRepository
                .GetByConditionAsync(p => p.RequestFacilityId.Equals(model.RequestFacilityId) &&
                    p.RequestFacility.BuyerId.Equals(model.UserId) &&
                    p.DocumentType == DocumentType.PromissoryNoteDocument &&
                    !p.IsDeleted, cancellationToken);
            if (promissoryNoteDocument != null) //Edit Mode
            {
                var fileName = Path.GetFileName(promissoryNoteDocument.FilePath);
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PromissoryNoteFile.CopyToAsync(fileStream);

                    promissoryNoteDocument.Version += 1;
                    promissoryNoteDocument.DocumentNumber = model.DocumentNumber;
                    await requestFacilityWarrantyRepository
                        .UpdateCustomPropertiesAsync(promissoryNoteDocument, cancellationToken, saveNow: false,
                        nameof(promissoryNoteDocument.Version),
                        nameof(promissoryNoteDocument.DocumentNumber));
                }
            }
            else
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.PromissoryNoteFile.FileName)}";
                var relativePath = $"/UploadFiles/RequestFacilityWarranty/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PromissoryNoteFile.CopyToAsync(fileStream);
                    #region Add File Info To DB
                    await requestFacilityWarrantyRepository.AddAsync(new RequestFacilityWarranty()
                    {
                        RequestFacilityId = model.RequestFacilityId,
                        DocumentNumber = model.DocumentNumber,
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Pdf,
                        DocumentType = DocumentType.PromissoryNoteDocument,
                        WarantyDate = DateTime.Now.AddMonths(6),
                        WarantyAmount = Convert.ToInt64(Math.Round(InstallmentCalculator
                                .CalculateChequeAmountWarranty(facilityInfo.Amount, 
                                                               facilityInfo.MonthCount, 
                                                               facilityInfo.FacilityInterest, 
                                                               facilityInfo.WarantyPercentage))),
                    }, cancellationToken, saveNow: false);
                    #endregion
                }
            }

            #endregion

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityId,
             WorkFlowFormEnum.RegisterGuarantees,
             StatusEnum.Approved,
             opratorId: model.UserId,
             "بارگذاری و ثبت تضامین",
             cancellationToken);
        }
    }
}
