using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class PlanMember : BaseEntity
    {
        public PlanMember()
        {
            //Users = new HashSet<User>();
            RequestFacilities = new HashSet<RequestFacility>();
        }
        public string? NationalCode { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string? SSID { get; set; }
        public string? BirthCertificateSerial { get; set; }
        //public string BirthCertificateNumber { get; set; }
        //public string BirthCertificateLetter { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? FatherName { get; set; }
        public string? Mobile { get; set; }
        public string? PostalCode { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? GenderText { get; set; }
        public GenderEnum? Gender { get; set; }

        #region Plan
        public int PlanId { get; set; }
        public virtual Plan Plan { get; set; }
        #endregion

        #region Plan
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        #endregion

        #region Birth Location
        public int? BirthLocationId { get; set; }
        public virtual Location BirthLocation { get; set; }
        public string? BirthLocationProvince { get; set; }
        public string? BirthLocationCity { get; set; }
        #endregion  Birth Location

        #region Address Location
        public int? CityId { get; set; }
        public virtual Location City { get; set; }
        public string? AddressProvince { get; set; }
        public string? AddressCity { get; set; }

        #endregion

        #region City of Issue
        public int? CityOfIssueId { get; set; }
        public virtual Location CityOfIssue { get; set; }
        public string? ProvinceOfIssue { get; set; }
        public string? CityOfIssueName { get; set; }

        #endregion

        #region Account Info
        public string? CustomerNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? IBAN { get; set; }
        public string? CardNumber { get; set; }
        public string? BankName { get; set; }
        public string? DepositStatus { get; set; }
        public string? DepositOwners { get; set; }
        #endregion

        #region Facility Info
        public long? FacilityAmount { get; set; }

        #endregion Facility Info

        #region Import Result
        public bool? ImportSuccess { get; set; }
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }

        #endregion Import Result

        public virtual ICollection<RequestFacility> RequestFacilities { get; set; }
    }

    public class PlanMemberConfiguration : IEntityTypeConfiguration<PlanMember>
    {
        public void Configure(EntityTypeBuilder<PlanMember> builder)
        {
            //builder.HasIndex(p => p.Nationalcode).IsUnique();
            //builder.Property(p => p.Nationalcode).IsRequired();

            //builder.HasIndex(p => p.Mobile).IsUnique();
            //builder.Property(p => p.Mobile).IsRequired();

            builder.Property(p => p.FName).HasMaxLength(200);
            builder.Property(p => p.LName).HasMaxLength(200);
            builder.Property(p => p.FatherName).HasMaxLength(200);
            builder.Property(p => p.SSID).HasMaxLength(20);
            builder.Property(p => p.BirthCertificateSerial).HasMaxLength(15);
            builder.Property(p => p.Address).HasMaxLength(500);
            builder.Property(p => p.PostalCode).HasMaxLength(15);
            builder.Property(p => p.AccountNumber).HasMaxLength(20).HasDefaultValue("");
            builder.Property(p => p.CardNumber).HasMaxLength(16).HasDefaultValue("");
            builder.Property(p => p.IBAN).HasMaxLength(50).HasDefaultValue("");
            builder.Property(p => p.BankName).HasMaxLength(200).HasDefaultValue("");
            builder.Property(p => p.DepositOwners).HasMaxLength(500).HasDefaultValue("");
            builder.Property(p => p.DepositStatus).HasMaxLength(10).HasDefaultValue("");
            builder.Property(p => p.CustomerNumber).HasMaxLength(25).HasDefaultValue("");

            builder.HasOne(p => p.Plan).WithMany(p => p.PlanMembers).HasForeignKey(p => p.PlanId);
            builder.HasOne(p => p.User).WithMany(p => p.PlanMembers).HasForeignKey(p => p.UserId);
            builder.HasOne(p => p.BirthLocation).WithMany(p => p.BirthLocationPlanMembers).HasForeignKey(p => p.BirthLocationId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.City).WithMany(p => p.CityPlanMembers).HasForeignKey(p => p.CityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.CityOfIssue).WithMany(p => p.CityOfIssuePlanMembers).HasForeignKey(p => p.CityOfIssueId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
