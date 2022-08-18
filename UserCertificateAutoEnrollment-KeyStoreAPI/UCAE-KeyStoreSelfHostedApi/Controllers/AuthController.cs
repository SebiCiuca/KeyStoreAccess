using Microsoft.AspNetCore.Mvc;
using UserCertificateAutoEnrollment.BL.Session;

namespace UCAE_KeyStoreSelfHostedApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ISessionProvider _sessionProvider;

        public AuthController(ILogger<AuthController> logger, ISessionProvider sessionProvider)
        {
            _logger = logger;
            _sessionProvider = sessionProvider;
        }

        [HttpGet]
        [Route("CreateSession")]
        public async Task<IActionResult> CreateSessionAsync(string nonceValue)
        {
            if (string.IsNullOrEmpty(nonceValue))
            {
                throw new ArgumentException($"'{nameof(nonceValue)}' cannot be null or empty.", nameof(nonceValue));
            }

            var session = await _sessionProvider.CreateSession(nonceValue);

            return Ok(session);
        }

        [HttpPost]
        [Route("ValidateSession")]
        public IActionResult ValidateSession([FromBody] byte[] sessionKey)
        {
            if (sessionKey is null)
            {
                throw new ArgumentNullException(nameof(sessionKey));
            }

            _sessionProvider.ValidateSession(sessionKey);

            return Ok();
        }
    }
}
