using Common.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;
using static Common.Enums;

namespace Services.Model
{
    public class RequestFacilityWarrantyModel
    {
        public int Id { get; set; }
        public int RequestFacilityId { get; set; }

        ///// <summary>
        ///// Id value from Organization
        ///// </summary>
        //public int LeasingId { get; set; }

        public string LeasingTitle { get; set; }
        /// <summary>
        /// شناسه ملی لیزینگ قرار است چک در وجه آن صادر شود
        /// </summary>
        public long LeasingNationalId { get; set; }
        public Guid UserId { get; set; }

        public long WarantyAmount { get; set; }

        public IFormFile ChequeFile { get; set; }
        public IFormFile PromissoryNoteFile { get; set; }
        /// <summary>
        /// تصویر چک
        /// </summary>
        public DocumentModel ChequeDocument { get; set; }

        /// <summary>
        /// فایل PDF سفته
        /// </summary>
        public DocumentModel PromissoryNote { get; set; }

        public DocumentType DocumentType { get; set; }
        /// <summary>
        ///   فایل تضمین
        /// </summary>
        public DocumentModel File { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(11, ErrorMessage = "{0} باید {1} رقمی باشد")]
        [MaxLength(11, ErrorMessage = "{0} باید {1} رقمی باشد")]
        [Display(Name = "شماره سفته")]
        [RegularExpression(RegularExpression.SaftehNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string DocumentNumber { get; set; }

        public DateTime CreatedDate { get; set; }
        public string ShamsiCreatedDate
        {
            get
            {
                return CreatedDate.GregorianToShamsi();
            }
        }
        public DateTime WarantyDate { get; set; }
        public string ShamsiWarantyDate
        {
            get
            {
                return WarantyDate.GregorianToShamsi();
            }
        }
    }
}
