using Asp.Versioning;
using Common;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{

    [ApiVersion("1")]
    public class DastineController : BaseApiController
    {
        private readonly IDastineService dastineService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DastineController(IDastineService dastineService, IRequestFacilityService requestFacilityService, IWebHostEnvironment webHostEnvironment)
        {
            this.dastineService = dastineService;
            this.requestFacilityService = requestFacilityService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing)]
        public virtual async Task<string> PDFDigestForMultiSign(PDFDigestForMultiSignInputModel model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId, User.Identity!.GetUserLeasingId(),
                new List<RoleEnum> { RoleEnum.AdminBankLeasing }, WorkFlowFormEnum.AdminBankLeasingSignature, cancellationToken);
            if (status)
            {
                var signedContractFileName = await requestFacilityService.GetSignedContractByUserFileName(model.RequestFacilityId, cancellationToken);
                string filePath = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{signedContractFileName}");
                if (System.IO.File.Exists(filePath))
                {
                    byte[] pdfBytes = System.IO.File.ReadAllBytes(filePath);
                    string pdfBase64 = Convert.ToBase64String(pdfBytes);
                    var digest = await dastineService.PDFDigestForMultiSign(pdfBase64, model.SelectedCertificate, User.Identity!.GetFullName(), userId, cancellationToken);

                    return digest;
                }
                else
                    throw new AppException("فایل قرارداد یافت نشد!");
            }
            else
                throw new AppException("شناسه تسهیلات نادرست است یا تسهیلات فوق در مرحله امضاء و تایید مدیر بانک قرار ندارد!");
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing)]
        public virtual async Task<string> JustPDFDigestForMultiSign(PDFDigestForMultiSignInputModel model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            var signedContractFileName = await requestFacilityService.GetSignedContractByUserFileName(model.RequestFacilityId, cancellationToken);
            string filePath = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{signedContractFileName}");
            if (System.IO.File.Exists(filePath))
            {
                byte[] pdfBytes = System.IO.File.ReadAllBytes(filePath);
                string pdfBase64 = Convert.ToBase64String(pdfBytes);
                var digest = await dastineService.PDFDigestForMultiSign(pdfBase64, model.SelectedCertificate, User.Identity!.GetFullName(), userId, cancellationToken);

                return digest;
            }
            else
                throw new AppException("فایل قرارداد یافت نشد!");
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing)]
        public virtual async Task<string> PutPDFSignatureForMultiSign(PutPDFSignatureForMultiSignInputModel model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            var leasingId = User.Identity!.GetUserLeasingId();
            //var status = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId, 
            //                                                                                          leasingId, 
            //                                                                                          new List<RoleEnum> { RoleEnum.AdminBankLeasing }, 
            //                                                                                          WorkFlowFormEnum.AdminBankLeasingSignature, 
            //                                                                                          cancellationToken);
            //if (status)
            //{
                var signedContractFileName = await requestFacilityService.GetSignedContractByUserFileName(model.RequestFacilityId, cancellationToken);
                string filePath = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{signedContractFileName}");
                if (System.IO.File.Exists(filePath))
                {
                    byte[] pdfBytes = System.IO.File.ReadAllBytes(filePath);
                    string pdfBase64 = Convert.ToBase64String(pdfBytes);
                    var signedPdf = await dastineService.PutPDFSignatureForMultiSign(pdfBase64, model.SelectedCertificate, model.Signature, User.Identity!.GetFullName(), userId, cancellationToken);
                    var pdfArray = Convert.FromBase64String(signedPdf);
                    var fileName = await requestFacilityService
                        .SaveSignedContractByBankAndMoveOnWorkFlow(model.RequestFacilityId,
                                                                   leasingId,
                                                                   userId,
                                                                   pdfArray,
                                                                   model.Digest,
                                                                   model.SelectedCertificate,
                                                                   model.Signature,
                                                                   cancellationToken);
                    return fileName;
                }
                else
                    throw new AppException("فایل قرارداد یافت نشد!");
            //}
            //else
            //    throw new AppException("شماره تسهیلات نادرست می باشد یا تسهیلات فوق در این مرحله نمی باشد!");
        }

        //[HttpPost("[action]")]
        //[CustomAuthorize(RoleEnum.AdminBankLeasing)]
        //public virtual async Task<string> PutPDFSignatureForMultiSign(PutPDFSignatureForMultiSignInputModel model, CancellationToken cancellationToken)
        //{
        //    var userId = new Guid(User.Identity.GetUserId());
        //    var signedContractFileName = await requestFacilityService.GetSignedContractByUserFileName(model.RequestFacilityId, cancellationToken);
        //    string filePath = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{signedContractFileName}");
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        byte[] pdfBytes = System.IO.File.ReadAllBytes(filePath);
        //        string pdfBase64 = Convert.ToBase64String(pdfBytes);
        //        var signedPdf = await dastineService.PutPDFSignatureForMultiSign(pdfBase64, model.SelectedCertificate, model.Signature, User.Identity.GetFullName(), userId, cancellationToken);
        //        var pdfArray = Convert.FromBase64String(signedPdf);
        //        var fileName = await requestFacilityService
        //            .SaveSignedContractByBankAndMoveOnWorkFlow(model.RequestFacilityId, userId, pdfArray, model.Digest, model.SelectedCertificate, model.Signature, false, cancellationToken);
        //        return fileName;
        //    }
        //    else
        //        throw new AppException("فایل قرارداد یافت نشد!");
        //}
    }
}