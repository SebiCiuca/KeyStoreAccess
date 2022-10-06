import * as nativeService from "./Services/nativeService.js"
import * as authorizeService from "./Services/authorizeService.js"
import * as storage from "./storage.js"

//Buttons
const btnAuthorize = document.getElementById("btnAuthorize");
const btnSyncCertificates = document.getElementById("btnSyncCertificates");
const btnGetLogs = document.getElementById("btnGetLogs");

btnAuthorize.addEventListener("click", handleAuthorize);
btnSyncCertificates.addEventListener("click", syncCertificates);
btnGetLogs.addEventListener("click", getLogs);

storage.deleteSessionKey();

async function handleAuthorize() {
    console.log("1.Getting user from OS");
    await nativeService.getLoggedUser();
    console.log("9. User retrieved..., authorizing");
    authorizeService.authorizeUserAsync();
    console.log("User authorized");
}

function syncCertificates(){
    nativeService.getCertificates();
}

function getLogs(){
    nativeService.getLogs();
}
