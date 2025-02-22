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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Common.Enums;

namespace Services
{
    public class RequestFacilityGuarantorWarrantyService : IRequestFacilityGuarantorWarrantyService, IScopedDependency
    {
        private readonly ILogger<RequestFacilityGuarantorWarrantyService> logger;
        private readonly IBaseRepository<RequestFacilityGuarantor> requestFacilityGuarantorRepository;
        private readonly IBaseRepository<RequestFacilityGuarantorWarranty> requestFacilityGuarantorWarrantyRepository;
        private readonly IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public RequestFacilityGuarantorWarrantyService(ILogger<RequestFacilityGuarantorWarrantyService> logger,
                                                      IBaseRepository<RequestFacilityGuarantor> requestFacilityGuarantorRepository,
                                                      IBaseRepository<RequestFacilityGuarantorWarranty> requestFacilityGuarantorWarrantyRepository,
                                                      IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository,
                                                      IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.requestFacilityGuarantorRepository = requestFacilityGuarantorRepository;
            this.requestFacilityGuarantorWarrantyRepository = requestFacilityGuarantorWarrantyRepository;
            this.requestFacilityGuarantorWorkFlowStepRepository = requestFacilityGuarantorWorkFlowStepRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<RequestFacilityGuarantorWarrantyModel> GetWarantyDocument(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            return (await requestFacilityGuarantorWarrantyRepository.SelectByAsync(p => p.RequestFacilityGuarantorId.Equals(requestFacilityGuarantorId) &&
             p.RequestFacilityGuarantor.GuarantorUserId.Equals(userId) &&
             !p.IsDeleted,
               p => new RequestFacilityGuarantorWarrantyModel()
               {
                   WarantyAmount = p.WarantyAmount,
                   WarantyDate = p.WarantyDate,
                   DocumentType = p.DocumentType,
                   DocumentNumber = p.DocumentNumber,
                   ChequeDocument = new DocumentModel()
                   {
                       FilePath = p.FilePath,
                       Version = p.Version,
                       FileType = p.FileType,
                       DocumentType = p.DocumentType
                   }
               }, cancellationToken))
               .FirstOrDefault();
        }
        public async Task<RequestFacilityGuarantorWarrantyModel> GetRequestFacilityGuarantorWarranty(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            RequestFacilityGuarantorWarrantyModel model;
            model = (await requestFacilityGuarantorWarrantyRepository.SelectByAsync(p => p.RequestFacilityGuarantorId.Equals(requestFacilityGuarantorId) &&
                p.RequestFacilityGuarantor.GuarantorUserId.Equals(userId) &&
                !p.IsDeleted,
                p => new RequestFacilityGuarantorWarrantyModel()
                {
                    Id = p.Id,
                    RequestFacilityGuarantorId = requestFacilityGuarantorId,
                    UserId = userId,
                    WarantyAmount = p.WarantyAmount,
                    WarantyDate = p.WarantyDate,
                    DocumentNumber = p.DocumentNumber,
                    //LeasingTitle = p.RequestFacilityGuarantor.RequestFacility.Organization.Name,
                    //LeasingNationalId = p.RequestFacilityGuarantor.RequestFacility.Organization.NationalId,
                    CreatedDate = p.CreatedDate,
                    DocumentType = p.DocumentType,
                    ChequeDocument = new DocumentModel()
                    {
                        FilePath = p.FilePath,
                        Version = p.Version,
                        FileType = p.FileType,
                        DocumentType = p.DocumentType
                    }

                }, cancellationToken))
                .FirstOrDefault();

            if (model == null)
            {
                var baseWarantyData = (await requestFacilityGuarantorRepository.SelectByAsync(p => p.Id.Equals(requestFacilityGuarantorId) &&
                    p.GuarantorUserId.Equals(userId),
                    p => new
                    {
                        p.RequestFacility.Amount,
                        p.RequestFacility.FacilityType.MonthCount,
                        p.RequestFacility.GlobalSetting.WarantyPercentage,
                        p.RequestFacility.GlobalSetting.FacilityInterest
                    }, cancellationToken))
                    .FirstOrDefault();

                model = new RequestFacilityGuarantorWarrantyModel()
                {
                    RequestFacilityGuarantorId = requestFacilityGuarantorId,
                    UserId = userId,
                    WarantyAmount = Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(baseWarantyData.Amount, baseWarantyData.MonthCount, baseWarantyData.FacilityInterest, baseWarantyData.WarantyPercentage))),
                    WarantyDate = DateTime.Now.AddMonths(6),
                    DocumentType = DocumentType.Cheque,
                    ChequeDocument = null,
                };
            }

            return model;
        }
        public async Task UploadWaranties(RequestFacilityGuarantorWarrantyModel model, CancellationToken cancellationToken = default)
        {
            if (requestFacilityGuarantorWarrantyRepository.TableNoTracking.Any(p => p.DocumentNumber == model.DocumentNumber))
                throw new AppException("شناسه صیادی چک تکراری است!");

            var facilityInfo = await requestFacilityGuarantorRepository.TableNoTracking
                .Where(p => p.Id == model.RequestFacilityGuarantorId)
                .Select(p => new
                {
                    p.RequestFacility.Amount,
                    p.RequestFacility.FacilityType.MonthCount,
                    p.RequestFacility.GlobalSetting.WarantyPercentage,
                    p.RequestFacility.GlobalSetting.FacilityInterest
                })
                .FirstOrDefaultAsync();
            //TODO:check this RequestFacilityId is waiting for 'RegisterAndUploadCheck' Step
            var userRequestFacilityWarranty = @"UploadFiles\RequestFacilityGuarantorWarranty";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userRequestFacilityWarranty);

            #region Save Cheque File On Disk & Insert/Update Record In Database

            var chequeDocument = await requestFacilityGuarantorWarrantyRepository
                .GetByConditionAsync(p => p.RequestFacilityGuarantorId.Equals(model.RequestFacilityGuarantorId) &&
                    p.RequestFacilityGuarantor.GuarantorUserId.Equals(model.UserId) &&
                    p.DocumentType == DocumentType.Cheque &&
                    !p.IsDeleted, cancellationToken);
            if (chequeDocument != null) //Edit Mode
            {
                var fileName = Path.GetFileName(chequeDocument.FilePath);
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ChequeFile.CopyToAsync(fileStream);

                    chequeDocument.DocumentNumber = model.DocumentNumber;
                    chequeDocument.Version += 1;
                    await requestFacilityGuarantorWarrantyRepository
                        .UpdateCustomPropertiesAsync(chequeDocument, cancellationToken, saveNow: false,
                        nameof(chequeDocument.Version),
                        nameof(chequeDocument.DocumentNumber));
                }
            }
            else
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ChequeFile.FileName)}";
                var relativePath = $"/UploadFiles/RequestFacilityGuarantorWarranty/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ChequeFile.CopyToAsync(fileStream);
                    #region Add File Info To DB
                    await requestFacilityGuarantorWarrantyRepository.AddAsync(new RequestFacilityGuarantorWarranty()
                    {
                        RequestFacilityGuarantorId = model.RequestFacilityGuarantorId,
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Image,
                        DocumentType = DocumentType.Cheque,
                        DocumentNumber = model.DocumentNumber,
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

            await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(model.RequestFacilityGuarantorId,
             WorkFlowFormEnum.RegisterGuaranteesByGuarantor,
             StatusEnum.Approved,
             opratorId: model.UserId,
             "بارگذاری و ثبت چک توسط ضامن",
             cancellationToken);
        }
    }
}
