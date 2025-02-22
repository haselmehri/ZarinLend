using Common;
using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityGuarantorListModel
    {
        public int Id { get; set; }
        public Guid GuarantorUserId { get; set; }
        public string GuarantorFullName { get; set; }
        public string GuarantorNationalCode { get; set; }
        public long Amount { get; set; }
        public string MonthCountTitle { get; set; }
        public int MonthCount { get; set; }
        //public string LeasingName { get; set; }
        public string Requester { get; set; }
        public string NationalCode { get; set; }
        public bool CancelByUser { get; set; }
        public short? StatusId
        {
            get
            {
                if (RequestFacilityGuarantorWorkFlowStepList != null && RequestFacilityGuarantorWorkFlowStepList.Any())
                {
                    if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep && p.StatusId == (short)StatusEnum.Approved))
                        return (short)StatusEnum.Approved;
                    else if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsLastStep && !p.IsApproveFinalStep && p.StatusId == (short)StatusEnum.Rejected))
                        return (short)StatusEnum.Rejected;
                    else
                        return null;
                }
                else
                    return null;
            }
        }
        public string FormUrl { get; set; }
        public int MaxWorkFlowStepId { get; set; }
        public string LastStatusDescription
        {
            get
            {
                if (RequestFacilityGuarantorWorkFlowStepList != null && RequestFacilityGuarantorWorkFlowStepList.Any())
                {
                    if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep && p.StatusId == (short)StatusEnum.Approved))
                        return "تایید نهایی ضامن";
                    else if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsLastStep && !p.IsApproveFinalStep && p.StatusId == (short)StatusEnum.Rejected))
                        return "رد (درخواست) ضامن";
                    else if (RequestFacilityGuarantorWorkFlowStepList.Any(p => !p.StatusId.HasValue))
                        return RequestFacilityGuarantorWorkFlowStepList.First(p => !p.StatusId.HasValue).WorkFlowStepName;
                    else
                        return RequestFacilityGuarantorWorkFlowStepList.OrderByDescending(p => p.CreateDate).First().WorkFlowStepName;
                }
                else
                    return null;
            }
        }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime LastActionDate { get; set; }
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
        public string ShamsiLastActionDate
        {
            get
            {
                return LastActionDate.GregorianToShamsi(showTime: true);
            }
        }

        public List<WorkFlowStepListModel> RequestFacilityGuarantorWorkFlowStepList { get; set; }

        #region GlobalSetting
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
