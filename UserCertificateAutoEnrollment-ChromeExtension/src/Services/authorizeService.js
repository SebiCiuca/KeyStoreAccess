import * as api from "../api.js"
import * as storage from "../storage.js"

const baseUrl = "https://saml-si.allianz.com/";
const authUser = "GINPW/Azure/loginNoPromptUserCA";

//#region  Calls to API
export const authorizeUserAsync = async () => {
  //get the unique value after authroization
  var domain = await storage.getLoggedInUser();
  console.log("9. Domain " + domain);
  var response = await api.loginUser(domain);
  console.log(response);
  storage.saveSessionKey(makeid(10));
  var sessionKey = await storage.getSessionKey();
  console.log("11. Session key " + sessionKey);
};

export const isUserLoggedIn = async (content) => {
 
  if (content != "") {
    storage.saveSessionKey(content);
  }

  return content != "";
}

export const openLoginPage = () => {
  const loginUrl = `${baseUrl}/${authUser}`;

  chrome.tabs.create({ url: loginUrl });
  chrome.tabs.update(undefined, { url: loginUrl });

}
function makeid(length) {
  var result = '';
  var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  var charactersLength = characters.length;
  for (var i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() *
      charactersLength));
  }
  return result;
}
