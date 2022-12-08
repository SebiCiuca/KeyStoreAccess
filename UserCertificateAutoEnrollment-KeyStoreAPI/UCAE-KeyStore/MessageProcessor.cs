using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sentry;
using UCAE_KeyStore.SessionManager;
using UserCertificateAutoEnrollment.BL.KeyStore;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace UCAE_KeyStore
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IKeyStoreManager m_KeyStoreManager;
        private readonly ISessionManager m_SessionManager;
        private readonly ILogger m_Logger;

        public MessageProcessor(IKeyStoreFactory keyStoreFactory, ISessionManager sessionManager, ILogger<MessageProcessor> logger)
        {
            m_KeyStoreManager = keyStoreFactory.GetKeyStoreManager(Helpers.Constants.WINDOWS_OS);
            m_SessionManager = sessionManager;
            m_Logger = logger;
        }

        private async Task<string> ProcessGetCertificatesCommand()
        {
            m_Logger.LogDebug("Processing get certificates list command");

            var certificates = await m_KeyStoreManager.GetCertificatesAsync();

            m_Logger.LogTrace($"Found {certificates.LocalCertificates?.Count()} certificates");

            return JsonConvert.SerializeObject(certificates);
        }

        private async Task<string> ProcessSyncCertificates(string commandValue, string sessionKey)
        {
            m_Logger.LogDebug("Processing sync certificates list command");

            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                m_Logger.LogWarning($"Could not sync certificates {nameof(sessionKey)} missing");

                return $"Missing {nameof(sessionKey)} can't process certificates";
            }

            await m_KeyStoreManager.SyncCertificatesAsync(commandValue, sessionKey);

            return "Sync done. Check logs for sync result!";
        }

        private async Task<string> ProcessGetLoggedInUser()
        {
            m_Logger.LogDebug("Processing get logged in user command");

            var loggedInUser = await m_KeyStoreManager.GetLoggedInUser();

            m_Logger.LogInformation("Logged in user {0}", loggedInUser);

            return loggedInUser;
        }

        private async Task<string> ProcessUploadLogs(string sessionKey)
        {
            m_Logger.LogDebug("Getting logs for upload");

            var fileTarget = SessionLogger.GetLogFileName("UCAELogSessionFile");

            if (fileTarget != null)
            {
                m_Logger.LogInformation($"Found file with target LOG_{sessionKey}");

                using var f = new FileStream(fileTarget, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var s = new StreamReader(f);

                var logs = s.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(logs))
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(logs); 
                    
                    return System.Convert.ToBase64String(plainTextBytes);
                }
            }

            return string.Empty;
        }

        public async Task<string> ProcessMessageAsync(CommandModel command)
        {
            string returnedValue;
            switch (command.CommandId)
            {
                case 1:
                    returnedValue = await ProcessGetCertificatesCommand();
                    break;
                case 2:
                    returnedValue = await ProcessSyncCertificates(command.CommandValue, command.SessionKey);
                    break;
                case 3:
                    returnedValue = "Update auth certificate was called";
                    break;
                case 4:
                    returnedValue = await ProcessGetLoggedInUser();
                    break;
                case 5:
                    returnedValue = await ProcessUploadLogs(command.SessionKey);
                    break;
                case 6:
                    returnedValue = DateTime.UtcNow.ToString();
                    break;
                default:
                    returnedValue = "Unknown command, please try again";
                    break;
            }

            m_Logger.LogTrace($"Returning to CE {returnedValue}");

            return returnedValue;
        }

        public async Task<string> ProcessCommand(CommandModel command)
        {
            if (command == null)
            {
                m_Logger.LogWarning(
                    $"Received no data to process. Recieved from CE {JsonConvert.SerializeObject(command)}");

                SentrySdk.CaptureEvent(new SentryEvent
                {
                    Level = SentryLevel.Info,
                    Message = $"Received no data to process. Recieved from CE {JsonConvert.SerializeObject(command)}"
                });

                return string.Empty;

            }

            m_Logger.LogDebug($"Processing command id: {command.CommandId}");

            string tranName = "GetUsername";

            if (command.SessionKey != null)
            {
                tranName = command.SessionKey;
            }

            var span = m_SessionManager.GetTransaction(command.SessionKey, command.CommandId);

            try
            {
                User user = new User();

                user.Username = await m_KeyStoreManager.GetLoggedInUser();
                user.Email = await m_KeyStoreManager.GetEmail();

                m_Logger.LogDebug($"Configure scope for Sentry: {user}");

                SentrySdk.ConfigureScope(s =>
                {
                    s.SetTag("SessionKey", command.SessionKey);
                    s.User = user;
                });

                m_Logger.LogDebug($"Scope configured successfully!");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }

            SentrySdk.CaptureEvent(new SentryEvent
            {
                Level = SentryLevel.Info,
                Message = $"Processing command {JsonConvert.SerializeObject(command)}"
            });

            try
            {
                var result = await ProcessMessageAsync(command);

                return result;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
            finally
            {
                span.Finish();
                m_SessionManager.FinishTransaction();
            }

            return string.Empty;
        }
    }
}
