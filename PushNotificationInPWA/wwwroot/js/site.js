"use strict";
let installPrompt = null;
const btnInstallApp = $('#btnInstallApp');

window.addEventListener("beforeinstallprompt", (event) => {
    alert("beforeinstallprompt");
    debugger;
    event.preventDefault();
    installPrompt = event;
    $('#installAppBox').show();
});

btnInstallApp.click(async (event) => {
    event.preventDefault();
    alert("install app button click");
    if (!installPrompt) {
        return;
    }
    const result = await installPrompt.prompt();
    console.log(`Install prompt was: ${result.outcome}`);
    disableInAppInstallPrompt();
});

window.addEventListener("appinstalled", () => {
    alert("appinstalled");
    disableInAppInstallPrompt();
});

function disableInAppInstallPrompt() {
    installPrompt = null;
    $('#installAppBox').hide();
}
//====================================================
//Service Workers supported?
if ('serviceWorker' in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/firebase-messaging-sw.js")
            .then((reg) => {
                debugger;
                if (Notification.permission === "granted") {
                    $("#form").show();
                    getSubscription(reg);
                } else if (Notification.permission === "blocked") {
                    unSubscription(reg);
                    $("#NoSupport").show();
                } else {
                    $("#giveNotificationAccess").show();
                    $("#PromptForAccessBtn").on('click', () => requestNotificationAccess(reg));
                }
            });
    });
} else {
    $("#NoSupport").show();
}

const firebaseConfig = {
    apiKey: "AIzaSyAadFhrMTSwxiZCKLJx4cH71z4-2ud0QmU",//"your_api_key", - from google-service.json
    authDomain: "honar-71e36.firebaseapp.com",//from google-service.json
    projectId: "honar-71e36",// "your_project_id",
    storageBucket: "honar-71e36.firebasestorage.app",// "your_storage_bucket",
    messagingSenderId: "383995737204",// "your_messaging_sender_id",
    appId: "1:383995737204:android:e9d7c774351be2e963cd21",// "your_app_id",
    measurementId: "G-SPEKNZFB6W"
};

const app = firebase.initializeApp(firebaseConfig);
const messaging = firebase.messaging();
debugger;
messaging.requestPermission()
    .then(() => {
        debugger;
        console.log('Notification permission granted.');
        return messaging.getToken();
    })
    .then((token) => {
        debugger;
        console.log('FCM Token:', token);
        // Send the token to your server
        sendTokenToServer(token);
    })
    .catch((err) => {
        debugger;
        console.error('Unable to get permission to notify.', err);
    });

function sendTokenToServer(token) {
    fetch('/api/fcm/savetoken', {
        method: 'POST',
        body: JSON.stringify({ token: token }),
        headers: {
            'Content-Type': 'application/json'
        }
    }).then(response => {
        debugger;
        console.log('Token sent to server');
    }).catch(err => {
        console.log('Error sending token to server', err);
    });
}

//function requestNotificationAccess(reg) {
//    Notification.requestPermission(function (status) {
//        alert(status);
//        $("#giveNotificationAccess").hide();
//        if (status == "granted") {
//            $("#form").show();
//            debugger;
//            getSubscription(reg);
//        }
//        else if (status == 'denied') {
//            unSubscription(reg);
//        } else {
//            $("#NoSupport").show();
//        }
//    });

//    //messaging.requestPermission()
//    //    .then(() => {
//    //        debugger;
//    //        console.log('Notification permission granted.');
//    //        return messaging.getToken();
//    //    })
//    //    .then((token) => {
//    //        debugger;
//    //        console.log('FCM Token:', token);
//    //        // Send the token to your server
//    //        sendTokenToServer(token);
//    //    })
//    //    .catch((err) => {
//    //        debugger;
//    //        console.error('Unable to get permission to notify.', err);
//    //    });
//}

//function unSubscription(reg) {
//    reg.pushManager.getSubscription().then(function (sub) {
//        if (sub != null) {
//            sub.unsubscribe()
//                .then(function (sub) {
//                    alert(sub.endpoint);
//                }).catch(function (e) {
//                    alert('Unable to unsubscribe to push');
//                    console.error("Unable to unsubscribe to push", e);
//                });
//        }
//    });
//}

//function getSubscription(reg) {
//    reg.pushManager.getSubscription().then(function (sub) {
//        if (sub === null) {
//            reg.pushManager.subscribe({
//                userVisibleOnly: true,
//                applicationServerKey: "BNwmzjuUQJqbl10J_mV5tsXiUB4bK68AhlJKPx-HG_OmcnwLuZkieAbe9SLxE3REQZI1E_-PUZ_EDZ8xcoIoYaE"
//            }).then(function (sub) {
//                fillSubscribeFields(sub);
//            }).catch(function (e) {
//                console.error("Unable to subscribe to push", e);
//            });
//        } else {
//            debugger;
//            fillSubscribeFields(sub);
//        }
//    });
//}

//function fillSubscribeFields(sub) {
//    $("#endpoint").val(sub.endpoint);
//    $("#p256dh").val(arrayBufferToBase64(sub.getKey("p256dh")));
//    $("#auth").val(arrayBufferToBase64(sub.getKey("auth")));
//}

//function arrayBufferToBase64(buffer) {
//    var binary = '';
//    var bytes = new Uint8Array(buffer);
//    var len = bytes.byteLength;
//    for (var i = 0; i < len; i++) {
//        binary += String.fromCharCode(bytes[i]);
//    }
//    return window.btoa(binary);
//}