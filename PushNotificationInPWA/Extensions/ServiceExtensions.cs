using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace PushNotificationInPWA.Extensions
{
    static public class ServiceExtensions
    {
        public static void FirebaseConfig(this IServiceCollection services, IWebHostEnvironment env)
        {
            FirebaseApp.Create(new AppOptions()
            {
                //Credential = GoogleCredential.FromFile(Path.Combine(env.WebRootPath, "google-services.json")),
                Credential = GoogleCredential.FromJson("{\r\n    apiKey: \"AIzaSyAadFhrMTSwxiZCKLJx4cH71z4-2ud0QmU\",//\"your_api_key\", - from google-service.json\r\n    authDomain: \"honar-71e36.firebaseapp.com\",//from google-service.json\r\n    projectId: \"honar-71e36\",// \"your_project_id\",\r\n    storageBucket: \"honar-71e36.firebasestorage.app\",// \"your_storage_bucket\",\r\n    messagingSenderId: \"383995737204\",// \"your_messaging_sender_id\",\r\n    appId: \"1:383995737204:android:e9d7c774351be2e963cd21\",// \"your_app_id\",\r\n    measurementId: \"G-SPEKNZFB6W\"\r\n}"),
            });
        }
    }
}
