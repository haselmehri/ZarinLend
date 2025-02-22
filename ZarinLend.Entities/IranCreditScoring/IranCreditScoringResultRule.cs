using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public enum IranCreditScoringResultRuleType
    {
        ForRequestFacility = 1,
        ForRequestFacilityGuarantor = 2
    }
    /// <summary>
    /// Based on the rules in this table, it is determined how much each applicant can borrow
    /// </summary>
    public class IranCreditScoringResultRule : BaseEntity
    {
        public IranCreditScoringResultRule()
        {
            
        }

        public IranCreditScoringResultRuleType IranCreditScoringResultRuleType { get; set; }
        /// <summary>
        /// risk to pay facility is shown to A,B,C...
        /// </summary>
        public string? Risk { get; set; }

        /// <summary>
        /// Minimum Amount
        /// </summary>
        public long? MinimumAmount { get; set; }

        /// <summary>
        /// Minimum Amount
        /// </summary>
        public long? MaximumAmount { get; set; }

        public bool GuarantorIsRequired { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<RequestFacility> RequestFacilities { get; set; } = new HashSet<RequestFacility>();
        public virtual ICollection<RequestFacilityGuarantor> RequestFacilityGuarantors { get; set; } = new HashSet<RequestFacilityGuarantor>();


        #region Creator
        public Guid CreatorId { get; set; }

        public virtual User Creator { get; set; }
        #endregion

        #region Updater
        public Guid? UpdaterId { get; set; }

        public virtual User Updater { get; set; }
        #endregion
    }

    public class IranCreditScoringResultRuleConfiguration : BaseEntityTypeConfiguration<IranCreditScoringResultRule>
    {
        public override void Configure(EntityTypeBuilder<IranCreditScoringResultRule> builder)
        {
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorIranCreditScoringResultRules).HasForeignKey(p => p.CreatorId);
            builder.HasOne(p => p.Updater).WithMany(p => p.UpdaterIranCreditScoringResultRules).HasForeignKey(p => p.UpdaterId);
            builder.Property(p => p.Risk).HasMaxLength(50);
        }
    }
}
