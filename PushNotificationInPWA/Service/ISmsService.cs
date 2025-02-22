namespace Services
{
    public interface ISmsService
    {
        Task<long> SendOtp(string mobile, string otp, string site, CancellationToken cancellationToken);
    }
}