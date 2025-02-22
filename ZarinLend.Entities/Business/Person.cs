using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public enum GenderEnum
    {
        Male = 1,
        Female = 2
    }
    public class Person : BaseEntity
    {
        public Person()
        {
            //Users = new HashSet<User>();
        }
        /// <summary>
        /// Zarinpal Id
        /// </summary>
        public long? ZP_Id { get; set; }
        public string NationalCode { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string SSID { get; set; }
        public string? IdentificationSerial { get; set; }
        public string? IdentificationSeri { get; set; }
        //public string BirthCertificateLetter { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string FatherName { get; set; }
        public string Mobile { get; set; }
        public bool MobileConfirmed { get; set; }
        public bool MobileShahkarConfirmed { get; set; }
        public string? PostalCode { get; set; }

        /// <summary>
        /// currently fill from Zarinpal
        /// </summary>
        public bool? VerifiedPostalCode { get; set; }
        public string? Address { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        //public byte[] Avatar { get; set; }
        public GenderEnum? Gender { get; set; }
        public bool GetIdentityDataFromCivilRegistryData { get; set; }
        public string? VerifiedAddress { get; set; }
        public bool IsAddressValidated { get; set; }
        public string? SanaTrackingId { get; set; }

        #region Organization
        public int? OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion

        #region Country/Noationality
        public int CountryId { get; set; }

        /// <summary>
        /// Nationality
        /// </summary>
        public virtual Country Country { get; set; }
        #endregion

        #region Birth Location
        //public int? BirthLocationId { get; set; }
        //public virtual Location BirthLocation { get; set; }
        #endregion  Birth Location

        #region City(Address)
        public int? CityId { get; set; }
        public virtual Location City { get; set; }
        #endregion

        #region City of Issue
        //public int? CityOfIssueId { get; set; }
        //public virtual Location CityOfIssue { get; set; }
        #endregion

        #region Account Info
        //public string? SanaNumber { get; set; }
        public string? CustomerNumber { get; set; }
        //public string? AccountNumber { get; set; }
        //public string? IBAN { get; set; }
        //public string? CardNumber { get; set; }

        /// <summary>
        /// hash base sha256
        /// </summary>
        public string? HashCardNumber { get; set; }
        //public string BankName { get; set; }
        //public string DepositStatus { get; set; }
        //public string DepositOwners { get; set; }
        //public int? BankId { get; set; }
        //public virtual Bank Bank { get; set; }
        #endregion

        public virtual ICollection<PersonJobInfo> PersonJobInfos { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public string FullName {
            get
            {
                return $"{FName} {LName}";
            }
        }
    }

    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasIndex(p => p.NationalCode).IsUnique();
            builder.Property(p => p.NationalCode).IsRequired();

            builder.HasIndex(p => p.ZP_Id).IsUnique();

            builder.HasIndex(p => p.Mobile).IsUnique();
            builder.Property(p => p.Mobile).IsRequired();

            builder.Property(p => p.Mobile).HasMaxLength(15);
            //builder.Property(p => p.SanaNumber).HasMaxLength(20);
            builder.Property(p => p.FName).HasMaxLength(200).IsRequired();
            builder.Property(p => p.LName).HasMaxLength(200).IsRequired();
            builder.Property(p => p.FatherName).HasMaxLength(200).IsRequired();
            builder.Property(p => p.SSID).HasMaxLength(20).IsRequired();
            builder.Property(p => p.IdentificationSerial).HasMaxLength(15);
            builder.Property(p => p.IdentificationSeri).HasMaxLength(15);
            builder.Property(p => p.PlaceOfBirth).HasMaxLength(250);
            builder.Property(p => p.Address).HasMaxLength(500);
            builder.Property(p => p.PostalCode).HasMaxLength(15);
            //builder.Property(p => p.AccountNumber).HasMaxLength(20);
            //builder.Property(p => p.CardNumber).HasMaxLength(16);
            builder.Property(p => p.HashCardNumber).HasMaxLength(100);
            //builder.Property(p => p.IBAN).HasMaxLength(50);
            //builder.Property(p => p.BankName).HasMaxLength(200);
            //builder.Property(p => p.DepositOwners).HasMaxLength(500);
            //builder.Property(p => p.DepositStatus).HasMaxLength(10);
            builder.Property(p => p.CustomerNumber).HasMaxLength(25);

            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.HasOne(p => p.Organization).WithMany(p => p.People).HasForeignKey(p => p.OrganizationId);
            builder.HasOne(p => p.Country).WithMany(p => p.People).HasForeignKey(p => p.CountryId);
            //builder.HasOne(p => p.BirthLocation).WithMany(p => p.BirthLocationPeople).HasForeignKey(p => p.BirthLocationId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.City).WithMany(p => p.CityPeople).HasForeignKey(p => p.CityId).OnDelete(DeleteBehavior.Restrict);
            //builder.HasOne(p => p.CityOfIssue).WithMany(p => p.CityOfIssuePeople).HasForeignKey(p => p.CityOfIssueId).OnDelete(DeleteBehavior.Restrict);
            //builder.HasOne(p => p.Bank).WithMany(p => p.People).HasForeignKey(p => p.BankId);
        }
    }
}
