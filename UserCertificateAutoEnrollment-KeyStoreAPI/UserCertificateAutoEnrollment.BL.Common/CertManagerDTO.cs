using System.Text.Json.Serialization;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class CertManagerDTO
    {
        [JsonPropertyName("CertManager")]
        public List<CertificateDTO> LocalCertificates { get; set; }
    }
}
