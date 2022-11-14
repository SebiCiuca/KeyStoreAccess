using System.Text.Json.Serialization;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class ImportCertificateDTO
    {
        [JsonPropertyName("sn")]
        public string SerialNumber { get; set; }
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }
        [JsonPropertyName("tp")]
        public string Thumbprint { get; set; }
        [JsonPropertyName("dn")]
        public string SubjectName { get; set; }
        [JsonPropertyName("auth")]
        public bool IsAuthCertificate { get; set; }
    }
}
