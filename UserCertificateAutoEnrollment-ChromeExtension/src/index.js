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
var spinner = $("#spinner");
var data = $("#data")

storage.deleteSessionKey();
hideSpinner();
window.onload = onWindowLoad;

async function handleAuthorize() {
    // showSpinner();
    // console.log("1.Getting user from OS");
    // await nativeService.getLoggedUser();
    // authorizeService.authorizeUserAsync();
    // writeAlertMessage("Sucess!","Authorize succeded!");
    // btnAuth.removeClass("d-block").addClass("d-none");
    // btnSync.removeClass("d-none").addClass("d-block");
    // hideSpinner();
    onWindowLoad();
}

async function pingClienApi() {
    console.log("1. Seding ping");
    await nativeService.pingHost();
    console.log("Ping finished");
}

async function syncCertificates() {
    showSpinner();
    var result = await certificateService.syncCertificates();
    writeAlertMessage("Sync done!", result);

    await getLogs();
    hideSpinner();
}

async function getLogs() {
    nativeService.getLogs();
}

function writeAlertMessage(strong, message) {
    if (strong) {
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

function onWindowLoad() {

    chrome.tabs.
        //search for active tab
        query({ active: true, currentWindow: true })
        //find active tab
        .then(function (tabs) {
            var activeTab = tabs[0];
            var activeTabId = activeTab.id;
            //retrieve activeTab DOM object and filter it
            return chrome.scripting.executeScript({
                target: { tabId: activeTabId },
                // injectImmediately: true, 
                // uncomment this to make it execute straight away, other wise it will wait for document_idle
                func: ParseDOM,
                args: ['#syntax', true]
            });
        })
        //parse the filtered DOM object of current opened tab
        .then(function (results) {
            handleCurrentPage(results[0].result);
        })
        .catch(function (error) {
            console.log('There was an error injecting script : \n' + error.message);
        });
}

function ParseDOM(selector, inner) {
    //narrow the search in the hole DOM
    if (selector) {

        selector = document.querySelector(selector);
        if (!selector) {
            return "ERROR: querySelector failed to find node";
        }
        //in case no query is specified in args, return the hole DOM object     
    } else {
        selector = document.documentElement;
    }

    //in case we just want the innerHTML value of an id
    if (inner) {
        return selector.innerHTML;
    }
    else {
        return selector.outerHTML;
    }
}

function handleCurrentPage(content) {
    //in case the the search for our handlerId is not found reopen in a new tab the loginNoPrompt page
    if(content == "ERROR: querySelector failed to find node"){
        authorizeService.openLoginPage();
    }

    //save handled
    var isLoggedIn = authorizeService.isUserLoggedIn(content);

    //if handler has not been saved successfully reopen the loginNoPrompt page
    if (!isLoggedIn) {
        authorizeService.openLoginPage();
    }
}

