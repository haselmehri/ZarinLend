namespace ZarinLend.Services.Model.AyandehSign;

public class GetSigningTokenResult
{
    public bool IsSuccess { get; set; }
    public string SigningToken { get; set; }
    public  string ErrorMessage { get; set; }
}
