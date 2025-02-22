using Common.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.Invoice
{
    [Serializable]
    public class InvoiceFilterModel
    {
        [Display(Name = "مبلغ فاکتور از")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? StartAmount { get; set; }

        [Display(Name = "مبلغ فاکتور تا")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? EndAmount { get; set; }

        [Display(Name = "مبلغ فاکتور از")]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? StartAmountThousandSeparator { get; set; }

        [Display(Name = "مبلغ فاکتور تا")]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? EndAmountThousandSeparator { get; set; }

        [Display(Name = "تاریخ درخواست از")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاریخ درخواست تا")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "فروشگاه")]
        public IEnumerable<SelectListItem> Shops { get; set; }

        [Display(Name = "وضعیت")]
        public IEnumerable<SelectListItem> StatusList { get; set; }
    }
}
