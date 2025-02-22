using Core.Entities;

namespace Services.Model
{
    public class TokenModel
    {
        public string Token { get; set; }
        public string Expire { get; set; }
        public double ExpireBaseTimeStamp { get; set; }
    }

    public class TokenModeWithUser
    {
        public TokenModel Token { get; set; }
        public User User { get; set; }
    }
}
