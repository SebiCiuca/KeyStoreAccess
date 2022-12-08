import * as nativeService from "./Services/nativeService.js"
import * as authorizeService from "./Services/authorizeService.js"
import * as certificateService from "./Services/certificateService.js"
import * as storage from "./storage.js"
import * as api from "./api.js"

//Buttons
const btnAuthorize = document.getElementById("btnAuthorize");
const btnSyncCertificates = document.getElementById("btnSyncCertificates");
const btnPing = document.getElementById("btnPing");

btnAuthorize.addEventListener("click", handleAuthorize);
btnSyncCertificates.addEventListener("click", syncCertificates);
btnPing.addEventListener("click", pingClienApi);

var btnAuth = $("#btnAuthorize");
var btnSync = $("#btnSyncCertificates");
var spinner =  $("#spinner");
var data =  $("#data")

storage.deleteSessionKey();
hideSpinner();

async function handleAuthorize() {
    showSpinner();
    console.log("1.Getting user from OS");
    await nativeService.getLoggedUser();
    authorizeService.authorizeUserAsync();
    writeAlertMessage("Sucess!","Authorize succeded!");
    btnAuth.removeClass("d-block").addClass("d-none");
    btnSync.removeClass("d-none").addClass("d-block");
    hideSpinner();
}

async function pingClienApi() {
    console.log("1. Seding ping");
    await nativeService.pingHost();
    console.log("Ping finished");
}

async function syncCertificates(){
    showSpinner();
    var result  = await certificateService.syncCertificates();
    writeAlertMessage("Sync done!",result);

    await getLogs();
    hideSpinner();
}

async function getLogs(){
    nativeService.getLogs();
}

function writeAlertMessage(strong,message)
{
    if(strong){
        $('#alert-strong-message').html(strong);
    }

    $('#alert-message').html(message);
    
    var mainAlert = $("#mainAlert");
    mainAlert.addClass("show");
}

function hideSpinner() {   
    spinner.removeClass("d-block").addClass("d-none");
    data.removeClass("d-none").addClass("d-block");
} 

function showSpinner() {
    spinner.removeClass("d-none").addClass("d-block");
    data.removeClass("d-block").addClass("d-none");
} 

