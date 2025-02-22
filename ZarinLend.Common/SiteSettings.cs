namespace Common
{
    public class SiteSettings
    {
        public string Domain { get; set; }
        public string ZarinPalCallBackUrl { get; set; }
        public string FinotechAuthorizationCodeRedirectUrl { get; set; }
        public string ElmahPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public MailSettings MailSettings { get; set; }
        public SigningSettings SigningSettings { get; set; }
        public VAPID VAPID { get; set; }
    }

    public class VAPID
    {
        public string subject { get; set; }
        public string publicKey { get; set; }
        public string privateKey { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }
    }
    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumic { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
        public bool RequireConfirmedEmail { get; set; }
        public bool RequireConfirmedPhoneNumber { get; set; }
    }

    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
    }
    public class SigningSettings
    {
        public string AyandehSignCallback { get; set; }
    }
}
