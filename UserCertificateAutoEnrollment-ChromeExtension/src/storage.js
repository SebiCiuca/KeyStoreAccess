const sessionKeyLocalStorage = "SessionKey";
const loggedInUser = "LoggedInUser";

const deleteLocalStorageValue = async key => {
    return new Promise((resolve, reject) => {
        chrome.storage.local.remove([key], function (result) {
            console.log(result);
            resolve(true);
        })
    })
}

export const getSessionKey = async () => {
    return new Promise((resolve, reject) => {
        chrome.storage.local.get([sessionKeyLocalStorage], function (result) {          
            if (result[sessionKeyLocalStorage] === undefined) {
                resolve(null);
            } else {
                resolve(result[sessionKeyLocalStorage]);
            }
        });
    });
};

export const deleteSessionKey = async() => {
   return deleteLocalStorageValue(sessionKeyLocalStorage)
}

export const saveSessionKey = async (keyValue) => {
    return new Promise((resolve,reject) => {
        var storageSessionKey = {}
        storageSessionKey[sessionKeyLocalStorage] = keyValue;
        //save to chrome extensions
        chrome.storage.local.set(storageSessionKey, function () {
            console.log('Saving session key ' + storageSessionKey[sessionKeyLocalStorage]);
            resolve(true);
        });
    })
}

export const saveLoggedInUser = async (user) => {
    return new Promise((resolve,reject) => {
        var loggedInUserKey = {}
        loggedInUserKey[loggedInUser] = user;
        //save to chrome extensions
        chrome.storage.local.set(loggedInUserKey, function () {
            console.log('7. Saving logged in user ' + loggedInUserKey[loggedInUser]);
            resolve(true);
        });
    })
}

export const getLoggedInUser = async () => {
    return new Promise((resolve, reject) => {
        chrome.storage.local.get([loggedInUser], function (result) {          
            if (result[loggedInUser] === undefined) {
                resolve(null);
            } else {
                resolve(result[loggedInUser]);
            }
        });
    });
};

