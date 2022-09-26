
using Newtonsoft.Json.Linq;
using System.Text.Json;
using UserCertificateAutoEnrollment.BL.Common;
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

            m_Logger.Debug($"Found {certificates.ToList().Count}");

            var result = JsonSerializer.Serialize(certificates);

            return result;
        }

        public async Task<string> ProcessMessageAsync(JObject data)
        {
            if (data == null)
            {
                m_Logger.Trace("Received no data to process");

                return string.Empty;

            }
            m_Logger.Trace("Received data from CE: {0}", data.ToString());

            CommandModel command = data.ToObject<CommandModel>();

            m_Logger.Trace($"Received command {command.CommandId}");

            string returnedValue = string.Empty;

            switch (command.CommandId)
            {
                case 1:
                    returnedValue = await ProcessGetCertificatesCommand();
                    break;
                case 2:
                    returnedValue = "Process certificates was called";
                    break;
                case 3:
                    returnedValue = "Update auth certificate was called";
                    break;
                default:
                    returnedValue = "Unknown command, please try again";
                    break;
            }


            m_Logger.Trace($"Returning to CE {returnedValue}");

            return returnedValue;
        }

        public async Task<string> ProcessCommand(JObject command)
        {
            m_Logger.Debug($"Processing command {command}");

            return await ProcessMessageAsync(command);
        }


    }
}
