using System;
using static Common.Enums;

namespace Services.Model
{
    [Serializable]
    public class DocumentModel
    {
        public virtual int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string FilePath { get; set; }
        public bool IsPrivate { get; set; }
        public DocumentType DocumentType { get; set; }
        public FileType FileType { get; set; }
        public DocumentStatus Status { get; set; }
        public string DocumentNumber { get; set; }
        public short Version { get; set; } = 1;
    }
}
