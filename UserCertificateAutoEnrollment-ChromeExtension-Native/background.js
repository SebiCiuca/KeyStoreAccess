var port = chrome.runtime.connectNative('com.allianz.usercertificateautoenrollment');
port.onMessage.addListener(onNativeMessage);
port.onDisconnect.addListener(onDisconnected);
const button1 = document.getElementById("button1");

console.log("Background JS was called");

button1.addEventListener("click", handleClick);
var count = 0;

function handleClick() {
     console.log("handleClick was called");
     count++;
     console.log("Sending command "+ count);
     sendNativeMessage(count);
}

function getUrl(title, url) {
     var o = { title: title, url: url };
     try {
          console.log('seding to c# title and url');
          console.log("Title: " + title);
          console.log("Url: " + url);
          port.postMessage(o);
     }
     catch (err) {

               port = chrome.runtime.connectNative('com.allianz.usercertificateautoenrollment');
               port.postMessage(o);
     }
}

function MyCurrentTab(tab) {
     console.log(tab);
     getUrl(tab.title, tab.url);
}


function sendNativeMessage(command, commandValue) {
     message =
     {
          "CommandId": command,
          "CommandValue": commandValue
     };
     //port = chrome.runtime.connectNative('com.allianz.usercertificateautoenrollment');
     console.log('Sending message to native app: ' + JSON.stringify(message));
     console.log('Port: ' + JSON.stringify(port));
     
     port.postMessage(message);
     console.log('Sent message to native app: ' + message);
}

function onNativeMessage(message) {
     console.log('recieved message from native app: ' + JSON.stringify(message));
}

function onDisconnected() {
     console.log(chrome.runtime.lastError);
     console.log('disconnected from native app.');
     port = null;
}