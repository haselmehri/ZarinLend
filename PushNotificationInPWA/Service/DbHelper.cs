using Microsoft.Data.SqlClient;
using System.Data;
using WebPush;

namespace PushNotificationInPWA.Service
{
    public class DbHelper : IDbHelper
    {
        private static string? ConnectionString;
        public DbHelper(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("Db");
        }
        public async Task<bool> SaveDevice(string client, string endpoint, string p256dh, string auth)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO [dbo].[Devices]
                                          ([Client]
                                          ,[Endpoint]
                                          ,[p256dh]
                                          ,[auth])
                                    VALUES
                                          (@Client,
                                           @endpoint,
                                           @p256dh,
                                           @auth)";
                cmd.Parameters.AddWithValue("@Client", client);
                cmd.Parameters.AddWithValue("@endpoint", endpoint);
                cmd.Parameters.AddWithValue("@p256dh", p256dh);
                cmd.Parameters.AddWithValue("@auth", auth);

                await con.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> SaveToken(string token)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO [dbo].[FcmTokens]
                                          ([Token])
                                    VALUES
                                          (@Token)";
                cmd.Parameters.AddWithValue("@Token", token);

                await con.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<PushSubscription?> GetClient(string client)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("Select * from [dbo].[Devices] Where Client = @Client", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@Client", client);
                DataTable dt = new DataTable();
                da.Fill(dt);

                await con.OpenAsync();
                if (dt != null && dt.Rows.Count > 0)
                    return new PushSubscription()
                    {
                        Endpoint= dt.Rows[0]["Endpoint"].ToString(),
                        P256DH = dt.Rows[0]["p256dh"].ToString(),
                        Auth = dt.Rows[0]["Auth"].ToString(),                        
                    };

                return null;
            }
        }
        public async Task<bool> ExistClient(string client)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("Select * from [dbo].[Devices] Where Client = @Client", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@Client", client);
                DataTable dt = new DataTable();
                da.Fill(dt);

                await con.OpenAsync();
                return dt != null && dt.Rows.Count > 0;
            }
        }

        public async Task<List<string>> GetClients()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("Select Client from [dbo].[Devices]", con))
            {
                await con.OpenAsync();
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                    return dt.AsEnumerable().Select(p => p["Client"].ToString()).ToList();

                return null;
            }
        }
    }
}
