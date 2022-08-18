using System.Text;

namespace UserCertificateAutoEnrollment.BL.Helpers
{
    public static class UnicodeExtentions
    {
        public static byte[] ToByteArray(this string input)
        {
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();

            return unicodeEncoding.GetBytes(input);
        }

        public static string ToString(this byte[] input)
        {
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();

            return unicodeEncoding.GetString(input);
        }
    }
}
