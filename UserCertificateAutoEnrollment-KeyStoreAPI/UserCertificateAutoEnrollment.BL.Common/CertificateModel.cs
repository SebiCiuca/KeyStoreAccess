using System.Text.Json.Serialization;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class CertificateDTO
    {
        [JsonPropertyName("sn")]
        public string SerialNumber { get; set; }
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }
        [JsonPropertyName("tp")]
        public string Thumbprint { get; set; }
        [JsonPropertyName("NotAfter")]
        public string NotAfter { get; set; }
        [JsonPropertyName("dn")]
        public string Subject { get; set; }
        [JsonPropertyName("eku")]
        public List<string> EKU { get; set; }
    }
}
