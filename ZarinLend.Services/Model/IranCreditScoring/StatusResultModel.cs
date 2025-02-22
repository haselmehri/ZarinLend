using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model.IranCreditScoring
{
   public class StatusResultModel
    {
        public string StatusTitle { get; set; }
        public string Status { get; set; }
        public string ReportLink { get; set; }
        public string OtpMessageStatusTitle { get; set; }
        public int OtpMessageStatus { get; set; }
        public string ReportTryCount { get; set; }
    }
}
