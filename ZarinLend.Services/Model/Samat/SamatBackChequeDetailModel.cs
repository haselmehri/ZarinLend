using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model
{
    public class SamatBackChequeDetailModel
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string BackDate { get; set; }
        public string Date { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchDescription { get; set; }
        public string Number { get; set; }
    }
}
