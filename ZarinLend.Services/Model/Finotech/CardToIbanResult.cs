//using System;

//namespace Services.Model
//{
//    [Serializable]
//    public class CardToIbanRasult: FinotechBaseResult
//    {
//        public CardToIbanSuccessRasult Result { get; set; }
//    }

//    public class CardToIbanSuccessRasult
//    {
//        public string Deposit { get; set; }
//        public string BankName { get; set; }
//        public string DepositOwners { get; set; }
//        public string IBAN { get; set; }
//        public string Card { get; set; }
//        /// <summary>
//        /// 02 : حساب فعال است
//        /// ‌03 : حساب مسدود با قابلیت واریز
//        /// ‌04 : حساب مسدود بدون قابلیت واریز
//        /// ‌05 : حساب راکد است
//        /// ‌06 : بروز خطادر پاسخ دهی, شرح خطا در فیلد توضیحات است
//        /// ‌07 : سایر موارد
//        /// </summary>
//        public string DepositStatus { get; set; }

//    }
//}
