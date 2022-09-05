namespace UserCertificateAutoEnrollment.BL.Common
{
    public class CertificateModel
    {
        public string UniqueIdentifier { get; set; }
        public string SerialNumber { get; set; }
        public string Issuer { get; set; }
        public string SubjectName { get; set; }
        public string Provider { get; set; }
        public bool PrivateKeyPresent { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
    }
}
