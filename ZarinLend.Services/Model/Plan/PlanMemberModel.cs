using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class PlanMemberModel : IValidatableObject
    {
        [Display(Name = "UserName", ResourceType = typeof(ResourceFile))]
        public Guid? UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(200, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessage = "{0} را فقط با حروف فارسی وارد کنید")]
        [Display(Name = "نام")]
        public string FName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessage = "{0} را فقط با حروف فارسی وارد کنید")]
        [StringLength(200, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "نام خانوادگی")]
        public string LName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessage = "{0} را فقط با حروف فارسی وارد کنید")]
        [StringLength(200, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }

        [StringLength(500, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "آدرس")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        public string Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(10, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "شماره شناسنامه")]
        [RegularExpression(RegularExpression.SSID, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        public string SSID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(6, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "سريال شناسنامه")]
        [RegularExpression(RegularExpression.BirthCertificateSerial, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        public string BirthCertificateSerial { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "جنسیت")]
        public GenderEnum? Gender { get; set; }
        public string GenderText { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(11, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "موبایل")]
        [RegularExpression(RegularExpression.Mobile, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        public string Mobile { get; set; }

        [StringLength(50, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "تلفن ثابت")]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(10, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "کد ملی")]
        [RegularExpression(RegularExpression.NationalCode, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        public string NationalCode { get; set; }

        [Display(Name = "ایمیل")]
        [RegularExpression(RegularExpression.Email, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        public string Email { get; set; }

        [Display(Name = "کد پستی")]
        [StringLength(10, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [RegularExpression(RegularExpression.PostalCode, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        public string PostalCode { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        public DateTime? BirthDate
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(ShamsiBirthDate) && ShamsiBirthDate.Split("/").Length == 3)
                        return DateTimeHelper.ShamsiToGregorian(ShamsiBirthDate);

                    return null;
                }
                catch (Exception)
                {
                    throw new AppException("تاریخ تولد را با فرمت صحیح وارد کنید. فرمت صحیح : ----/--/--");
                }
            }
        }

        [Display(Name = "تاریخ تولد")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        public string ShamsiBirthDate { get; set; }

        [Display(Name = "استان محل تولد")]
        public string ProvinceOfBirthName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "استان محل تولد")]
        public int? ProvinceOfBirthId { get; set; }

        [Display(Name = "استان محل صدور شناسنامه")]
        public string ProvinceOfIssueName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "استان محل صدور شناسنامه")]
        public int? ProvinceOfIssueId { get; set; }

        [Display(Name = "استان محل سکونت")]
        public string ProvinceName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "استان محل سکونت")]
        public int? ProvinceId { get; set; }

        [Display(Name = "شهر محل سکونت")]
        public string CityName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "شهر محل سکونت")]
        public int? CityId { get; set; }

        [Display(Name = "شهر محل تولد")]
        public string CityOfBirthName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "شهر محل تولد")]
        public int? CityOfBirthId { get; set; }

        [Display(Name = "شهر محل صدور")]
        public string CityOfIssueName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "شهر محل صدور")]
        public int? CityOfIssueId { get; set; }

        #region Bank Info

        [Display(Name = "شماره مشتری")]
        [StringLength(15, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [RegularExpression(RegularExpression.CustomerNumber, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        public string CustomerNumber { get; set; }

        [Display(Name = "شماره حساب")]
        [StringLength(20, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [RegularExpression(RegularExpression.AccountNumber, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        public string AccountNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(20, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "شماره کارت")]
        [RegularExpression(RegularExpression.AyandehBankCardNumberWithDash, ErrorMessage = @"شماره کارت باید با فرمت صحیح <spna style='direction: ltr;unicode-bidi: embed;'>6362-14__-____-____</span> وارد شود")]
        public string CardNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(26, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "شماره شبا")]
        [RegularExpression(RegularExpression.IBAN, ErrorMessage = "{0} را با فرمت صحیح وارد کنید")]
        public string IBAN { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        //[Display(Name = "DepositStatus")]
        //public string DepositStatus { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        //[Display(Name = "DepositOwners")]
        //public string DepositOwners { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [StringLength(200, ErrorMessage = "{0} حداکثر میتواند {1} حرف/عدد باشد")]
        [Display(Name = "نام بانک")]
        public string BankName { get; set; }

        #endregion

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} اجباری است!")]
        [Display(Name = "مبلغ درخواستی وام")]
        public long? FacilityAmount { get; set; }

        public bool IsEditMode { get; set; } = false;
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public bool? ImportSuccess { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NationalCodeHelper.IsValidIranianNationalCode(NationalCode))
                yield return new ValidationResult("فرمت کد ملی صحیح نمی باشد!");

            if (!BankHelper.CheckIBAN(IBAN))
                yield return new ValidationResult("فرمت شماره شبا صحیح نمی باشد!");

            yield return ValidationResult.Success;
        }
    }
}
