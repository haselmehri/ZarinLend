using System;

namespace ZarinLend.Services.Model.AyandehSign;

public class AyandehSignGetDataModel
{
    public bool Succeeded { get; set; }
    public string Certificate { get; set; }
    public string SerialNumber { get; set; }
    public string Digest { get; set; }
    public string Signature { get; set; }
    public int SignType { get; set; }
    public string Metadata { get; set; }
    public DateTime SendTime { get; set; }
    public DateTime SignTime { get; set; }
    public string SenderIP { get; set; }
    public string IssuerDn { get; set; }
    public string SubjectDn { get; set; }
    public string SignerCn { get; set; }
    public string Thumbprint { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }

}

