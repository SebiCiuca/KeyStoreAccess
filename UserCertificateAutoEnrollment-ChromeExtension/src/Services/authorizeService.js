import * as api from "../api.js"
import * as storage from "../storage.js"

//#region  Calls to API
export const authorizeUserAsync = async () => {
    //get the unique value after authroization
    var domain = await storage.getLoggedInUser();
    console.log("9. Domain " + domain);
    await api.loginUser(domain);
    storage.saveSessionKey(makeid(10));
     var sessionKey = await storage.getSessionKey();
    console.log("11. Session key " + sessionKey);
};




function makeid(length) {
    var result           = '';
    var characters       = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for ( var i = 0; i < length; i++ ) {
      result += characters.charAt(Math.floor(Math.random() * 
 charactersLength));
   }
   return result;
}
