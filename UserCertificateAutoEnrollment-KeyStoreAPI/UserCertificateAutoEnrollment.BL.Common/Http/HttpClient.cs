using UserCertificateAutoEnrollment.BL.Http;

namespace UserCertificateAutoEnrollment.BL.Common.Http
{
    public class HttpClient : IHttpClient
    {
        private readonly HttpClientUrls m_Urls;

        public HttpClient(HttpClientUrls urls)
        {
            m_Urls = urls;
        }
        public async Task<string> GetNonceValue()
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();

                var uri = new Uri(m_Urls.GetNonceUrl);

                var result = await client.GetStringAsync(uri);

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error at retrieving nonce value {ex.Message}");

                return string.Empty;
            }
        }

        public async Task<bool> ValidateSessionPassword(byte[] password)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();

                var uri = new Uri(m_Urls.ValidateSessionPasswordUrl);

                var result = await client.GetAsync(uri);

                if (result.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Session is ok");

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error at retrieving nonce value {ex.Message}");

                return false;
            }
        }
    }
}
