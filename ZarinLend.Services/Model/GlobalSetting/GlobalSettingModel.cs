using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.GlobalSetting
{
    [Serializable]
    public class GlobalSettingModel
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[RegularExpression(RegularExpression.FloatNumber, ErrorMessage = "FloatRegularExpression")]
        [RegularExpression(RegularExpression.FloatNumber3,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        [Display(Name = "کارمزد تسهیلات-زرین لند(درصد)")]
        [Range(0, 100, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RangeValidationMessage))]
        public double LendTechFacilityFee { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[RegularExpression(RegularExpression.FloatNumber, ErrorMessage = "FloatRegularExpression")]
        [RegularExpression(RegularExpression.FloatNumber3, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        [Display(Name = "کارمزد تسهیلات برای مشتریان زرین پال-زرین لند(درصد)")]
        [Range(0, 100, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RangeValidationMessage))]
        public double LendTechFacilityForZarinpalClientFee { get; set; }        

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.FloatNumber3,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        [Display(Name = "کارمزد تسهیلات-نهاد مالی(درصد)")]
        [Range(0, 100, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RangeValidationMessage))]
        public double FinancialInstitutionFacilityFee { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.FloatNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        [Range(0, 100, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RangeValidationMessage))]
        [Display(Name = "سود تسهیلات(درصد)")]
        public int FacilityInterest { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.FloatNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        [Range(0, 500, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RangeValidationMessage))]
        [Display(Name = "مبلغ ضمانت نامه(درصد)")]
        public int WarantyPercentage { get; set; }

        [Display(Name = "هزینه اعتبار سنجی(ريال)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long ValidationFee { get; set; }

        [Display(Name = "هزینه اعتبار سنجی(ريال)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public string ValidationFeeThousandSeparator { get; set; }

        [Display(Name = "مدت زمان اعتبار نتیجه اعتبارسنجی(روز)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public int ValidityPeriodOfValidation { get; set; }

        public Guid CreatorId { get; set; }
        public Guid UpdaterId { get; set; }
    }
}
