namespace UserCertificateAutoEnrollment.BL.Security
{
    public class RandomPasswordGenerator
    {
        const string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(charPool, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
