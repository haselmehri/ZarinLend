using Common;
using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class FindOpenRequestFacilityResultModel
    {
        public int Id { get; set; }
        public Guid RequesterId { get; set; }

        [Display(Name = "FName")]
        public string FName { get; set; }

        [Display(Name = "LName")]
        public string LName { get; set; }

        [Display(Name = "FatherName")]
        public string FatherName { get; set; }

        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        public string NationalCode { get; set; }

        [Display(Name = "RequestAmount")]
        public long Amount { get; set; }

        [Display(Name = "MonthCountTitle")]
        public string MonthCountTitle { get; set; }

        [Display(Name = "MonthCount")]
        public int MonthCount { get; set; }

        [Display(Name = "CreateDate")]
        public DateTime CreateDate { get; set; }

        public Guid BuyerId { get; set; }
        [Display(Name = "CreateDate")]
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }

        public bool GuarantorIsRequired { get; set; }


        #region GlobalSetting
        public long ValidationFee { get; set; }
        public double WarantyPercentage { get; set; }
        public double FacilityInterest { get; set; }
        public double LendTechFacilityFee { get; set; }
        public double FinancialInstitutionFacilityFee { get; set; }

        #endregion GlobalSetting

        #region Computational Properties
        public long AmountInstallment
        {
            get
            {
                if (Amount == default || MonthCount == default || FacilityInterest == default) return 0;
                return Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateAmountInstallment(Amount, MonthCount, FacilityInterest)));
            }
        }
        public long TotalInstallment
        {
            get
            {
                if (Amount == default || MonthCount == default || FacilityInterest == default) return 0;
                return Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateTotalInstallment(Amount, MonthCount, FacilityInterest)));
            }
        }
        public long ChequeAmountWarranty
        {
            get
            {
                if (Amount == default || MonthCount == default || FacilityInterest == default || WarantyPercentage == default) return 0;
                return Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateChequeAmountWarranty(Amount, MonthCount, FacilityInterest, WarantyPercentage)));
            }
        }
        public long TotalPayment
        {
            get
            {
                if (Amount == default || MonthCount == default || FacilityInterest == default) return 0;
                return Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateTotalPayment(Amount, MonthCount, FacilityInterest, LendTechFacilityFee + FinancialInstitutionFacilityFee)));
            }
        }
        public long PrePaymentOrFee
        {
            get
            {
                if (Amount == default || AmountInstallment == default) return 0;
                return Convert.ToInt64(Math.Round(InstallmentCalculator.CalculatePrePaymentOrFee(Amount, MonthCount, LendTechFacilityFee + FinancialInstitutionFacilityFee)));
            }
        }
        #endregion
    }
}