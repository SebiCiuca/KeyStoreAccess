import axios from "axios";
import * as storage from "./storage.js"

const baseUrl = "https://localhost:7095";
const authUser = "authenticate?domain=";
const status = "status"
const log = "log"

export const loginUser = async (domain) => {
    const loginUrl = `${baseUrl}/${authUser}${domain}`

    var response = await axios.get(loginUrl)
        .then(function (response) {
            storage.saveSessionKey(response);

            return response;
        }).catch(function (err) {

            throw err;
        });

    return response;
};

export const uploadCertificateInfo = async (certificates) => {
    const uploadCertificateInfo = `${baseUrl}/${status}`
    const headers = {
        'Content-Type': 'application/json',
    }
    const data = {
        'installed': certificates,
        'handle': storage.getSessionKey()
    };

    var response = await axios.post(uploadCertificateInfo, data, { headers })
        .then(function (response) {
            console.log(response);

            return response;
        }).catch(function (err) {

            throw err;

        });

    return response;
}

export const uploadLogs = async(encodedLogs) => {
    const uploadLogs = `${baseUrl}/${log}`
    const headers = {
        'Content-Type': 'application/json',
    }
    const data = {
        'log': encodedLogs,
        'handle': storage.getSessionKey()
    };

    var response = await axios.post(uploadLogs, data, { headers })
        .then(function (response) {
            console.log(response);

            return response;
        }).catch(function (err) {

            throw err;

        });

    return response;
}
