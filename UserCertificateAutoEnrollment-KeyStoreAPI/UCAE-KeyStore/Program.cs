using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sentry;
using System.Net.Http.Headers;
using System.Security.Principal;
using UCAE_KeyStore.Helpers;
using Formatting = Newtonsoft.Json.Formatting;

namespace UCAE_KeyStore
{
    internal class Program
    {
        static Logger m_Logger;
        const string NLOG_SESSION_KEY = "SessionKey";
        public static void Main(string[] args)
        {
            m_Logger = LogManager.Setup().GetCurrentClassLogger();
            m_Logger.Info("App Started!");

            try
            {
                m_Logger.Debug("Registering DI");
                m_Logger.Debug("Running process under {0}", WindowsIdentity.GetCurrent().Name);

                var serviceProvider = RegisterDependencies.RegisterAppDependencies();

                m_Logger.Debug("DI Registered");

                using (serviceProvider as IDisposable)
                {
                    var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor>();

                    m_Logger.Debug($"Created message processor. {messageProcessor.GetType()}");

                    JObject command;
                    while ((command = Read()) != null)
                    {
                        var ceCommand = command.ToObject<CommandModel>();

                        if (!string.IsNullOrWhiteSpace(ceCommand.SessionKey))
                        {
                            GlobalDiagnosticsContext.Set(NLOG_SESSION_KEY, ceCommand.SessionKey);
                        }

                        if (ceCommand == null)
                        {
                            m_Logger.Warn("Could not deserialize command from extention");

                            return;
                        }

                        var responseToSend = messageProcessor.ProcessCommand(ceCommand).Result;

                        m_Logger.Debug("Sending response to CE {0}", JsonConvert.SerializeObject(responseToSend));

                        if (!string.IsNullOrEmpty(responseToSend))
                        {
                            Write(responseToSend);
                        }

                        GlobalDiagnosticsContext.Remove(NLOG_SESSION_KEY);
                    }
                }

            }
            catch (Exception ex)
            {
                m_Logger.Error(ex.Message);
                m_Logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                m_Logger.Info("Logger shutting down");

                LogManager.Shutdown();
            }
        }

        public static JObject Read()
        {
            m_Logger.Debug("Read started");
            var stdin = Console.OpenStandardInput();
            m_Logger.Debug("StDin initialized");
            var length = 0;

            var lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);
            m_Logger.Debug($"Message length: {length}");

            var buffer = new char[length];
            using (var reader = new StreamReader(stdin))
            {
                while (reader.Peek() >= 0)
                {
                    reader.Read(buffer, 0, buffer.Length);
                }
            }
            var stringMessage = new string(buffer);

            m_Logger.Info($"Recieved from CE {stringMessage}");

            return JsonConvert.DeserializeObject<JObject>(stringMessage);
        }

        public static void Write(JToken data)
        {
            var json = new JObject();
            json["data"] = data;

            var bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString(Formatting.None));

            var stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
            stdout.Write(bytes, 0, bytes.Length);
            stdout.Flush();
        }
    }
}