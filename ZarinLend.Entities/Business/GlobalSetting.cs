using Core.Entities.Base;
using Core.Entities.Business.Payment;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// save global setting such as verifyy price,sales commission
    /// </summary>

    public class GlobalSetting : BaseEntity
    {
        public GlobalSetting()
        {
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //public override int Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// validation fee
        /// </summary>
        public long  ValidationFee { get; set; }

        /// <summary>
        ///مدت زمان اعتبار نتیجه اعتبارسنجی ایرانیان به روز
        /// </summary>
        public int ValidityPeriodOfValidation { get; set; }

        /// <summary>
        /// درصد ضمانت نامه/چک با توجه به وام درخواستی 
        /// </summary>
        public double WarantyPercentage { get; set; }

        /// <summary>
        /// سود تسهیلات
        /// </summary>
        public double FacilityInterest { get; set; }

        ///// <summary>
        ///// کارمزد تسهیلات/عملیات
        ///// </summary>
        //public double FacilityFee { get; set; }

        /// <summary>
        /// کارمزد تسهیلات/عملیات-سهم نهاد مالی
        /// </summary>
        public double LendTechFacilityFee { get; set; }

        /// <summary>
        /// کارمزد تسهیلات/عملیات برای مشتریان زرین پال-سهم نهاد مالی
        /// </summary>
        public double LendTechFacilityForZarinpalClientFee { get; set; }

        /// <summary>
        /// کارمزد تسهیلات/عملیات-سهم لندتک 
        /// </summary>
        public double FinancialInstitutionFacilityFee { get; set; }

        public bool  IsActive { get; set; }
        public virtual ICollection<RequestFacility> RequestFacilities { get; set; } = new HashSet<RequestFacility>();
        public virtual ICollection<Plan> Plans { get; set; } = new HashSet<Plan>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();

        #region Creator
        public Guid CreatorId { get; set; }

        public virtual User Creator { get; set; }
        #endregion

        #region Updater
        public Guid? UpdaterId { get; set; }

        public virtual User Updater { get; set; }
        #endregion
    }

    public class GlobalSettingConfiguration : BaseEntityTypeConfiguration<GlobalSetting>
    {
        public override void Configure(EntityTypeBuilder<GlobalSetting> builder)
        {
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorGlobalSettings).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Updater).WithMany(p => p.UpdaterGlobalSettings).HasForeignKey(p => p.UpdaterId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
