namespace Services.Model
{
    public class VerifyTransactionModel
    {
        public int ResultCode { get; set; }
        public string ResultDescription { get; set; }
        public bool Success { get; set; }
        public VerifyTransactionDetailModel TransactionDetail { get; set; }
    }

    public class VerifyTransactionDetailModel
    {
        public string RRN { get; set; }
        public string RefNum { get; set; }
        public string MaskedPan { get; set; }
        public string HashedPan { get; set; }
        public int TerminalNumber { get; set; }
        public long OrginalAmount { get; set; }
        public long AffectiveAmount { get; set; }
        public string StraceDate { get; set; }
        public string StraceNo { get; set; }
    }
}
