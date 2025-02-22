using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Services.Model
{
    [Serializable]
    public class UserIdentityDocumentModel
    {
        public Guid UserId { get; set; }
        public int? RequestFacilityId { get; set; }
        public int? RequestFacilityGuarantorId { get; set; }

        /// <summary>
        /// تصویر صفحه اول شناسنامه
        /// </summary>
        public DocumentModel? BirthCertificatePage1 { get; set; }

        /// <summary>
        /// تصویر توضیحات شناسنامه
        /// </summary>
        public DocumentModel? BirthCertificateDescription { get; set; }

        /// <summary>
        /// تصویر روی کارت ملی
        /// </summary>
        public DocumentModel? NationalCardFront { get; set; }

        /// <summary>
        /// تصویر پشت کارت ملی
        /// </summary>
        public DocumentModel? NationalCardBack { get; set; }

        /// <summary>
        /// مدرک شغلی
        /// </summary>
        public List<DocumentModel>? JobDocuments { get; set; }
        public string? JobDocumentsJson
        {
            get
            {
                if (JobDocuments == null) return null;
                return JsonConvert.SerializeObject(JobDocuments.Select(p => new FileStatus
                {
                    FileId = p.Id,
                    Status = FileStatusEnum.None,
                    File = null
                }));
            }
        }

        /// <summary>
        /// اجاره نامه یا سند مالکیت
        /// </summary>
        public List<DocumentModel>? AddressDocuments { get; set; }

        public string? AddressDocumentsJson
        {
            get
            {
                if (AddressDocuments == null) return null;
                return JsonConvert.SerializeObject(AddressDocuments.Select(p => new FileStatus
                {
                    FileId = p.Id,
                    Status = FileStatusEnum.None,
                    File = null
                }));
            }
        }

        /// <summary>
        /// کارت پایان خدمت سربازی
        /// </summary>
        public DocumentModel MilitaryServiceCard { get; set; }

        /// <summary>
        /// فایل صورتحساب/گردش حساب
        /// </summary>
        public DocumentModel? AccountStatement { get; set; }

        [Display(Name = "Bank")]
        public int? BankId { get; set; }

        [Display(Name = "Bank")]
        public IEnumerable<SelectListItem>? Banks { get; set; }

        public bool IsEditMode { get; set; } = false;
    }

    [Serializable]
    public class UploadIdentityDocumentPostModel
    {
        //public int? bankId { get; set; }
        //public required IFormFile BirthCertificatePage1 { get; set; }
        //public required IFormFile BirthCertificateDescription { get; set; }
        public required IFormFile NationalCardFront { get; set; }
        public required IFormFile NationalCardBack { get; set; }
        //public required IFormFile AccountStatement { get; set; }
        //public required List<IFormFile> JobDocument { get; set; }
        //public required List<IFormFile> AddressDocument { get; set; }
        //public List<int>? DeleteFileIds { get; set; }
    }

    [Serializable]
    public enum FileStatusEnum
    {
        None = 0,
        Add = 1,
        Delete = 2
    }

    [Serializable]
    public class FileStatus
    {
        public IFormFile? File { get; set; }
        public int FileId { get; set; }
        public FileStatusEnum Status { get; set; }
    }
}
