using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities.Base
{
    public abstract class BaseEntity<TKey> : IEntity
    {
        public virtual TKey Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        //public bool IsDeleted { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int> { }

    public abstract class BaseEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase>
        where TBase : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TBase> builder)
        {
            //Base Configuration
            builder.Property("CreatedDate").HasDefaultValueSql("GetDate()");
        }
    }
}