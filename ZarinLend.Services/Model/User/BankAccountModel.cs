using Common.Utilities;
using System;

namespace Services.Model
{
    public class BankAccountModel
    {
        public int Id { get; set; }
        public required string Deposit { get; set; }
        public required string CardNumber { get; set; }
        public required string IBAN { get; set; }
        public required string BankName { get; set; }
        public required string DepositOwners { get; set; }
        /// <summary>
        /// 02 : حساب فعال است
        /// ‌03 : حساب مسدود با قابلیت واریز
        /// ‌04 : حساب مسدود بدون قابلیت واریز
        /// ‌05 : حساب راکد است
        /// ‌06 : بروز خطادر پاسخ دهی, شرح خطا در فیلد توضیحات است
        /// ‌07 : سایر موارد
        /// </summary>
        public string DepositStatus { get; set; }
        public string DepositStatusDescription
        {
            get
            {
                switch (DepositStatus)
                {
                    case "02": return "حساب فعال است";
                    case "03": return "حساب مسدود با قابلیت واریز";
                    case "04": return "حساب مسدود بدون قابلیت واریز";
                    case "05": return "حساب راکد است";
                    case "06": return "بروز خطادر پاسخ دهی, شرح خطا در فیلد توضیحات است";
                    case "07": return "سایر موارد";
                    default: return "نامشخص";
                }
            }
        }
        public bool IsConfirm { get; set; }
        public DateTime CreateDate { get; set; }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi();
            }
        }
    }
}
