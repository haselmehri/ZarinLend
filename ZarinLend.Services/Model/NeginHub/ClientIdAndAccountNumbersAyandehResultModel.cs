using System.Collections.Generic;

namespace ZarinLend.Services.Model.NeginHub;

public class ClientIdAndAccountNumbersAyandehResultModel
{
    public ClientData Data { get; set; }
    public MetaData Meta { get; set; }
}
public class ClientData
{
    public string ClientId { get; set; }
    public List<Account> Accounts { get; set; }
}

public class Account
{
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
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