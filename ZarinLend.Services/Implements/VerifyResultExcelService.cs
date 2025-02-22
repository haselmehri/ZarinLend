using Common;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
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
    public class VerifyResultExcelService : IVerifyResultExcelService, IScopedDependency
    {
        private readonly ILogger<VerifyResultExcelService> logger;
        private readonly IBaseRepository<VerifyResultExcel> verifyResultExcelRepository;
        private readonly IBaseRepository<VerifyResultExcelDetail> verifyResultExcelDetailRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public VerifyResultExcelService(ILogger<VerifyResultExcelService> logger,IBaseRepository<VerifyResultExcel> verifyResultExcelRepository, 
            IBaseRepository<VerifyResultExcelDetail> verifyResultExcelDetailRepository,IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.verifyResultExcelRepository = verifyResultExcelRepository;
            this.verifyResultExcelDetailRepository = verifyResultExcelDetailRepository;
            this.webHostEnvironment = webHostEnvironment;
        }
        public async Task SaveVerifyResultFromFile(IFormFile verifyExcelFile, List<ApplicantValidationResultModel> extractResultFromFile, int organizationId,
            Guid creator, CancellationToken cancellationToken = default)
        {
            var basePath = @"UploadFiles\VerifyResultExcel";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, basePath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(verifyExcelFile.FileName)}";
            var relativePath = $"/UploadFiles/VerifyResultExcel/{fileName}";
            string filePath = Path.Combine(uploadFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var approvedCount = extractResultFromFile.Count(p => p.BlackListInquiry.HasValue && p.BlackListInquiry.Value &&
                                                                    p.CivilRegistryInquiry.HasValue && p.CivilRegistryInquiry.Value &&
                                                                    p.FacilityInquiry.HasValue && p.FacilityInquiry.Value &&
                                                                    p.MilitaryInquiry.HasValue && p.MilitaryInquiry.Value &&
                                                                    p.PostalCodeInquiry.HasValue && p.PostalCodeInquiry.Value &&
                                                                    p.ReturnedCheckInquiry.HasValue && p.ReturnedCheckInquiry.Value &&
                                                                    p.SecurityCouncilSanctionsInquiry.HasValue && p.SecurityCouncilSanctionsInquiry.Value &&
                                                                    p.ShahkarInquiry.HasValue && p.ShahkarInquiry.Value);
                var rejectedCount = extractResultFromFile.Count(p => (p.BlackListInquiry.HasValue && !p.BlackListInquiry.Value) ||
                                                                     (p.CivilRegistryInquiry.HasValue && !p.CivilRegistryInquiry.Value) ||
                                                                     (p.FacilityInquiry.HasValue && !p.FacilityInquiry.Value) ||
                                                                     (p.MilitaryInquiry.HasValue && !p.MilitaryInquiry.Value) ||
                                                                     (p.PostalCodeInquiry.HasValue && !p.PostalCodeInquiry.Value) ||
                                                                     (p.ReturnedCheckInquiry.HasValue && !p.ReturnedCheckInquiry.Value) ||
                                                                     (p.SecurityCouncilSanctionsInquiry.HasValue && !p.SecurityCouncilSanctionsInquiry.Value) ||
                                                                     (p.ShahkarInquiry.HasValue && !p.ShahkarInquiry.Value));
                await verifyExcelFile.CopyToAsync(fileStream);
                #region Add Result to DB
                await verifyResultExcelRepository.AddAsync(new VerifyResultExcel()
                {
                    OrganizationId = organizationId,
                    FilePath = relativePath,
                    Status = DocumentStatus.Active,
                    FileType = FileType.Excel,
                    CreatorId = creator,
                    RowCount = extractResultFromFile.Count,
                    ApprovedCount = approvedCount,
                    RejectedCount = rejectedCount,
                    UnknownCount = extractResultFromFile.Count - (approvedCount + rejectedCount),
                    VerifyResultExcelDetails = extractResultFromFile.Select(p => new VerifyResultExcelDetail()
                    {
                        RequestFacilityId = p.RequestFacilityId,
                        BlackListInquiry = p.BlackListInquiry,
                        CivilRegistryInquiry = p.CivilRegistryInquiry,
                        FacilityInquiry = p.FacilityInquiry,
                        MilitaryInquiry = p.MilitaryInquiry,
                        PostalCodeInquiry = p.PostalCodeInquiry,
                        ReturnedCheckInquiry = p.ReturnedCheckInquiry,
                        SecurityCouncilSanctionsInquiry = p.SecurityCouncilSanctionsInquiry,
                        ShahkarInquiry = p.ShahkarInquiry,
                        CreatorId = creator,
                        Description = p.ErrorMessage,
                        FinalResult = p.BlackListInquiry.HasValue && p.BlackListInquiry.Value &&
                                 p.CivilRegistryInquiry.HasValue && p.CivilRegistryInquiry.Value &&
                                 p.FacilityInquiry.HasValue && p.FacilityInquiry.Value &&
                                 p.MilitaryInquiry.HasValue && p.MilitaryInquiry.Value &&
                                 p.PostalCodeInquiry.HasValue && p.PostalCodeInquiry.Value &&
                                 p.ReturnedCheckInquiry.HasValue && p.ReturnedCheckInquiry.Value &&
                                 p.SecurityCouncilSanctionsInquiry.HasValue && p.SecurityCouncilSanctionsInquiry.Value &&
                                 p.ShahkarInquiry.HasValue && p.ShahkarInquiry.Value
                                 ? true
                                 : ((p.BlackListInquiry.HasValue && !p.BlackListInquiry.Value) ||
                                    (p.CivilRegistryInquiry.HasValue && !p.CivilRegistryInquiry.Value) ||
                                    (p.FacilityInquiry.HasValue && !p.FacilityInquiry.Value) ||
                                    (p.MilitaryInquiry.HasValue && !p.MilitaryInquiry.Value) ||
                                    (p.PostalCodeInquiry.HasValue && !p.PostalCodeInquiry.Value) ||
                                    (p.ReturnedCheckInquiry.HasValue && !p.ReturnedCheckInquiry.Value) ||
                                    (p.SecurityCouncilSanctionsInquiry.HasValue && !p.SecurityCouncilSanctionsInquiry.Value) ||
                                    (p.ShahkarInquiry.HasValue && !p.ShahkarInquiry.Value))
                                    ? false
                                    : (bool?)null
                    }).ToList()
                }, cancellationToken);
                #endregion
            }
        }

        public async Task<PagingDto<VerifyResultExcelModel>> GetVerifyResultExcelHistory(int leasingId, PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var result =await verifyResultExcelRepository.SelectByAsync(p => p.OrganizationId == leasingId,
                p => new VerifyResultExcelModel()
                {
                    Id = p.Id,
                    RowCount = p.RowCount,
                    ApprovedCount = p.ApprovedCount,
                    RejectedCount = p.RejectedCount,
                    UnknownCount = p.UnknownCount,
                    ExcelFilePath = p.FilePath,
                    CreatedDate = p.CreatedDate,
                    Creator = $"{p.Creator.Person.FName} {p.Creator.Person.LName}",
                }, cancellationToken, false, filter.Page, filter.PageSize);

            var totalRowCounts = await verifyResultExcelRepository.TableNoTracking.CountAsync(p => p.OrganizationId == leasingId);

            var pagingResult = new PagingDto<VerifyResultExcelModel>()
            {
                CurrentPage = filter.Page,
                Data = result,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return pagingResult;
        }

        public async Task<List<VerifyResultExcelDetailModel>> GetVerifyResultExcelDetail(int leasingId, int verifyResultExcelId, CancellationToken cancellationToken)
        {
            var result = await verifyResultExcelDetailRepository.SelectByAsync(p => p.VerifyResultExcel.OrganizationId == leasingId && p.VerifyResultExcelId == verifyResultExcelId,
                p => new VerifyResultExcelDetailModel()
                {
                    BlackListInquiry = p.BlackListInquiry,
                    CivilRegistryInquiry = p.CivilRegistryInquiry,
                    FacilityInquiry = p.FacilityInquiry,
                    MilitaryInquiry = p.MilitaryInquiry,
                    PostalCodeInquiry = p.PostalCodeInquiry,
                    ReturnedCheckInquiry = p.ReturnedCheckInquiry,
                    SecurityCouncilSanctionsInquiry = p.SecurityCouncilSanctionsInquiry,
                    ShahkarInquiry = p.ShahkarInquiry,
                    FinalResult =p.FinalResult,
                    RequesterFullName = $"{p.RequestFacility.Buyer.Person.FName} {p.RequestFacility.Buyer.Person.LName}",
                    Description = p.Description,
                }, cancellationToken, false);

            return result;
        }
    }
}
