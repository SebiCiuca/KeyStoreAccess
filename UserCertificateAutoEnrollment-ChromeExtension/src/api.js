import axios from "axios";
import * as storage from "./storage.js"
const baseUrl = "https://saml-si.allianz.com/";
const authUser = "GINPW/Azure/loginNoPromptUserCA";
const status = "status"
const log = "log"


const parseOpenIdContentAndSendResponse = async (content) => {
    console.log(content);
	var startSAMLJson;
	if (content.indexOf("$Config") > 0)
    {
		startSAMLJson = getJsonFromConfig(content)
	}
    console.log(startSAMLJson);

    var SAML = JSON.parse(startSAMLJson);
    console.log("=====================\\n\\n\\n");    
    console.log(JSON.stringify(SAML));

    return SAML;
}

const getJsonFromConfig = (content) => {
	var startIndex = content.indexOf("$Config=") + 8;
    var contentFromConfig = content.substring(startIndex);
	var endIndex = startIndex + contentFromConfig.indexOf( ";");

    console.log("StartIndex: "+ startIndex);
    console.log("EndIndex: "+ endIndex);

	return content.substring(startIndex,endIndex);
}

export const loginUser = async (domain) => {
    const loginUrl = `${baseUrl}/${authUser}`
    
    // fetch('html_microsoft.html')
    //     .then(response => response.text())
    //     .then(text => parseOpenIdContentAndSendResponse(text))
    //     .then(json => console.log(json));
    //var content = readSingleFile("C:\\Users\\SEBI\\Desktop\\html_microsoft.html");
    var openIdresponse = await axios.get(loginUrl)
        .then(function (response) {
            var microsoftOpenIdResponse = response.data;
            var jsonConfig = parseOpenIdContentAndSendResponse(microsoftOpenIdResponse);

            return jsonConfig;
        }).catch(function (err) {

            throw err;
        });
    
    const reprocessUrl = openIdresponse.urlLogin;
    var reprocessResponse = await axios.get(reprocessUrl)
        .then(function(response){
            var microsoftOpenIdResponse = response.data;
            var jsonConfig = parseOpenIdContentAndSendResponse(microsoftOpenIdResponse);
            
            return jsonConfig; 
        }).catch(function (err){
            throw err;
        });

    const callbackUrl = reprocessResponse.urlLogin;
    var callbackRespose = await axios.get(callbackUrl)
        .then(function(response){
               storage.saveSessionKey(response);

                return response; 
        }).catch(function (err){
            throw err;
        });

 
    // return allianzResponse;
    // chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {

    //     // since only one tab should be active and in the current window at once
    //     // the return variable should only have one entry
    //     var activeTab = tabs[0];
    //     var activeTabId = activeTab.id; // or do whatever you need
   
    //  });
        //chrome.tabs.create({url: loginUrl});
    // chrome.tabs.update(undefined, { url: loginUrl });

    // chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {

    //     // since only one tab should be active and in the current window at once
    //     // the return variable should only have one entry
    //     var activeTab = tabs[0];
        
    //     var activeTabId = activeTab.id; // or do whatever you need
        
    //  });
    
};

export const uploadCertificateInfo = async (certificates) => {
    const uploadCertificateInfo = `${baseUrl}${status}`
    const headers = {
        'Content-Type': 'application/json',
    }
    const data = {
        'installed': certificates,
        'handle': await storage.getSessionKey()
    };
    
    //return await getCerts();
    var response = await axios.post(uploadCertificateInfo, data, { headers })
        .then(function (response) {
            console.log(response);

            return response;
        }).catch(function (err) {

            throw err;

        });

    return response;
}

