using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using static Common.Enums;

namespace Core.Entities
{   
    public class UserIdentityDocument : BaseDocument
    {
        public UserIdentityDocument()
        {

        }

        public DocumentType DocumentType { get; set; }

        #region User
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        #endregion

        #region User
        public int? BankId { get; set; }
        public virtual Bank Bank { get; set; }
        #endregion
    }

    public class UserIdentityDocumentConfiguration : BaseEntityTypeConfiguration<UserIdentityDocument>
    {
        public override void Configure(EntityTypeBuilder<UserIdentityDocument> builder)
        {
            builder.HasOne(p => p.User).WithMany(p => p.UserIdentityDocuments).HasForeignKey(p => p.UserId);
            builder.HasOne(p => p.Bank).WithMany(p => p.UserIdentityDocuments).HasForeignKey(p => p.BankId);
            builder.Property(p => p.DocumentType).HasDefaultValue(DocumentType.Other);
            builder.Property(p => p.FilePath).IsRequired();
        }
    }
}
