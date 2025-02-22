using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model
{
    public class UserEditResult
    {
        public bool MobileChanged { get; set; }
        public bool EmailChanged { get; set; }
        public bool UsernameChanged { get; set; }
        public bool NationalCodeChanged { get; set; }
        public bool EnsureSignOut { get; set; }
        public string Token { get; set; }

    }
}
