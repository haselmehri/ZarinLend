using Common.Utilities;
using Core.Entities.Business.Payment;
using System;

namespace Services.Model
{
    [Serializable]
    public class SamanIntenetBankPaymentModel
    {
        public long Id { get; set; }
        public PaymentType PaymentType { get; set; }
        public long Amount { get; set; }
        public string Payer { get; set; }
        public string PayerNationalCode { get; set; }
        public bool? IsSuccess { get; set; }
        public string MaskedPan { get; set; }
        public string ResNum { get; set; }
        public Guid UserId { get; set; }
        public int? RequestFacilityId { get; set; }
        public int? RequestFacilityInstallmentId { get; set; }
        public int? RequestFacilityGuarantorId { get; set; }
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

        #region GlobalSetting

        public long ValidationFee { get; set; }
        public double LendTechFacilityFee { get; set; }
        public double FinancialInstitutionFacilityFee { get; set; }

        #endregion GlobalSetting
    }
}
