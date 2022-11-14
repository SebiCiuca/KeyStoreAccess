import * as nativeService from "./nativeService.js"
import * as api from "../api.js"

export const syncCertificates = async () => {
   console.log("1. Syncing certifcates");
   console.log("2. Retriving local certicates");
   //var certificates = await nativeService.getCertificates();
   console.log("3. Local certificates ==================");
   console.log(certificates);
   console.log("4. ===================================");
   console.log("5. Uploading certificates to server....");
   //var certsToUpdate = await api.uploadCertificateInfo(certificates);
   var certs =await api.getCerts(); 
   console.log("6. Certificates uploaded successfully");
   console.log(certs);
   console.log("7. Sending new certs to client ");
   var certUploadResponse = nativeService.syncCertificates(certsToUpdate);
   console.log("8.Certs sent succesffuly to client");
   console.log(certUploadResponse);
}
