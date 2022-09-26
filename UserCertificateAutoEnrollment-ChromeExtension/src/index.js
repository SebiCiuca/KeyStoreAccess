import * as nativeService from "./Services/nativeService.js"
import * as authorizeService from "./Services/authorizeService.js"

//Buttons
const btnAuthorize = document.getElementById("btnAuthorize");
const btnSyncCertificates = document.getElementById("btnSyncCertificates");

btnAuthorize.addEventListener("click", handleAuthorize);
btnSyncCertificates.addEventListener("click", syncCertificates);

function handleAuthorize() {
    authorizeService.authorizeUserAsync();
}

function syncCertificates(){
    nativeService.getCertificates();
}
