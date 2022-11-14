using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserCertificateAutoEnrollment.BL.Common
{
    public class ImportCertManagerDTO
    {
        [JsonPropertyName("ImportCertificates")]
        public List<ImportCertificateDTO> ImportCertificates { get; set; }
        [JsonPropertyName("pkcs12")]
        public byte[] Pkcs12 { get; set; }
    }   
}
