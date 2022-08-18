namespace UserCertificateAutoEnrollment.BL.Security
{
    public interface ICryptoService
    {
        byte[] EncrpyRandomPassword(string nonceValue);
    }
}
