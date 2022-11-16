import * as nativeService from "./Services/nativeService.js"
import * as authorizeService from "./Services/authorizeService.js"
import * as certificateService from "./Services/certificateService.js"
import * as storage from "./storage.js"
import * as api from "./api.js"

//Buttons
const btnAuthorize = $("#btnAuthorize");
const btnSyncCertificates = $("#btnSyncCertificates");
// const btnGetLogs = document.getElementById("btnGetLogs");
// const logs = document.querySelector("#logs-area");
// const error = document.querySelector("#error");

btnAuthorize.click(handleAuthorize);
btnSyncCertificates.click(syncCertificates);
// btnGetLogs.addEventListener("click", getLogs);

storage.deleteSessionKey();
// logs.value = "";
// error.style.display = "none";
// btnSyncCertificates.style.display = "none";
// btnGetLogs.style.display = "none";
// btnAuthorize.style.display = "block";

async function handleAuthorize() {
    console.log("1.Getting user from OS");
    await nativeService.getLoggedUser();
    console.log("9. User retrieved..., authorizing");
    authorizeService.authorizeUserAsync();
    console.log("User authorized");
    writeAlertMessage("Sucess!","Authorize succeded!");
    btnAuthorize.removeClass("d-block").addClass("d-none");
    btnSyncCertificates.removeClass("d-none").addClass("d-block");
    btnSyncCertificates.style.display = "block";
}

function syncCertificates(){
    certificateService.syncCertificates();
    btnGetLogs.style.display = "block";
}

async function getLogs(){
    //nativeService.getLogs();
    const certs = await api.getCerts();
    const response = await nativeService.syncCertificates(certs);
}

function writeAlertMessage(strong,message)
{
    if(strong){
        $('#alert-strong-message').html(strong);
    }

    $('#alert-message').html(message);
    
    $('.alert').alert();
}
