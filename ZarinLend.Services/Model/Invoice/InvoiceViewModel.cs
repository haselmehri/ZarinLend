using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.Model.Invoice
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }

        [Display(Name = "وضعیت")]
        public InvoiceStatus Status { get; set; }

        [Display(Name = "وضعیت")]
        public string StatusDescription
        {
            get
            {
                return Status.ToDisplay();
            }
        }

        public int OrganizationId { get; set; }

        [Display(Name ="فروشگاه")]
        public  string OrganizationName { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string Number { get; set; }

        [Display(Name = "مبلغ فاکتور")]
        public long Amount { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "ایجاد کننده")]
        public string Creator { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public string ShamsiCreateDate
        {
            get
            {
                return DateTimeHelper.GregorianToShamsi(CreateDate);
            }
        }

        public List<DocumentModel> InvoiceDocuments { get; set; }
    }
}
