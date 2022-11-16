using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sentry;
using System.Text;
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

                var serviceProvider = RegisterDependencies.RegisterAppDependencies();

                m_Logger.Debug("DI Registered");

                using (serviceProvider as IDisposable)
                {
                    var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor>();

                    m_Logger.Debug($"Created message processor. {messageProcessor.GetType()}");

                    JObject command = Read();
                    if (command != null)
                    {
                        var ceCommand = command.ToObject<CommandModel>();

                        if (ceCommand == null)
                        {
                            m_Logger.Warn("Could not deserialize command from extention");

                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(ceCommand.SessionKey))
                        {
                            GlobalDiagnosticsContext.Set(NLOG_SESSION_KEY, ceCommand.SessionKey);
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
                SentrySdk.CaptureException(ex);
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
            try
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
                m_Logger.Debug($"Buffer lenght: {buffer.Length}");
                using (var reader = new StreamReader(stdin))
                {
                    m_Logger.Debug("Reading from stream...");
                    while (reader.Peek() >= 0)
                    {
                        m_Logger.Debug("We are in reading process...please wait");
                        int result = reader.Read(buffer, 0, buffer.Length);
                        m_Logger.Debug($"Read {result} numer of chars");
                    }
                    m_Logger.Debug("Read finished");
                }
                var stringMessage = new string(buffer);

                m_Logger.Info($"Recieved from CE {stringMessage}");

                return JsonConvert.DeserializeObject<JObject>(stringMessage);
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, $"Error at reading input data");

                return null;
            }
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

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static string ByteToString(byte[] array, int count)
        {
            char[] chars = Encoding.UTF8.GetChars(array, 0, count);

            return new string(chars);
        }

        private static void LogInput(Stream stdin)
        {
            byte[] bytes = new byte[2500];
            byte[] lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes, 0);
            m_Logger.Debug("Message length in byte format: {0}", ByteToString(lengthBytes, 4));

            m_Logger.Debug("Message length: {0}", length);

            byte[] reverse = BitConverter.GetBytes(length);

            m_Logger.Debug("Invers length in byte format: {0}", ByteToString(reverse, 4));

            int outputLength = stdin.Read(bytes, 0, length);

            char[] chars = Encoding.UTF8.GetChars(bytes, 0, outputLength);

            m_Logger.Debug("Full message :{0}", new string(chars));
        }
    }
}