(()=>{"use strict";var e=chrome.runtime.connectNative("com.allianz.usercertificateautoenrollment");e.onMessage.addListener((function(e){console.log("Recieved message from native app: "+JSON.stringify(e))})),e.onDisconnect.addListener((function(){console.log(chrome.runtime.lastError),console.log("disconnected from native app.")}));const n=[{Id:1,Description:"Command to get local certificates"},{Id:2,Description:"Send this with the PFX file to update certificates"},{Id:3,Description:"Send this with a ThumbprintID to make a certificate as auth certificate"}],t=document.getElementById("btnAuthorize"),o=document.getElementById("btnSyncCertificates");t.addEventListener("click",(function(){(async()=>{await(async()=>!0)(),(async e=>{new Promise(((e,n)=>{var t={SessionKey:"valueFromlogin"};chrome.storage.local.set(t,(function(){console.log("Saving session key "+t.SessionKey),e(!0)}))}))})()})()})),o.addEventListener("click",(function(){(async()=>{var t=n[0];console.log("Sending commnad "+t.Id);var o=function(n,t){var o={CommandId:n,CommandValue:""};console.log("Sending message to native app: "+JSON.stringify(o)),console.log("Port: "+JSON.stringify(e)),e.postMessage(o),console.log("Sent message to native app: "+JSON.stringify(o))}(t.Id);console.log(o)})()}))})();