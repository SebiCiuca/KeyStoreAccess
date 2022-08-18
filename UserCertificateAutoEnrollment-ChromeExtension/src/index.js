import axios from "axios";

const baseUrl = "https://localhost:7095";

const generateSessionEndpoint = "Auth/CreateSession?nonceValue=";
const validateSessionEndpoint = "Auth/ValidateSession";
const getWeatherEndpoint = "WeatherForecast"

const sessionKeyLocalStorage = "SessionKey";

const error = document.querySelector("#error");
const logs = document.querySelector("#logs-area");
// const loading = document.querySelector(".loading");
// const cases = document.querySelector(".cases");
// const recovered = document.querySelector(".recovered");
// const deaths = document.querySelector(".deaths");
// const results = document.querySelector(".result-container");
const sessionKey = "";
error.style.display = "none";
logs.value = "";
// loading.style.display = "none";
// errors.textContent = "";
// grab the form
const form = document.querySelector(".form-data");

// declare a function to handle form submission
const handleSubmit = async e => {
    e.preventDefault();
    logs.append("Retriving session key from local storage");
    let sessionKey = await getLocalStorageValue(sessionKeyLocalStorage);
    if(sessionKey === null){
        logs.append("No session key found locally.");
        logs.append("Generating new session key");

        await generateNewSession();
    }
    logs.append("Session key found, retriving weather");
    //validate with server that session is ok
    //add code here
    //mark session as valid in API
    await validateSession();
    console.log(sessionKey);
    console.log("Getting weather");
    await getCurrentWeather();
};

form.addEventListener("submit", e => handleSubmit(e));

const generateNewSession = async() => {
    await generateSessionAsync("mysmallkey123456");
    logs.append("Session key generated!");
    logs.append("Validating session key...");
    await validateSession();
}

const getCurrentWeather = async () => {
     let sessionKey = await getLocalStorageValue(sessionKeyLocalStorage);

    await getWeatherAsync(sessionKey);
}

const validateSession = async() => {  
    console.log("Retriving Session key from chrome local storage");
    let sessionKey = await getLocalStorageValue(sessionKeyLocalStorage);
    console.log("Session key found in storage: " + sessionKey);
    await validateSessionKeyAsync(sessionKey);
    console.log("Session key validated");
}


const getLocalStorageValue = async key =>  {
    return new Promise((resolve, reject) => {
        chrome.storage.local.get([key], function (result) {
          if (result[key] === undefined) {
            reject();
          } else {
            resolve(result[key]);
          }
        });
      });
};

const validateSessionKeyAsync = async sessionKey => {
    const validateSessionUrl = `${baseUrl}/${validateSessionEndpoint}`
    const headers = {
        'Content-Type': 'application/json',
    }
    var response = await axios.post(validateSessionUrl, sessionKey, {headers})
    .then(function (response) {
        console.log(response);
        cases.textContent = "Session validated successfully";
    }).catch(function (err) {
        error.style.display = "block";
        console.log(err);
        error.textContent = "Could not validate session, please retry!";
    });
    console.log(response);
};

const getWeatherAsync = async sessionKey => {
    const getWeateherUrl = `${baseUrl}/${getWeatherEndpoint}`
    const headers = {
        'Content-Type': 'application/json',
        'SessionKey': sessionKey
    }

    var response = await axios.get(getWeateherUrl,{headers})
    .then(function (response){
        console.log(response);
        cases.textContent = "Current weather " + response;
    })
    .catch(function(err){
        console.log(err);
        errors.textContent = "Error ar retriving weather"
    });    
    console.log(response);
}


const generateSessionAsync = async nonceValue => {
    loading.style.display = "block";
    errors.textContent = "";
    try {
        const generateSessionUrl = `${baseUrl}/${generateSessionEndpoint}${nonceValue}`;
        const response = await axios.get(generateSessionUrl);
        console.log(generateSessionUrl);
        console.log(response);
        loading.style.display = "none";
        // cases.textContent = response.data.sessionKey;
        console.log(response);
        var sessionKey = response.data.sessionKey;

        console.log('Session key' + response.data.sessionKey);
        console.log('Saving to chrome extension');
        var storageSessionKey = {}
        storageSessionKey[sessionKeyLocalStorage] = sessionKey;
        console.log(storageSessionKey);
        //save to chrome extensions
        chrome.storage.local.set(storageSessionKey, function () {
            console.log('Saving session key' + sessionKey);
        });
        console.log('Saved to chrome extension');
      
    } catch (error) {
        error.style.display = "block";
        console.log(error);
        error.textContent = "Could not create session with API, can't load ";
    }
}


