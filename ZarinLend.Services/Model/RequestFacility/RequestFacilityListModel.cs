using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityListModel
    {
        public int Id { get; set; }
        public Guid ReguesterId { get; set; }
        public long Amount { get; set; }
        public string MonthCountTitle { get; set; }
        public string LeasingName { get; set; }
        public string Requester { get; set; }
        public string RequesterFName { get; set; }
        public string RequesterLName { get; set; }
        public string RequesterMobile { get; set; }
        public DateTime BirthDate { get; set; }
        public string ShamsiBirthDate { get
            {
                return BirthDate.GregorianToShamsi();
            }
        }
        public string NationalCode { get; set; }
        public string CustomerNumber { get; set; }
        public string AccountNumber { get; set; }
        public string FacilityNumber { get; set; }
        public string PoliceNumber { get; set; }

        #region Guarantor Detail
        public bool GuarantorIsRequired { get; set; }
        public bool AwaitingIntroductionGuarantor { get; set; }
        public List<RequestFacilityGuarantorDetailModel>? RequestFacilityGuarantorsDetail { get; set; }

        //public string LastGuarantorStatusDescription
        //{
        //    get
        //    {
        //        if (GuarantorIsRequired && RequestFacilityGuarantorsDetail != null && RequestFacilityGuarantorsDetail.Any())
        //        {
        //            if (RequestFacilityGuarantorsDetail.Any(p => !p.CancelByUser &&
        //                                                          p.RequestFacilityGuarantorWorkFlowStepList
        //                                                                .Any(x => x.IsApproveFinalStep && 
        //                                                                          x.IsLastStep)))
        //            {
        //                if (RequestFacilityGuarantorsDetail.Any(p => !p.CancelByUser && 
        //                                                              p.RequestFacilityGuarantorWorkFlowStepList
        //                                                                    .Any(x => x.IsApproveFinalStep && 
        //                                                                              x.IsLastStep && 
        //                                                                              x.StatusId.HasValue && 
        //                                                                              x.StatusId.Value == (short)StatusEnum.Approved)))
        //                    return "تایید نهایی ضامن";
        //                else
        //                    return RequestFacilityGuarantorsDetail.First(p => !p.CancelByUser &&
        //                                                                       p.RequestFacilityGuarantorWorkFlowStepList
        //                                                                            .Any(x => x.IsApproveFinalStep && 
        //                                                                                      x.IsLastStep)).;
        //            }
        //            else if (RequestFacilityWorkFlowStepList.Any(p => p.IsLastStep))
        //                return RequestFacilityWorkFlowStepList.First(p => p.IsLastStep).WorkFlowStepName;
        //            else if (RequestFacilityWorkFlowStepList.Any(p => !p.StatusId.HasValue))
        //                return RequestFacilityWorkFlowStepList.First(p => !p.StatusId.HasValue).WorkFlowStepName;
        //            else
        //                return RequestFacilityWorkFlowStepList.OrderByDescending(p => p.CreateDate).First().WorkFlowStepName;
        //        }
        //        else
        //            return null;
        //    }
        //}
        public short? GuarantorStatusId
        {
            get
            {
                if (GuarantorIsRequired && RequestFacilityGuarantorsDetail != null && RequestFacilityGuarantorsDetail.Any())
                {
                    if (RequestFacilityGuarantorsDetail.Any(p => !p.CancelByUser && 
                                                                  p.RequestFacilityGuarantorWorkFlowStepList
                                                                        .Any(x => x.IsApproveFinalStep && 
                                                                                  x.IsLastStep && 
                                                                                  x.StatusId.HasValue && 
                                                                                  x.StatusId.Value == (short)StatusEnum.Approved)))
                        return (short)StatusEnum.Approved;
                    else if (RequestFacilityWorkFlowStepList.Any(p => p.IsLastStep))
                        return (short)StatusEnum.Rejected;
                    else
                        return null;
                }
                else
                    return null;
            }
        }
        public int? RequestFacilityGuarantorId
        {
            get
            {
                if (GuarantorIsRequired && RequestFacilityGuarantorsDetail != null && RequestFacilityGuarantorsDetail.Any())
                {
                    if (RequestFacilityGuarantorsDetail.Any(p => !p.CancelByUser &&
                                                                  p.RequestFacilityGuarantorWorkFlowStepList.Any(x => !x.StatusId.HasValue)))
                    {
                        return RequestFacilityGuarantorsDetail.First(p => !p.CancelByUser &&
                                                                           p.RequestFacilityGuarantorWorkFlowStepList.Any(x => !x.StatusId.HasValue))
                                                              .RequestFacilityGuarantorId;
                    }
                    else
                    {
                        return RequestFacilityGuarantorsDetail.OrderByDescending(p => p.RequestFacilityGuarantorId)
                                                              .First()
                                                              .RequestFacilityGuarantorId;
                    }
                }

                return null;
            }
        }
        public string LastGuarantorFullName
        {
            get
            {
                if (GuarantorIsRequired && RequestFacilityGuarantorsDetail != null && RequestFacilityGuarantorsDetail.Any())
                {
                    if (RequestFacilityGuarantorsDetail.Any(p => !p.CancelByUser &&
                                                                  p.RequestFacilityGuarantorWorkFlowStepList.Any(x => !x.StatusId.HasValue)))
                    {
                        return RequestFacilityGuarantorsDetail.First(p => !p.CancelByUser &&
                                                                           p.RequestFacilityGuarantorWorkFlowStepList.Any(x => !x.StatusId.HasValue))
                                                              .GuarantorFullName;
                    }
                    else
                    {
                        return RequestFacilityGuarantorsDetail.OrderByDescending(p => p.RequestFacilityGuarantorId)
                                                              .First().GuarantorFullName;
                    }
                }

                return null;
            }
        }

        #endregion Guarantor Detail
        public bool CancelByUser { get; set; }
        public short? StatusId
        {
            get
            {
                if (RequestFacilityWorkFlowStepList != null && RequestFacilityWorkFlowStepList.Any())
                {
                    if (RequestFacilityWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep && p.StatusId == (short)StatusEnum.Approved))
                        return (short)StatusEnum.Approved;
                    else if (RequestFacilityWorkFlowStepList.Any(p => p.IsLastStep && !p.IsApproveFinalStep && p.StatusId == (short)StatusEnum.Rejected))
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
        public bool InquiryDone { get; set; }
        public string LastStatusDescription
        {
            get
            {
                if (RequestFacilityWorkFlowStepList != null && RequestFacilityWorkFlowStepList.Any())
                {
                    if (RequestFacilityWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep && p.StatusId == (short)StatusEnum.Approved))
                        return "تایید نهایی-تخصیص اعتبار";
                    else if (RequestFacilityWorkFlowStepList.Any(p => p.IsLastStep && !p.IsApproveFinalStep && p.StatusId == (short)StatusEnum.Rejected))
                        return "رد درخواست";
                    else if(RequestFacilityWorkFlowStepList.OrderBy(o => o.Id).LastOrDefault(l => l.StatusId != null) != null &&
                            RequestFacilityWorkFlowStepList.OrderBy(o => o.Id).LastOrDefault(l => l.StatusId != null).StatusId == (short)StatusEnum.ReturnToCorrection)
                        return "برگشت جهت اصلاح";
                    else if (RequestFacilityWorkFlowStepList.Any(p => !p.StatusId.HasValue))
                        return RequestFacilityWorkFlowStepList.First(p => !p.StatusId.HasValue).WorkFlowStepName;
                    else
                        return RequestFacilityWorkFlowStepList.OrderByDescending(p => p.CreateDate).First().WorkFlowStepName;
                }
                else
                    return null;
            }
        }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime LastActionDate { get; set; }
        public string? SignedContractByBankFileName { get; set; }
        public string? Operator { get; set; }
        public bool CanAdminLeasingSignContract
        {
            get
            {
                if (CancelByUser || SignedContractByBankFileName != null) return false;

                if (RequestFacilityWorkFlowStepList != null)
                {
                    if (RequestFacilityWorkFlowStepList.Any(x => x.StatusId.HasValue &&
                            x.StatusId == (short)StatusEnum.Approved &&
                            x.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature &&
                            x.WorkFlowStepRoles.Any(c => c == RoleEnum.AdminBankLeasing.ToString())) &&
                       (SignedContractByBankFileName == null || SignedContractByBankFileName == string.Empty)) return true;

                    if (RequestFacilityWorkFlowStepList.Any(x => !x.StatusId.HasValue &&
                                x.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature &&
                                x.WorkFlowStepRoles.Any(c => c == RoleEnum.AdminBankLeasing.ToString())) ||
                       (RequestFacilityWorkFlowStepList.Any(x => x.StatusId.HasValue &&
                                x.WorkFlowFormId == WorkFlowFormEnum.AdminBankLeasingSignature) &&
                                (SignedContractByBankFileName == null || SignedContractByBankFileName == string.Empty))) return true;
                }
                return false;
            }
        }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }
        public string? ShamsiUpdateDate
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

        //public WorkFlowStepListModel WaitingStep { get; set; }
        public List<WorkFlowStepListModel>? RequestFacilityWorkFlowStepList { get; set; }
    }
}
