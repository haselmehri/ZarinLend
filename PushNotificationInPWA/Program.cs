using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using PushNotificationInPWA.Extensions;
using PushNotificationInPWA.Service;
using Services;
using WebEssentials.AspNetCore.Pwa;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IDbHelper, DbHelper>();
builder.Services.AddSingleton<ISmsService, SmsService>();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(builder.Environment.WebRootPath, "honar-71e36-firebase-adminsdk-g2x7v-d50cacd3f1.json")),
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddProgressiveWebApp(
    new PwaOptions
    {        
        RoutesToPreCache = "/, /home/index, /home/notify",
        Strategy = ServiceWorkerStrategy.CacheFirst
    });

var app = builder.Build();
// Initialize Firebase Admin SDK
//builder.Services.FirebaseConfig(app.Environment);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//first handle any websocket requests
app.UseWebSockets();
//app.UseWebSocketHandler();

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