export const getCerts = async() => {
    const response = {
        "ImportCertificates": [
            {
              "sn": "caa21",
              "issuer": "CN=Allianz UserCA V, O=Allianz, C=DE",
              "tp": "2F6873B5D3D962CBB1003D3AC79C997C5118ECFF",
              "dn": "C=DE,O=Allianz,E=volker.zeihs@allianz.de,CN=Volker Zeihs",
              "auth": true
            },
            {
              "sn": "098149736644D339",
              "issuer": "CN=Allianz UserCA, O=Allianz, C=DE",
              "tp": "F261488F38B4D49BB1003D3AC79C997C51181234",
              "dn": "C=DE,O=Allianz,E=volker.zeihs@allianz.de,CN=Volker Zeihs",
              "auth": false
            }
          ],
        //   "pkcs12":"test/wqewq+./qweqeq//eqwq"
          "pkcs12": "MIIKWAIBAzCCChQGCSqGSIb3DQEHAaCCCgUEggoBMIIJ/TCCBe4GCSqGSIb3DQEHAaCCBd8EggXbMIIF1zCCBdMGCyqGSIb3DQEMCgECoIIE7jCCBOowHAYKKoZIhvcNAQwBAzAOBAjslY0EWPiyRwICB9AEggTItTgbud"+
                    "KRaEBCdOn1rMgGUoGjVIOxTgT/5Oo5D0gfBe/0yorvQKenVimcfDmWCXZGlsvou5Km7g3yjmK7PTZh3IUuX86bJeyECrJBZa+4ZXyrbkVV7R2GEwy99ACOkevHxBZ7H6deVKdRFDsCSC+cdLpsSGoNNi3Coowqw8i7lzXDP"+
                    "zph64L2Rre7cky/QJdkXKIEUGjxKYUR4cpOOmlLXfbQMR3fOChT5FrxbOnTRTAvVELtwFTh8Gxq55Et8rLgIktQ2eL8FIx43sGspukhZvm5bH05aEwVz5df0T3e3cMsZ13t4oc72phNKx9HiZmPvmi4lUdBwn6qXEJSRg+P5"+
                    "MWGD41StCAoZ/pDtMTPSV5DalLpDpKF6XfxByOSXfn27KYWFZoiEdxCmabS7eomqeD03uWEzWbgPW1hha69Bg1DcmSYZgDfISugtow6p+ozQSjaNnELlbz1SsiBMPbkoNj944IxSKkHgfiqwOyQm3JqHGjDH6Hp9OB3RyuQH"+
                    "oeWLrW9ulmWvnT/Dn+LC0T26YHeVeTO30U86r9ElBiv4iXa5JdflWqFecbjQC1Xs29yBkkFRNrfgJ4Txd/xOz0rhzFxZmcIwiJsQgh+WxSWLG3em02xmtyO7Rcxc6g+onVpEFXKiaYDqTWNDKE2wDSDPZauheyiiluj42gi5"+
                    "A086rSjuqqyidzWQb6FKYs73ZUM8drnYK36RT7zG8TFpV3RdrXbnui8eFRZ4bFHNlptHTwvqpZY/sDDP1R1q1EgrckMbZhOpHhKW/1GRd95eHlSGJPdTROtR3uEARZD0+1zBarw68p1k650DWua88cMrOQPKH1X8Ce++KIdF"+
                    "G2RtfI2P3lhPsD0gdpet0gDEC1peUXfrZVQYD3G8IfEjpYgDpgPa6zjblScidem628MPz4dnTnWrp1S7P2ILlGYinb9uCzSR17IvPoVITQvXk9KpTp1uzuWTnRQHMfUyNvgjHSAOwt3bmx5kEyC/04ujhUvQkiyPMCrDtiO/"+
                    "fLYkrWuqOLNr74l/VAJYU06YR7yNMfG1kEpRYkf96v0s3XK1tZEQBt26lZFPIMl/NOHYpbCfxCGZkpOGIFDojjo72p4VUuyGD/MZ41B99jcNVB2/Sqmc9XwALl/jjkJz6+aCmvLUePeLmhg2Bxrjkmf8iDfDnKByXM5lam3D"+
                    "P1PDBIsopTWgZH5ftg1ooKd76iM2RDxPNh8aQZTuOzGiezj/"+
                    "tjcVY3nAX1+kS4fDK6LU0AE251RNLAwRTSLep9vSkwWSaGs7mYh8YcbaKZHQu2fiVRUUc2jGFncTqyLo6TRE1i2g4cwsBGSH3ih8stP0wexd0gDn8uZ34eZkoHO5ATe4h1kw6KiEqayjAkUGidkflvXRZid2B8yqkI2UkVZNhPBVTPDoOvaK6eaegelZuWhWhmi7vg/"+
                    "n6z7HchVJ7JweRdKPMkDqe7HFOlBMrrrmevfIvPqohqtkecFFJk5foSno+wvt20bKKhlHKM6gi3vK9MIs7loK8QqaT7C83RFeKB/h91RAMtUgmAt+YtJKvkk"+
                    "/aybMQmGPL8y4emoOO0oBDMZzS/noQGYBvbRP476eijRsseNJy8SVW9aTAIpNrgx3U+cRcsa2uoXKObmbaHuMYHRMBMGCSqGSIb3DQEJFTEGBAQBAAAAMFsGCSqGSIb3DQEJFDFOHkwAewBDAEMANQBEADMARQAxADMALQAxA"+
                    "EUANQAxAC0ANAA5AEIAMwAtADkANwBBADYALQBBADQANgAzAEIAOQBGAEMANgAzADMAQwB9MF0GCSsGAQQBgjcRATFQHk4ATQBpAGMAcgBvAHMAbwBmAHQAIABTAG8AZgB0AHcAYQByAGUAIABLAGUAeQAgAFMAdABvAHIAYQB"+
                    "nAGUAIABQAHIAbwB2AGkAZABlAHIwggQHBgkqhkiG9w0BBwagggP4MIID9AIBADCCA+0GCSqGSIb3DQEHATAcBgoqhkiG9w0BDAEDMA4ECF+pVxzoIjjqAgIH0ICCA8DGdJpRdt23njzhoK9wD4XbN89S2tIlKdK6+pyD5pEkz"+
                    "D8nLxS4LhHRjGvF"+
                    "+FhlVsEwU37WTR90Dh4t5pxfa+rGkVSvVVe2yZ0moB21pCCDHzfov3wvdwgtJDjDvXy0tJxpX64Ktmd7zkmA17a45SrgMOUR4filIp9Hxtjou480cfXPXH22bQFALNE4Dthr7+nXp6EqEiF0iufEoI6/KsrkEgyLh+bLm4hNUM"+
                    "CvVdGmsYUCUNLrZaF9yFwZbXSIYyYPmZtBeDEhJL9Qd7mhW4NQ4dlI2"+
                    "+YBs0/bcMEbY4qWYpPiDxvCp2QHSuK8mBGTJWAtk9dpwbz8jxjPNR0FMho0kWFytDmta0/z55n5kEJXwyLdvq4clSmA1enZBm+gZMKEQSpDoZTyONkGKijGoh4SJ4yn6A7HqmzFKgYn7PqYAa1JRfG/l13xruGZBlZ9p+mLsGD5"+
                    "LzwzFKfykvW33Sr/87F8UPY4iRsMqqGOsvBkZinTp0Qd9rZXmYdF2yPOMyRZgsGnPtWNFkWv18FrxEnVSy+"+
                    "M6OgQosx/AJAyUsg6MzrAwdi7auHU50rMsDN4r9wezVLVic0wqqC73bJh3upneH3jmaYcqYb20farfufQX+vA5HHBMlqveSKMJb5q7oxxeN0cSqn4C8EqjX9GlGF+EpTEQaD2wT9NptyPoCcXVKhGnwUVKQ8B6fpHw5a7eOoz+v"+
                    "Xokzlc6RPwMpFFlUIyGDmUXO3cj1YAfbhbiINDS34SDXEPKe89ivMf+"+
                    "47+eLWZqKVhng87Ze8MQxHzaGTsLreLtRc9F4L6SjVEFktkaDNVrF6d7j2HNlM7SARzyZ8HeWrjWG6MZIOGq9vep2MdQUVtCAnf5bJRimfLGmyiHyXfcXMeW5cDMB1Q0uCZByHL0z+f0+YthPo32kn1Kr/7zi9XSQlVA4iQFCCwj"+
                    "vx537cT58awzPaDpovQESVbBIrf3yLSWDLduBiE8KAhVt"+
                    "+zFA1XTbFoZtuTsXy77f9nWonMmLROnn/hO57d/ork++kNOPYI55JbU5Wa66/RLMMoirQGCxqywDsIRZUIQMFy1lZNCV2OVrdrS92KGGL4+k5U3FgFs353yBiOtft9z1vnDKopUEuBEl7nTj8a9PBDO9k3T8c6gt8090zQwbPQQp"+
                    "LqNjZPQjViBlmha/Nj/raw57URwZpCEDA7SO9ygU3aurRmIf5qpcjZR+mOYQNZYgst/T8YcNUGL3E+rxS8iWb1nOyCvGXEfiul7T/MaHFcdo+t978Eulqx6TqZ343dcNIwOzAfMAcGBSsOAwIaBBThVsi8m662LgkehyqZmFTFyA"+
                    "SAZwQUaZtd8IMY7TaqXBNxkbD3AZwJoQ4CAgfQ"
                    
    }

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
