import axios from "axios";

const baseUrl = "https://localhost:7095";
const auth = "Auth";
const keyStore = "KeyStore"

const generateSessionEndpoint = auth + "/CreateSession?nonceValue=";
const validateSessionEndpoint = auth + "/ValidateSession";
const getCertifcatesEndpoint = keyStore + "/GetCertificates?ssTType=";
const syncCertifcatesEndpoint = keyStore + "?ssTType=";
const getWeatherEndpoint = "WeatherForecast"

//#region  Calls to API
export const validateSessionKeyAsync = async sessionKey => {
    const validateSessionUrl = `${baseUrl}/${validateSessionEndpoint}`
    const headers = {
        'Content-Type': 'application/json',
    }

    var response = await axios.post(validateSessionUrl, sessionKey, { headers })
        .then(function (response) {
            console.log(response);
            return response;
        }).catch(function (err) {
            throw err;
        });

    return response;
};

export const getWeatherAsync = async sessionKey => {
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

    return response;
}


export const generateSessionAsync = async nonceValue => {

    const generateSessionUrl = `${baseUrl}/${generateSessionEndpoint}${nonceValue}`;
    const response = await axios.get(generateSessionUrl);

    return response;
}

export const getCertificatesAsync = async (ssTType, sessionKey) => {
    const getCertificatesUrl = `${baseUrl}/${getCertifcatesEndpoint}${ssTType}`
    const headers = {
        'Content-Type': 'application/json',
        'SessionKey': sessionKey
    }

    return await axios.get(getCertificatesUrl, { headers })
        .then(function (response) {
            console.log(response);
            return response;
        }).catch(function (err) {
           throw err;
        });
}

export const syncCertificatesAsync = async (ssTType, sessionKey) => {
    const synCertificatesUrl = `${baseUrl}/${syncCertifcatesEndpoint}${ssTType}`
    const headers = {
        'Content-Type': 'application/json',
        'SessionKey': sessionKey
    }
    const data = {};

    var response = await axios.post(synCertificatesUrl, data, { headers })
        .then(function (response) {
            console.log(response);
        }).catch(function (err) {
            error.style.display = "block";

            if (err.message === "Network Error") {
                error.textContent = "Error: Network error!";
            } else {
                handleSessionError(err);
                error.textContent = "Error ar retriving weather, please retry!";
            }
        });

    return response;
}

//#endregion Calls to API
