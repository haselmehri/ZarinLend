using Common.Utilities;
using Core.Entities.Business.Payment;
using System;

namespace Services.Model
{
    [Serializable]
    public class SamanIntenetBankPaymentExportModel
    {
        public long Id { get; set; }
        public PaymentType PaymentType { get; set; }
        public string PaymentTypeDesc
        {
            get
            {
                switch (PaymentType)
                {
                    case PaymentType.PayValidationFeeByGuarantor:
                        return "کارمزد اعتبارسنجی(ضامن)";
                    case PaymentType.PayValidationFee:
                        return "کارمزد اعتبارسنجی(تسهیلات گیرنده)";
                    case PaymentType.PayInstallment:
                        return "پرداخت اقساط تسهیلات";
                    case PaymentType.PaySalesCommission:
                        return "پرداخت کمیسیون فروش";
                    default:
                        return PaymentType.ToString();
                }
            }
        }
        public long Amount { get; set; }
        #region GlobalSetting
        //public long ValidationFee { get; set; }
        //public double WarantyPercentage { get; set; }
        //public double FacilityInterest { get; set; }
        public double LendTechFacilityFee { get; set; }
        public double LendTechFacilityFeeAmount
        {
            get
            {
                if (PaymentType.PaySalesCommission == PaymentType)
                {
                    var valueBase100Percentage = LendTechFacilityFee * 100 / (LendTechFacilityFee + FinancialInstitutionFacilityFee);
                    return valueBase100Percentage * Amount / 100;
                }
                else
                    return 0;
            }
        }
        public double FinancialInstitutionFacilityFee { get; set; }
        public double FinancialInstitutionFacilityAmount {
            get
            {
                if (PaymentType.PaySalesCommission == PaymentType)
                {
                    var valueBase100Percentage = FinancialInstitutionFacilityFee * 100 / (LendTechFacilityFee + FinancialInstitutionFacilityFee);
                    return valueBase100Percentage * Amount / 100;
                }
                else
                    return 0;
            }
        }

        #endregion GlobalSetting
        public string Payer { get; set; }
        public string PayerNationalCode { get; set; }
        public bool? IsSuccess { get; set; }
        public string State { get; set; }
        public int? Status { get; set; }
        public string MaskedPan { get; set; }
        public string RRN { get; set; }
        public string ResNum { get; set; }
        public string RefNum { get; set; }
        public string TraceNo { get; set; }
        //public Guid UserId { get; set; }
        //public int? RequestFacilityId { get; set; }
        //public int? RequestFacilityInstallmentId { get; set; }
        //public int? RequestFacilityGuarantorId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string StraceDate { get; set; }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }
        public string ShamsiUpdateDate
        {
            get
            {
                return UpdateDate.HasValue ? UpdateDate.Value.GregorianToShamsi(showTime: true) : null;
            }
        }

        public string ShamsiStraceDate
        {
            get
            {
                try
                {
                    return StraceDate.HasValue() ? Convert.ToDateTime(StraceDate).GregorianToShamsi(showTime: true) : null;
                }
                catch (Exception)
                {

                }
                return null;
                
            }
        }
    }
}
