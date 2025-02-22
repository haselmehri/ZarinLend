using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public enum StatusEnum
    {        
        /// <summary>
        /// برگشت جهت اصلاح
        /// </summary>
        ReturnToCorrection = 1,
        /// <summary>
        /// تایید شده/انجام شده
        /// </summary>
        Approved = 2,
        /// <summary>
        /// رد شده
        /// </summary>
        Rejected = 3
    }
    public class Status : BaseEntity<short>
    {
        public Status()
        {
            //RequestFacilities = new HashSet<RequestFacility>();
            RequestFacilityWorkFlowSteps = new HashSet<RequestFacilityWorkFlowStep>();
            RequestFacilityGuarantorWorkFlowSteps = new HashSet<RequestFacilityGuarantorWorkFlowStep>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override short Id { get => base.Id; set => base.Id = value; }
        public string Name { get; set; }
        public string? Description { get; set; }
        //public virtual ICollection<RequestFacility> RequestFacilities { get; set; }
        public virtual ICollection<RequestFacilityWorkFlowStep> RequestFacilityWorkFlowSteps { get; set; }
        public virtual ICollection<RequestFacilityGuarantorWorkFlowStep> RequestFacilityGuarantorWorkFlowSteps { get; set; }
    }

    public class StatusConfiguration : BaseEntityTypeConfiguration<Status>
    {
        public override void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
        }
    }
}

