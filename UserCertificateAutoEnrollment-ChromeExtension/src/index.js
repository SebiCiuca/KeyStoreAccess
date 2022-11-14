import * as nativeService from "./Services/nativeService.js"
import * as authorizeService from "./Services/authorizeService.js"
import * as certificateService from "./Services/certificateService.js"
import * as storage from "./storage.js"
import * as api from "./api.js"

//Buttons
const btnAuthorize = document.getElementById("btnAuthorize");
const btnSyncCertificates = document.getElementById("btnSyncCertificates");
const btnGetLogs = document.getElementById("btnGetLogs");
const logs = document.querySelector("#logs-area");
const error = document.querySelector("#error");

btnAuthorize.addEventListener("click", handleAuthorize);
btnSyncCertificates.addEventListener("click", syncCertificates);
btnGetLogs.addEventListener("click", getLogs);

storage.deleteSessionKey();
logs.value = "";
error.style.display = "none";
btnSyncCertificates.style.display = "none";
btnGetLogs.style.display = "block";
btnAuthorize.style.display = "block";

async function handleAuthorize() {
    console.log("1.Getting user from OS");
    await nativeService.getLoggedUser();
    console.log("9. User retrieved..., authorizing");
    authorizeService.authorizeUserAsync();
    console.log("User authorized");
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
