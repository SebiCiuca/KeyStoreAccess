import * as nativeService from "./Services/nativeService.js"
import * as authorizeService from "./Services/authorizeService.js"
import * as certificateService from "./Services/certificateService.js"
import * as storage from "./storage.js"
import * as api from "./api.js"

//Buttons
const btnAuthorize = document.getElementById("btnAuthorize");
const btnSyncCertificates = document.getElementById("btnSyncCertificates");

btnAuthorize.addEventListener("click", handleAuthorize);
btnSyncCertificates.addEventListener("click", syncCertificates);

var btnAuth = $("#btnAuthorize");
var btnSync = $("#btnSyncCertificates");


storage.deleteSessionKey();

async function handleAuthorize() {
    console.log("1.Getting user from OS");
    await nativeService.getLoggedUser();
    authorizeService.authorizeUserAsync();
    writeAlertMessage("Sucess!","Authorize succeded!");
    btnAuth.removeClass("d-block").addClass("d-none");
    btnSync.removeClass("d-none").addClass("d-block");
}

function syncCertificates(){
    var result  = certificateService.syncCertificates();
    writeAlertMessage("Sync done!",result);
}

// async function getLogs(){
//     //nativeService.getLogs();
//     const certs = await api.getCerts();
//     const response = await nativeService.syncCertificates(certs);
// }

function writeAlertMessage(strong,message)
{
    if(strong){
        $('#alert-strong-message').html(strong);
    }

    $('#alert-message').html(message);
    
    var mainAlert = $("#mainAlert");
    mainAlert.addClass("show");
}
