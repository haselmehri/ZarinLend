self.addEventListener('fetch', function (event) { });

let url = "https://lendtest.mypmo.ir/";
self.addEventListener('push', function (e) {
    let title = "زرین لند";
    let body = "یک پیام جدید دارید!"

    let imageUrl = "/images/accept.png";
    let iconUrl = "/images/icon-48x48.png";
    if (e.data && e.data.json() != undefined) {
        let jsonData = e.data.json();

        if (jsonData.Body != undefined && jsonData.Body != '')
            body = jsonData.Body;

        if (jsonData.Title != undefined && jsonData.title != '')
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
        badge: "/pwa/screenshots/1jpg",
        vibrate: [100, 50, 100],
        dir: 'rtl',
        url: url,
        data: {
            dateOfArrival: Date.now()
        },
        actions: [
            {
                action: "explore",
                title: "مشاهده",
                icon: "images/accept.png"
            },
            {
                action: "close",
                title: "بستن",
                icon: "images/close.png"                
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
        clients.openWindow(url);
    }
});
