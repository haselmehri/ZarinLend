namespace ZarinLend.Services.Model.Ayandeh.BankAccount;

public class DepositToIbanResponseModel
{
    public Data Data { get; set; }
    public MetaData Meta { get; set; }
}

public class Data 
{
    public string Deposit { get; set; }
    public string AccountStatus { get; set; }
    public string BankName { get; set; }
    public string Iban { get; set; }
    public string DepositOwners { get; set; }
}

public class MetaData
{
    public bool IsSuccess { get; set; }
    public string ErrorType { get; set; }
    public string ErrorMessage { get; set; }
    public string Message { get; set; }
    public int Code { get; set; }
    public object Errors { get; set; }
}