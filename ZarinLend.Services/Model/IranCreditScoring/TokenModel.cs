using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model.IranCreditScoring
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ApiKey { get; set; }
    }
}
