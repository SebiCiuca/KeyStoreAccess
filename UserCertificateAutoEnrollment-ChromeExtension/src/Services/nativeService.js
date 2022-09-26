//Native communcation
var usercertificateautoenrollmentHost = chrome.runtime.connectNative('com.allianz.usercertificateautoenrollment');

usercertificateautoenrollmentHost.onMessage.addListener(onNativeMessage);
usercertificateautoenrollmentHost.onDisconnect.addListener(onDisconnected);

const commands = [
     { Id: 1, Description: "Command to get local certificates" },
     { Id: 2, Description: "Send this with the PFX file to update certificates" },
     { Id: 3, Description: "Send this with a ThumbprintID to make a certificate as auth certificate" }
 ]

export const getCertificates = async() => {
     var command = commands[0];
     console.log("Sending commnad " + command.Id);
     var certificates = sendNativeMessage(command.Id);
     console.log(certificates);
}

function sendNativeMessage(command, commandValue) {
     var message =
     {
          "CommandId": command,
          "CommandValue": ""
     }  
     console.log('Sending message to native app: ' + JSON.stringify(message));
     console.log('Port: ' + JSON.stringify(usercertificateautoenrollmentHost));
     usercertificateautoenrollmentHost.postMessage(message);
     console.log('Sent message to native app: ' + JSON.stringify(message));
}

function onNativeMessage(message) {
     console.log('Recieved message from native app: ' + JSON.stringify(message));
}

function onDisconnected() {
     console.log(chrome.runtime.lastError);
     console.log('disconnected from native app.');
     //usercertificateautoenrollmentHost = null;
}