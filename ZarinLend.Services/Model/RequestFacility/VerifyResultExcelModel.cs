using Common.Utilities;
using Core.Entities;
using System;
using static Stimulsoft.Base.Drawing.StiFontReader;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Services.Model
{
    public class VerifyResultExcelModel
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string Creator { get; set; }
        public string ExcelFilePath { get; set; }
        public int RowCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int UnknownCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ShamsiCreatedDate
        {
            get
            {
                return CreatedDate.GregorianToShamsi(showTime: true);
            }
        }
    }
}
