﻿using Common.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityFilterModel
    {
        [Display(Name = "نام")]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? FName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? LName { get; set; }

        [Display(Name = "مبلغ درخواستی از")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? StartAmount { get; set; }

        [Display(Name = "مبلغ درخواستی تا")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? EndAmount { get; set; }

        [Display(Name = "مبلغ درخواستی از")]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? StartAmountThousandSeparator { get; set; }

        [Display(Name = "مبلغ درخواستی تا")]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? EndAmountThousandSeparator { get; set; }

        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string? NationalCode { get; set; }

        [Display(Name = "تاریخ درخواست از")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاریخ درخواست تا")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "تاریخ پرداخت تسهیلات از")]
        public DateTime? PaymentFacilityStartDate { get; set; }

        [Display(Name = "تاریخ پرداخت تسهیلات تا")]
        public DateTime? PaymentFacilityEndDate { get; set; }

        [Display(Name = "مرحله جاری")]
        public int? WaitingStepId { get; set; }

        [Display(Name = "مراحل درخواست")]
        public IEnumerable<SelectListItem> WorkFlowSteps { get; set; }
    }
}
