using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using Sentry;
using UCAE_KeyStore.SessionManager;
using UserCertificateAutoEnrollment.BL.KeyStore;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace UCAE_KeyStore
{
    public class MessageProcessor : IMessageProcessor
    {
        //private readonly NLog.Logger m_Logger = LogManager.GetCurrentClassLogger();
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

            m_Logger.LogTrace($"Found {certificates.LocalCertificates.ToList().Count} certificates");

            var result = JsonConvert.SerializeObject(certificates);

            return result;
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
            var user = await m_KeyStoreManager.GetEmail();

            SentrySdk.CaptureMessage($"E-mail of logged in user is {user}");

            m_Logger.LogInformation("Logged in user {0}", loggedInUser);

            return loggedInUser;
        }

        private async Task<string> ProcessUploadLogs(string sessionKey)
        {
            m_Logger.LogDebug("Getting logs for upload");

            var fileTarget = LogManager.Configuration.FindTargetByName<FileTarget>($"LOG_{sessionKey}");

            if (fileTarget != null)
            {
                m_Logger.LogInformation($"Found file with target LOG_{sessionKey}");

                var logs = new List<string>();

                return string.Join("\n", logs);
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
                m_Logger.LogWarning($"Received no data to process. Recieved from CE {JsonConvert.SerializeObject(command)}");

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

            SentrySdk.ConfigureScope(async s =>
            {
                s.SetTag("SessionKey", command.SessionKey);
                s.User = new User
                {
                    Username = await m_KeyStoreManager.GetLoggedInUser(),
                    Email = await m_KeyStoreManager.GetEmail()
                };
            });

            SentrySdk.CaptureEvent(new SentryEvent
            {
                Level = SentryLevel.Info,
                Message = $"Processing command {JsonConvert.SerializeObject(command)}"
            });


            var result = await ProcessMessageAsync(command);

            span.Finish();
            m_SessionManager.FinishTransaction();

            return result;
        }
    }
}
