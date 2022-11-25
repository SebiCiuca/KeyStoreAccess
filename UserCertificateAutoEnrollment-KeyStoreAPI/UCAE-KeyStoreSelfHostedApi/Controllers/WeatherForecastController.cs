using Microsoft.AspNetCore.Mvc;
using NLog.Targets;
using NLog;
using NLog.Targets.Wrappers;

namespace UCAE_KeyStoreSelfHostedApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        const string NLOG_SESSION_KEY = "SessionKey";
        [HttpPost]
        public async Task<IActionResult> GetLogsAsync(string sessionKey)
        {
            GlobalDiagnosticsContext.Set(NLOG_SESSION_KEY, sessionKey);

            _logger.LogDebug("Getting logs for upload");

            var sessionLogFileName = GetLogFileName("UCAELogSessionFile");

            GlobalDiagnosticsContext.Remove(NLOG_SESSION_KEY);

            var genericLogFileName = GetLogFileName("UCAELogSessionFile");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(sessionLogFileName);

            if (sessionLogFileName != null)
            {
                _logger.LogInformation($"Found file with target LOG_{sessionKey}");

                var logs = new List<string>();

                var logsString = string.Join("\n", logs);

                return Ok(logsString);
            }

            return NotFound();
        }

        private string GetLogFileName(string targetName)
        {
            string fileName = null;

            if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
            {
                Target target = LogManager.Configuration.FindTargetByName(targetName);
                if (target == null)
                {
                    throw new Exception("Could not find target named: " + targetName);
                }

                FileTarget fileTarget = null;
                WrapperTargetBase wrapperTarget = target as WrapperTargetBase;

                // Unwrap the target if necessary.
                if (wrapperTarget == null)
                {
                    fileTarget = target as FileTarget;
                }
                else
                {
                    fileTarget = wrapperTarget.WrappedTarget as FileTarget;
                }

                if (fileTarget == null)
                {
                    throw new Exception("Could not get a FileTarget from " + target.GetType());
                }

                //var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                fileName = fileTarget.FileName.Render(LogEventInfo.CreateNullEvent());
            }
            else
            {
                throw new Exception("LogManager contains no Configuration or there are no named targets");
            }

            if (!System.IO.File.Exists(fileName))
            {
                throw new Exception("File " + fileName + " does not exist");
            }

            return fileName;
        }
    }
}