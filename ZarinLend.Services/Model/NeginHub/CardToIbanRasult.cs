using System;
using System.Collections.Generic;

namespace Services.Model.NeginHub
{
    [Serializable]
    public class CardToIbanRasult : NeginHubBaseResult
    {
        public string Deposit { get; set; }
        public string BankName { get; set; }
        public List<DepositOwner> DepositOwners { get; set; }
        public string IBAN { get; set; }
        public string Card { get; set; }
        /// <summary>
        /// 02 : حساب فعال است
        /// ‌03 : حساب مسدود با قابلیت واریز
        /// ‌04 : حساب مسدود بدون قابلیت واریز
        /// ‌05 : حساب راکد است
        /// ‌06 : بروز خطادر پاسخ دهی, شرح خطا در فیلد توضیحات است
        /// ‌07 : سایر موارد
        /// </summary>
        public string DepositStatus { get; set; }

    }

    [Serializable]
    public class DepositOwner
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
