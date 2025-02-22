using Core.Entities.Base;
using System;
using static Common.Enums;

namespace Core.Entities
{
    public abstract class BaseDocument : IEntity
    {
        public virtual int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string FilePath { get; set; }
        public bool IsPrivate { get; set; }
        public FileType FileType { get; set; }
        public DocumentStatus Status { get; set; }
        public short Version { get; set; } = 1;
    }
}