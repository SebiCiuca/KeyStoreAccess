import * as api from "../api.js"
import * as storage from "../storage.js"

//#region  Calls to API
export const authorizeUserAsync = async () => {
    //get the unique value after authroization
     await api.loginUser();
     storage.saveSessionKey("valueFromlogin");
    // if (response.IsLoggedIn) {
    //     storage.saveSessionKey(response.UniqueValue);
    // }
};
