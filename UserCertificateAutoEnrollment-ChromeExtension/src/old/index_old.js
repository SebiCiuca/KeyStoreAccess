import * as apiService from "./api.js"
import * as storage from "./storage.js"

const error = document.querySelector("#error");
const logs = document.querySelector("#logs-area");

const sSTTypes = [
    { Name: "AuthRoots", SSTType: "AUTHROOTS" },
    { Name: "Disallowed", SSTType: "DISALLOWEDCERT" },
    { Name: "Delete Disallowed", SSTType: "DELDISALLOWED" },
    { Name: "Interception", SSTType: "INTERCEPTION" },
    { Name: "Roots", SSTType: "ROOTS" },
    { Name: "Delete roots", SSTType: "DELROOTS" },
    { Name: "Update roots", SSTType: "UPDROOTS" },
    { Name: "Trusted publisher", SSTType: "TRUESTEDPUBLISHER" },
    { Name: "Untrusted publisher", SSTType: "UNTRUSTEDPUBLISHER" }
]

error.style.display = "none";
logs.value = "";

// grab the form
const form = document.querySelector(".form-data");

// grab buttons
const getCertficateButton = document.querySelector("#get-certificates");

// declare a function to handle form submission
const handleSubmit = async e => {
    error.style.display = "none";
    e.preventDefault();

    writeLogLine("Retriving session key from local storage...");
    let sessionKey = await storage.getSessionKey();

    if (sessionKey === null) {
        writeLogLine("No session key found locally");
        writeLogLine("Generating new session key");

        await generateNewSession();
    }
    writeSuccessLine("Session key found, retriving weather");
    writeErrorLine("Getting weather");

    await syncAllCertificates();
};

const handleGetCertificates = async e => {
    error.style.display = "none";
    e.preventDefault();

    writeLogLine("Retriving session key from local storage...");

    let sessionKey = await storage.getSessionKey();

    if (sessionKey === null) {
        writeLogLine("No session key found locally");
        writeLogLine("Generating new session key");

        await generateNewSession();
    }

    writeSuccessLine("Session key found, retriving weather");
}

form.addEventListener("submit", e => handleSubmit(e));
getCertficateButton.addEventListener("onclick", e => handleGetCertificates(e));


//#region  Session related function 
async function generateNewSession() {
    try {
        var response = await apiService.generateSessionAsync("mysmallkey123456");
        if (response.data.sessionKey != null) {
            var savedSuccessfully = await storage.saveSessionKey(response.data.sessionKey);
            if (savedSuccessfully) {
                writeLogLine("Successfully generated new session key");
                writeLogLine("Validating session key..");
                await validateSession();
            }
        }
    } catch (error) {
        error.style.display = "block";
        error.textContent = "Could not create session with API, can't load ";
    }

}

const validateSession = async () => {
    let sessionKey = await storage.getSessionKey();
    try {
        const response = await apiService.validateSessionKeyAsync(sessionKey);

        if (response.data) {

            writeLogLine("Session validated successfully!");
        } else {
            writeLogLine("Could not validate session key!");
            writeLogLine("Removing session key from local storage..");
            const response = storage.deleteSessionKey();
            console.log(response);
            writeLogLine("Session key removed, please try again to generate a new session");
        }
    } catch (error) {
        var handled = handleNetwrokError(error);

        if (handled) {
            return;
        }
        handled = handleSessionError(error);
        if (handled) {
            return;
        }

        writeLogLine("Unexpected error");
        error.style.display = "block";
        error.textContent = err.message;
    }
}

//#endregion Session

//#region  Certificates

const getAllCertificatesByType = async (type) => {
    let sessionKey = await storage.getSessionKey();
    try{
    var response = await apiService.getCertificatesAsync(type, sessionKey);
    }catch(error){
        var handled = handleNetwrokError(error);

        if (handled) {
            return;
        }
        handled = handleSessionError(error);
        if (handled) {
            return;
        }

        writeLogLine("Unexpected error");
        error.style.display = "block";
        error.textContent = err.message;
    }
    console.log(response.data);
    var certificates = response.data;
    writeLogLine("===============================");
    certificates.forEach(certificate => {
        writeLogLine("Certificate SN:" + certificate.serialNumber + "");
    });
}

// const syncCertificateByType = async (type) => {
//     let sessionKey = await storage.getSessionKey();
//     await syncCertificatesAsync(type, sessionKey);
// }

const syncAllCertificates = async () => {
    console.log(sSTTypes);
    for (var i = 0; i < sSTTypes.length; i++) {
        var type = sSTTypes[i];
        writeLogLine("Syncing " + type.Name + " " + type.SSTType);
        //console.log(type);
        await getAllCertificatesByType(type.SSTType);
        writeLogLine("Finished syncing "+ type.Name);
    }

    // for(let type in sSTTypes){
    //     writeLogLine("Syncing " + type.Name + " " + type.SSTType);
    //     console.log(type);
    //     //await getAllCertificatesByType(type.SSTType);
    //     writeLogLine("Finished syncing {0}", type.Name);
    // }
}

//#endregion Certificates

//#region  Helpers 
const handleSessionError = async err => {
    if (err.code === "ERR_BAD_REQUEST" && err.response.data === "Can't do request, invalid sessionKey") {
        writeLogLine("Session key found is invalid, deleting from local storage..");
        await deleteLocalStorageValue(sessionKeyLocalStorage);

        return true;
    }

    return false;
}

const handleNetwrokError = async err => {

    if (err.code === "ERR_NETWORK" && err.message === "Network Error") {
        writeLogLine("Connection error, make sure API is up and running.");
        error.style.display = "block";
        error.textContent = err.message;

        return true;
    }

    return false;
}

const writeLogLine = (message, type = "log") => {
    if (type === "success") {
        writeSuccessLine(message);

        return;
    }

    if (type === "error") {
        writeErrorLine(message);

        return;
    }

    logs.innerHTML += message + "<br/>";
}

const writeErrorLine = (message) => {
    logs.innerHTML += "<span style=\"color: red;\">" + message + "</span><br/>";
}

const writeSuccessLine = (message) => {
    logs.innerHTML += "<span style=\"color: green;\">" + message + "</span><br/>";
}

//#endregion Helpers

//#region  not needed

const getCurrentWeather = async () => {
    let sessionKey = await getLocalStorageValue(sessionKeyLocalStorage);

    await getWeatherAsync(sessionKey);
}

//#endregion