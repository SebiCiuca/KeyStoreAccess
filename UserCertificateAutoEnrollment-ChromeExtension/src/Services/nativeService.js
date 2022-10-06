
import * as storage from "../storage.js"

//Native communcation
var usercertificateautoenrollmentHost = chrome.runtime.connectNative('com.allianz.usercertificateautoenrollment');

//usercertificateautoenrollmentHost.onMessage.addListener(onNativeMessage);
usercertificateautoenrollmentHost.onDisconnect.addListener(onDisconnected);

var commandInProgress = -1;

const commands = [
     { Id: 1, Description: "Command to get local certificates" },
     { Id: 2, Description: "Send this with the PFX file to update certificates" },
     { Id: 3, Description: "Send this with a ThumbprintID to make a certificate as auth certificate" },
     { Id: 4, Description: "Get current domain user" },
     { Id: 5, Description: "Get session logs" },
]

export const getCertificates = async () => {
     var command = commands[0];
     console.log("Sending commnad " + command.Id);
     await sendNativeMessageV2(command.Id);
}

export const getLoggedUser = async () => {
     var command = commands[3];
     console.log("2. Sending command " + command.Id);
     await sendNativeMessageV2(command.Id);
     console.log("8. " + commandInProgress);
}

export const getLogs = async () => {
     var command = commands[4];
     var logLevel = "5";
     await sendNativeMessageV2(command.Id);
     
}

// function sendNativeMessage(command, commandValue) {
//      commandInProgress = command;
//      var message =
//      {
//           "CommandId": command,
//           "CommandValue": ""
//      }
//      console.log('Sending message to native app: ' + JSON.stringify(message));
//      console.log('Port: ' + JSON.stringify(usercertificateautoenrollmentHost));
//      usercertificateautoenrollmentHost.postMessage(message);
//      console.log('Sent message to native app: ' + JSON.stringify(message));
// }

async function sendNativeMessageV2(command, commandValue) {
     commandInProgress = command;
     var sessionKey = await storage.getSessionKey();

     var message =
     {
          "CommandId": command,
          "CommandValue": commandValue,
          "SessionKey": sessionKey
     }
     console.log('3. Sending message to native app: ' + JSON.stringify(message));

     return new Promise((resolve, reject) => {
          chrome.runtime.sendNativeMessage('com.allianz.usercertificateautoenrollment', message,
               async function (result) {
                    console.log("4. Response triggerd...");
                    var response = await onNativeMessage(result);
                    resolve(response);
               }
          );
     });
}

async function onNativeMessage(message) {
     console.log('5. Recieved message from native app: ' + JSON.stringify(message));
     console.log('6. Command before ' + commandInProgress);

     switch (commandInProgress) {
          case 1:
               // code block
               break;
          case 2:
               // code block
               break;
          case 3:
               // code block
               break;
          case 4:
               await SaveLoggedInUser(message);
               break;
          case 5:
               await UploadLogs(message);
               break;
          default:
          // code block
     }

     commandInProgress = -1;

     return true;
}

function onDisconnected() {
     console.log(chrome.runtime.lastError);
     console.log('disconnected from native app.');
     usercertificateautoenrollmentHost = null;
}

async function SaveLoggedInUser(message) {
     await storage.saveLoggedInUser(message.data);
}

async function UploadLogs(message){
     console.log(message);
}