using Common.Utilities;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using System;
using System.Collections.Generic;

namespace Services.Model
{
    [Serializable]
    public class WorkFlowStepListModel
    {
        public int Id { get; set; }
        public int WorkFlowStepId { get; set; }
        public string WorkFlowStepName { get; set; }
        public short? StatusId { get; set; }
        public string? StatusDescription { get; set; }
        public bool IsApproveFinalStep { get; set; }
        public bool IsLastStep { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public IEnumerable<string>? WorkFlowStepRoles { get; set; }
        public string? WorkFlowFormUrl { get; set; }
        public WorkFlowFormEnum? WorkFlowFormId { get; set; }
        public bool StepIsManual { get; set; }
        public IEnumerable<WorkFlowStepRejectionReason> WorkFlowStepRejectionReasons { get; set; } = new List<WorkFlowStepRejectionReason>();

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
                return UpdateDate.HasValue ? CreateDate.GregorianToShamsi(showTime: true) : null;
            }
        }

    }
}
