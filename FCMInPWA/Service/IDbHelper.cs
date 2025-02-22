namespace FCMInPWA.Service
{
    public interface IDbHelper
    {
        Task<bool> SaveToken(string token);
        Task<bool> SaveDevice(string client, string endpoint, string p256dh, string auth);
        Task<bool> ExistClient(string client);
        //Task<PushSubscription?> GetClient(string client);
        Task<List<string?>?> GetClients();
    }
}
