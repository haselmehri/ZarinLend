using Common;
using Common.Utilities;
using Core.Entities.Business.RequestFacility;
using System;
using System.Collections.Generic;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityDetailModel
    {
        public int Id { get; set; }
        public bool UserIsZarinpalClient { get; set; }
        public long Amount { get; set; }
        public UserOption UserOption { get; set; }
        public string MonthCountTitle { get; set; }
        public int MonthCount { get; set; }
        public bool CancelByUser { get; set; }
        public bool GuarantorIsRequired { get; set; }
        public List<string>? UsagePlaceList { get; set; }
        public string UsagePlaceOtherDescription { get; set; }
        #region GlobalSetting
        public long ValidationFee { get; set; }
        public double WarantyPercentage { get; set; }
        public double FacilityInterest { get; set; }
        public double LendTechFacilityFee { get; set; }
        public double LendTechFacilityForZarinpalClientFee { get; set; }
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
                return Convert.ToInt64(Math.Round(InstallmentCalculator.CalculateTotalPayment(Amount, MonthCount, FacilityInterest, TotalFee)));
            }
        }
        public long PrePaymentOrFee
        {
            get
            {
                if (Amount == default || AmountInstallment == default || UserOption == UserOption.PurchaseFromContractedStores) return 0;
                return Convert.ToInt64(Math.Round(InstallmentCalculator.CalculatePrePaymentOrFee(Amount, MonthCount, TotalFee)));
            }
        }

        public double TotalFee
        {
            get {
                return UserIsZarinpalClient ? LendTechFacilityForZarinpalClientFee + FinancialInstitutionFacilityFee : LendTechFacilityFee + FinancialInstitutionFacilityFee;
}
        }
        #endregion

        public string SignedContractByUserFileName { get; set; }
        public string SignedContractByBankFileName { get; set; }
        public string FacilityNumber { get; set; }
        public string PoliceNumber { get; set; }
        public string LeasingName { get; set; }
        public long? LeasingNationalId { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid BuyerId { get; set; }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }
    }
}