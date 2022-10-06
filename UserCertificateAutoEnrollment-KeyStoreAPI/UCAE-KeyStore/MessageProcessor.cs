using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Targets;
using System.Text.Json;
using UserCertificateAutoEnrollment.BL.KeyStore;

namespace UCAE_KeyStore
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly NLog.Logger m_Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IKeyStoreManager m_KeyStoreManager;
        private const string OS = "windows";

        public MessageProcessor(IKeyStoreFactory keyStoreFactory)
        {
            m_KeyStoreManager = keyStoreFactory.GetKeyStoreManager(OS);
        }

        private async Task<string> ProcessGetCertificatesCommand()
        {
            m_Logger.Debug("Processing get certificates list command");

            var certificates = await m_KeyStoreManager.GetCertificatesAsync();

            m_Logger.Trace($"Found {certificates.ToList().Count} certificates");

            var result = JsonConvert.SerializeObject(certificates);

            return result;
        }

        private async Task<string> ProcessSyncCertificates(byte[] rawData, string sessionKey)
        {
            m_Logger.Debug("Processing sync certificates list command");

            await m_KeyStoreManager.SyncCertificatesAsync(rawData, sessionKey);

            return "Sync done. Check logs for sync result!";
        }

        private async Task<string> ProcessGetLoggedInUser()
        {
            m_Logger.Debug("Processing get logged in user command");

            var loggedInUser = await m_KeyStoreManager.GetLoggedInUser();

            m_Logger.Info("Logged in user {0}", loggedInUser);

            return loggedInUser;
        }

        private async Task<string> ProcessUploadLogs(string sessionKey)
        {
            m_Logger.Debug("Getting logs for upload");

            var fileTarget = LogManager.Configuration.FindTargetByName<FileTarget>("UCAELogSessionFile");


            if (fileTarget != null)
            {

                m_Logger.Info("Found inMemoryTarget");

                // List<string> logs = fileTarget.l.ToList();//.Where(l => l.Contains(sessionKey)).ToList();
                var logs = new List<string>();

                //m_Logger.Info("Found logs", JsonSerializer.Serialize(logs));

                return string.Join("\n", logs);
            }

            return string.Empty;
        }

        public async Task<string> ProcessMessageAsync(CommandModel command)
        {
            string returnedValue = string.Empty;

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


            m_Logger.Trace($"Returning to CE {returnedValue}");

            return returnedValue;
        }

        public async Task<string> ProcessCommand(CommandModel command)
        {
            if (command == null)
            {
                m_Logger.Warn($"Received no data to process. Recieved from CE {JsonConvert.SerializeObject(command)}");

                return string.Empty;

            }

            m_Logger.Debug($"Processing command id: {command.CommandId}");

            return await ProcessMessageAsync(command);
        }
    }
}
