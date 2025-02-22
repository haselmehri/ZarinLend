self.addEventListener('fetch', function (event) { });
self.addEventListener('push', function (e) {
    let title = "زرین لند";
    let body = "یک پیام جدید دارید!"
    let imageUrl = null;
    let iconUrl = "/images/favicon48x48.png";
    let url = "https://lend.mypmo.ir/";
    if (e.data && e.data.json() != undefined) {
        let jsonData = e.data.json();

        if (jsonData.Body != undefined && jsonData.Body != '')
            body = jsonData.Body;

        if (jsonData.Title != undefined && jsonData.Title != '')
            title = jsonData.Title;

        if (jsonData.ImageUrl != undefined && jsonData.ImageUrl != '')
            imageUrl = jsonData.ImageUrl;

        if (jsonData.IconUrl != undefined && jsonData.IconUrl != '')
            iconUrl = jsonData.IconUrl;

        if (jsonData.Url != undefined && jsonData.Url != '')
            url = jsonData.Url;
    }

    var options = {
        body: body,
        image: imageUrl,
        icon: iconUrl,
        badge: iconUrl,
        vibrate: [100, 50, 100],
        dir: 'rtl',
        //renotify: true,
        data: {
            dateOfArrival: Date.now(),
            url: url
        },
        actions: [
            {
                action: "explore",
                title: "مشاهده",
                icon: "/images/accept.png"
            },
            {
                action: "close",
                title: "بستن",
                icon: "/images/close.png"
            },
        ]
    };
    e.waitUntil(
        self.registration.showNotification(title, options)
    );
});
self.addEventListener('notificationclick', function (e) {
    var notification = e.notification;
    var action = e.action;
    debugger;
    if (action === 'close') {
        notification.close();
    } else {
        // Some actions
        notification.close();
        clients.openWindow(e.notification.data.url);
        //notification.close();
    }
});
