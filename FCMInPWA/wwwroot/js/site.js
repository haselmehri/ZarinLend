"use strict";
import { initializeApp } from "https://www.gstatic.com/firebasejs/11.1.0/firebase-app.js";
import { getMessaging, getToken } from 'https://www.gstatic.com/firebasejs/11.1.0/firebase-messaging.js';
const firebaseConfig = {
    apiKey: "AIzaSyAkJV6DCx2cj7e9zuSwnQPHnGFmErxUKYA",
    authDomain: "honar-71e36.firebaseapp.com",
    projectId: "honar-71e36",
    storageBucket: "honar-71e36.firebasestorage.app",
    messagingSenderId: "383995737204",
    appId: "1:383995737204:web:cb46ef5925c00d8e63cd21",
    measurementId: "G-SPEKNZFB6W"
};
let installPrompt = null;
const btnInstallApp = $('#btnInstallApp');
//const firebaseConfig = {
//    apiKey: "AIzaSyAadFhrMTSwxiZCKLJx4cH71z4-2ud0QmU",//"your_api_key", - from google-service.json
//    authDomain: "honar-71e36.firebaseapp.com",//from google-service.json
//    projectId: "honar-71e36",// "your_project_id",
//    storageBucket: "honar-71e36.firebasestorage.app",// "your_storage_bucket",
//    messagingSenderId: "383995737204",// "your_messaging_sender_id",
//    appId: "1:383995737204:android:e9d7c774351be2e963cd21",// "your_app_id",
//    measurementId: "G-SPEKNZFB6W"
//};

debugger;
// Initialize Firebase
const app = initializeApp(firebaseConfig);
const messaging = getMessaging(app);
//alert(Notification.permission);
//Notification.requestPermission()
//    .then(() => {
//        debugger;
//        console.log('Notification permission granted.');
//        return messaging.getToken();
//    })
//    .then((token) => {
//        debugger;
//        console.log('FCM Token:', token);
//        // Send the token to your server
//        sendTokenToServer(token);
//    })
//    .catch((err) => {
//        debugger;
//        console.error('Unable to get permission to notify.', err);
//    });
function requestNotificationAccess() {
    Notification.requestPermission()
        .then((permission) => {
            debugger;
            if (permission === 'granted') {
                console.log('Notification permission granted.');
                // Now you can get the FCM token
                getFcmToken();
            }
            else if (permission === 'denied') {
                console.log('Notification permission denied.');
            }
            else {
                console.log('Notification permission default.');
            }
        })
        .catch((err) => {
            debugger;
            console.error('Unable to get permission to notify.', err);
        });
}
function getFcmToken() {
    getToken(messaging, { vapidKey: 'BNeL-62ri6lipAXxi3CtRxvtWEZTFa8ZLftRoV8aLcW9ObWS5Y9vlLQiiCMoMoJmJUlv8vVvOKdnPmtJQn5raT4' }) // You need a VAPID key here
        .then((token) => {
            if (token) {
                console.log('FCM Token:', token);
                sendTokenToServer(token);
                // Send the token to your server for further use (to send notifications)
            } else {
                console.log('No registration token available. Request permission to generate one.');
            }
        })
        .catch((err) => {
            console.error('An error occurred while retrieving the token. ', err);
        });
}

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

//Service Workers supported?
if ('serviceWorker' in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/firebase-messaging-sw.js")
            .then((reg) => {
                debugger;
                if (Notification.permission === "granted") {
                    $("#form").show();
                    //$("#giveNotificationAccess").show();
                    //getSubscription(reg);
                } else if (Notification.permission === "denied") {
                    //unSubscription(reg);
                    //$("#giveNotificationAccess").show();
                    //$("#NoSupport").show();
                } else {
                    $("#giveNotificationAccess").show();
                    $("#PromptForAccessBtn").on('click', () => requestNotificationAccess(reg));
                }
            });
    });
} else {
    $("#NoSupport").show();
}

window.addEventListener("beforeinstallprompt", (event) => {
    alert("beforeinstallprompt");
    debugger;
    event.preventDefault();
    installPrompt = event;
    $('#installAppBox').show();
});

btnInstallApp.on('click', async (event) => {
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