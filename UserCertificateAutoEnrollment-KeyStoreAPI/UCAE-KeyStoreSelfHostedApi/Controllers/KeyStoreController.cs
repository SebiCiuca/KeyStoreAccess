using Microsoft.AspNetCore.Mvc;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Session;

namespace UCAE_KeyStoreSelfHostedApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KeyStoreController : Controller
    {
        private readonly IKeyStoreFactory _keyStoreFactory;
        private readonly ISessionProvider _sessionProvider;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public KeyStoreController(IKeyStoreFactory keyStoreFactory, ISessionProvider sessionProvider)
        {
            _keyStoreFactory = keyStoreFactory;
            _sessionProvider = sessionProvider;
        }

        [HttpGet]
        [Route("GetCertificates")]
        public async Task<IActionResult> GetCertificatesAsync()
        {
            //_logger.Trace($"Creating new session with provided nonceValue: {nonceValue}");
            _logger.Info($"List of certificates was requested");

            ////validate nonceValue <-- maybe limit chars to a specific amount
            //if (string.IsNullOrEmpty(nonceValue))
            //{
            //    _logger.Warn("Could not create session nonceValue format is incorect");
            //    _logger.Trace($"Could not create session nonceValue format is incorect. Value provided {nonceValue}");

            //    throw new ArgumentException($"'{nameof(nonceValue)}' format is not correct", nameof(nonceValue));

            //}
            var os = "windows";

            _logger.Info($"Retriving list of certificates from {os} OS");
            var keyStore = _keyStoreFactory.GetKeyStoreResolver(os, _sessionProvider.CurrentSession);
            _logger.Trace($"Created key store resolver of type {keyStore.GetType()}");

            //var certificates = keyStore.ListKeys();
            //_logger.Info("Certificates retrieved successfully");
            //_logger.Trace($"Number of certificates retrieved from {os} key store {certificates.Count()}");

            //return new List<object>();

            return Ok();
        }
    }
}
