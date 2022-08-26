using UserCertificateAutoEnrollment.BL.Common.Contracts;

namespace UserCertificateAutoEnrollment.BL.Http
{
    public class HttpClient : IHttpClient
    {
        private readonly HttpClientUrls m_Urls;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private const string SIG_EXTENTION = ".sig";

        public HttpClient(HttpClientUrls urls)
        {
            m_Urls = urls;
        }

        public async Task<byte[]> GetCertificate(string url)
        {
            _logger.Trace($"Retriving cerficate using url: {url}");
            _logger.Info("Retriving certificate info");

            try
            {
                using var client = new System.Net.Http.HttpClient();

                //var uri = new Uri(m_Urls.GetNonceUrl);

                var result = await client.GetByteArrayAsync(url);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Retriving certificate info resulted in an error");

                return new byte[0];
            }
        }

        public async Task<byte[]> GetCertificateSignature(string url)
        {
            _logger.Trace($"Retriving cerficate using url: {url}");
            _logger.Info("Retriving certificate info");

            if (!url.EndsWith(SIG_EXTENTION))
            {
                _logger.Trace($"Url received does not end in {SIG_EXTENTION}, adding {SIG_EXTENTION} to the url");
                url = $"{url}{SIG_EXTENTION}";
                _logger.Trace($"New url to be called:{url}");
            }

            try
            {
                using var client = new System.Net.Http.HttpClient();

                var result = await client.GetByteArrayAsync(url);

                _logger.Trace($"Certificate bytes: {result}");
                _logger.Info("Certificate info successfuly retrieved");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Retriving certificate signature resulted in an error");

                return new byte[0];
            }
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
