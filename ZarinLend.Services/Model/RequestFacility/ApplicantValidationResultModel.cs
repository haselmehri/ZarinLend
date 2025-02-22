using Common.Utilities;
using System;

namespace Services.Model
{
    public class ApplicantValidationResultModel
    {
        public string? NationalCode { get; set; }
        /// <summary>
        /// استعلام ثبت احوال
        /// </summary>
        public bool? CivilRegistryInquiry { get; set; }

        /// <summary>
        /// استعلام چک برگشتی
        /// </summary>
        public bool? ReturnedCheckInquiry { get; set; }

        /// <summary>
        /// استعلام تسهیلات
        /// </summary>
        public bool? FacilityInquiry { get; set; }

        /// <summary>
        /// استعلام کد پستی
        /// </summary>
        public bool? PostalCodeInquiry { get; set; }

        /// <summary>
        /// استعلام تحریم شورا امنیت
        /// </summary>
        public bool? SecurityCouncilSanctionsInquiry { get; set; }

        /// <summary>
        /// استعلام شاهکار
        /// </summary>
        public bool? ShahkarInquiry { get; set; }

        /// <summary>
        /// استعلام نظام وظیفه
        /// </summary>
        public bool? MilitaryInquiry { get; set; }

        /// <summary>
        /// استعلام لیست سیاه
        /// </summary>
        public bool? BlackListInquiry { get; set; }
        public bool? FinalResult { get; set; }

        public string? Description { get; set; }

        public string? OrganizationName { get; set; }
        public int? OrganizationId { get; set; }
        public int? RequestFacilityId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ShamsiCreatedDate
        {
            get
            {
                return CreatedDate != null ? CreatedDate!.Value.GregorianToShamsi(showTime: true) : null;
            }
        }
        public Guid CreatorId { get; set; }
        public string? Creator { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
