using System;
using System.Collections.Generic;

namespace Services.Model.ZarinPal
{
    [Serializable]
    public class ZarinpalUserData
    {
        public RootField Data { get; set; } 
    }

    [Serializable]
    public class RootField
    {
        /// <summary>
        /// identity inof
        /// </summary>
        public ZarinPalUserInfo Me { get; set; }

        /// <summary>
        /// address list
        /// </summary>
        public List<ZarinPalUserAddress> Addresses { get; set; }
    }

    [Serializable]
    public class ZarinPalUserInfo
    {
        public string ws_id { get; set; }

        /// <summary>
        /// zp id
        /// </summary>
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string full_name { get; set; }

        /// <summary>
        /// mobile
        /// </summary>
        public string cell_number { get; set; }
        public string gender { get; set; }

        /// <summary>
        /// nationalCode
        /// </summary>
        public string ssn { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
    }

    [Serializable]
    public class ZarinPalUserAddress
    {
        /// <summary>
        /// address type such as Home,Work....
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// tilte
        /// </summary>
        public string title { get; set; }
        public string address { get; set; }

        /// <summary>
        /// telphone
        /// </summary>
        public string landline { get; set; }
        public string postal_code { get; set; }
        public bool is_postal_code_verified { get; set; }
    }
}
