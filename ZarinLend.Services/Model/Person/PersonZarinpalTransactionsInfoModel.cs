using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Model
{
    [Serializable]
    public class PersonZarinpalTransactionsInfoModel
    {

        public string CardPan { get; set; }

        public string CardHash { get; set; }

        public int NumnerOfTerminals { get; set; }

        [Display(Name ="تعداد تراکنش ها")]
        public int NumberOfTransactions { get; set; }

        [Display(Name = "مبلغ تراکنش ها")]
        public double SumAmount { get; set; }
    }
}
