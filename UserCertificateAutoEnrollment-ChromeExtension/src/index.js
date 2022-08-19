import axios from "axios";

const baseUrl = "https://localhost:7095";

const generateSessionEndpoint = "Auth/CreateSession?nonceValue=";
const validateSessionEndpoint = "Auth/ValidateSession";
const getWeatherEndpoint = "WeatherForecast"

const sessionKeyLocalStorage = "SessionKey";

const error = document.querySelector("#error");
const logs = document.querySelector("#logs-area");

error.style.display = "none";
logs.value = "";

// grab the form
const form = document.querySelector(".form-data");

// declare a function to handle form submission
const handleSubmit = async e => {
    console.log(logs);
    e.preventDefault();
    logs.value += "Retriving session key from local storage...\n";

    let sessionKey = await getLocalStorageValue(sessionKeyLocalStorage);
   
    if (sessionKey === null) {
        logs.value += "No session key found locally.\n";
        logs.value += "Generating new session key\n";

        await generateNewSession();
    }
    logs.value += "Session key found, retriving weather\n";
    logs.value += "Getting weather\n";

    await getCurrentWeather();
};

form.addEventListener("submit", e => handleSubmit(e));

const generateNewSession = async () => {
    await generateSessionAsync("mysmallkey123456");
    logs.value += "Session key generated!\n";
    logs.value += "Validating session key...\n";
    await validateSession();
}

const getCurrentWeather = async () => {
    let sessionKey = await getLocalStorageValue(sessionKeyLocalStorage);
  
    await getWeatherAsync(sessionKey);
}

const validateSession = async () => {   
    let sessionKey = await getLocalStorageValue(sessionKeyLocalStorage);
    await validateSessionKeyAsync(sessionKey);
}


const getLocalStorageValue = async key => {
    return new Promise((resolve, reject) => {
        chrome.storage.local.get([key], function (result) {
            console.log(result);
            console.log(result[key]);
            if (result[key] === undefined) {
                resolve(null);
            } else {
                resolve(result[key]);
            }
        });
    });
};

const deleteLocalStorageValue = async key => {
    return new Promise((resolve, reject) => {
        chrome.storage.local.remove([key], function (result) {
            console.log(result);
            resolve();
        })
    })
}
const handleSessionError = async err => {  
    if(err.code === "ERR_BAD_REQUEST" && err.response.data === "Can't do request, invalid sessionKey"){
        logs.value += "Session key found is invalid, deleting from local storage...\n";
        await deleteLocalStorageValue(sessionKeyLocalStorage);
    }
}


//#region  Calls to API
const validateSessionKeyAsync = async sessionKey => {
    const validateSessionUrl = `${baseUrl}/${validateSessionEndpoint}`
    const headers = {
        'Content-Type': 'application/json',
    }
    var response = await axios.post(validateSessionUrl, sessionKey, { headers })
        .then(function (response) {
            console.log(response);          
        }).catch(function (err) {
            if (!err.response) {
                error.style.display = "block";
                error.textContent = "Error: Network error";
            } else {
                error.style.display = "block";
                error.textContent = "Could not validate session, please retry!";
            }
        });
};

const getWeatherAsync = async sessionKey => {
    const getWeateherUrl = `${baseUrl}/${getWeatherEndpoint}`
    const headers = {
        'Content-Type': 'application/json',
        'SessionKey': sessionKey
    }

    var response = await axios.get(getWeateherUrl, { headers })
        .then(function (response) {
          
             logs.value += "Current weather " + response + "\n";
        })
        .catch(function (err) {
            error.style.display = "block";

            if (err.message === "Network Error") {             
                error.textContent = "Error: Network error!";
            } else {
                handleSessionError(err);
                error.textContent = "Error ar retriving weather, please retry!";
            }
        });
}


const generateSessionAsync = async nonceValue => {   
    try {
        const generateSessionUrl = `${baseUrl}/${generateSessionEndpoint}${nonceValue}`;
        const response = await axios.get(generateSessionUrl);
      
        var sessionKey = response.data.sessionKey;
        var storageSessionKey = {}
        storageSessionKey[sessionKeyLocalStorage] = sessionKey;

        //save to chrome extensions
        chrome.storage.local.set(storageSessionKey, function () {
            console.log('Saving session key' + sessionKey);
        });
        
    } catch (error) {
        error.style.display = "block";
        
        error.textContent = "Could not create session with API, can't load ";
    }
}

//#endregion Calls to API

