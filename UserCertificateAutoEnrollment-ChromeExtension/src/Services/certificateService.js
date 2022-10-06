import * as nativeService from "./Services/nativeService.js"
import * as api from "../api.js"

export const syncCertificates = async () => {
   console.log("1. Syncing certifcates");
   console.log("2. Retriving local certicates");
   var certificates = await nativeService.getCertificates();
   console.log("3. Local certificates ==================");
   console.log(certificates);
   console.log("4. ===================================");
   console.log("5. Uploading certificates to server....");
   await api.uploadCertificateInfo(certificates);
   console.log("Certificates uploaded successfully");
}
