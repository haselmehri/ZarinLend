using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PushNotificationInPWA.Models;
using PushNotificationInPWA.Service;
using PushNotificationInPWA3.Models;
using Services;
using System.Diagnostics;
using WebPush;

namespace PushNotificationInPWA3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;
        private readonly IDbHelper dbHelper;
        private readonly ISmsService smsService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IDbHelper dbHelper, ISmsService smsService)
        {
            _logger = logger;
            this.configuration = configuration;
            this.dbHelper = dbHelper;
            this.smsService = smsService;
        }

        public IActionResult Index()
        {

            ViewBag.applicationServerKey = configuration["VAPID:publicKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string client, string endpoint, string p256dh, string auth)
        {
            if (client == null)
            {
                return BadRequest("No Client Name parsed.");
            }
            if (await dbHelper.ExistClient(client))
            {
                return BadRequest("Client Name already used.");
            }
            var result = await dbHelper.SaveDevice(client, endpoint, p256dh, auth);
            //var subscription = new PushSubscription(endpoint, p256dh, auth);
            //PersistentStorage.SaveSubscription(client, subscription);
            return View("Notify", await dbHelper.GetClients());
        }

        public IActionResult RecordVideo()
        {
            return View();
        }

        public async Task<IActionResult> Notify()
        {
            return View(await dbHelper.GetClients());
        }

        public async Task<IActionResult> NotifyByFCM()
        {
            return View(await dbHelper.GetClients());
        }

        [HttpPost]
        public async Task<IActionResult> Notify(string message, string client)
        {
            if (client == null)
            {
                return BadRequest("No Client Name parsed.");
            }
            PushSubscription subscription = await dbHelper.GetClient(client);
            if (subscription == null)
            {
                return BadRequest("Client was not found");
            }

            var subject = configuration["VAPID:subject"];
            var publicKey = configuration["VAPID:publicKey"];
            var privateKey = configuration["VAPID:privateKey"];

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

            var payload = JsonConvert.SerializeObject(
                new
                {
                    Title = "پیام تستی 1",
                    Body = message,
                    IconUrl = "/images/close.png",
                    ImageUrl = "/images/backgound.png",
                    Url = "https://lendtest.mypmo.ir/Home/Notify"
                });

            using (var webPushClient = new WebPushClient())
            {
                try
                {
                    await webPushClient.SendNotificationAsync(subscription, payload, vapidDetails);
                }
                catch (Exception exception)
                {
                    // Log error
                }
            }

            return View(await dbHelper.GetClients());
        }

        public async Task<IActionResult> SendSms()
        {
            return View("ReadOtpSms", new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> SendSms(string mobile, CancellationToken cancellationToken)
        {
            await smsService.SendOtp(mobile, new Random().Next(10000, 99999).ToString(), "lendtest.mypmo.ir", cancellationToken);

            return View("ReadOtpSms",
                new LoginModel()
                {
                    Mobile = mobile,
                    // = otp
                });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}