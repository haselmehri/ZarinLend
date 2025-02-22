namespace Services.Model.WalletTransaction
{
    public class WalletBalanceBaseCardNumber
    {
        public long Balance { 
            get
            {
                return Deposit - Withdraw;
            }
        }
        public long Deposit { get; set; }
        public long Withdraw { get; set; }
        public string CardNumber { get; set; }
        public int RequestFacilityId { get; set; }
    }
}
