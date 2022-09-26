using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using UCAE_KeyStore.Helpers;
using Formatting = Newtonsoft.Json.Formatting;

namespace UCAE_KeyStore
{
    internal class Program
    {
        static Logger m_Logger;
        public static void Main(string[] args)
        {
            m_Logger = LogManager.Setup().GetCurrentClassLogger();
            m_Logger.Debug("init main");

            try
            {
                m_Logger.Debug("Registering DI");
                var serviceProvider = RegisterDependencies.RegisterAppDependencies();

                using (serviceProvider as IDisposable)
                {
                    var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor>();

                    m_Logger.Debug($"Cretead message processor. {messageProcessor.GetType()}");

                    JObject command;
                    while ((command = Read()) != null)
                    {
                        m_Logger.Debug($"Recieved from CE {command}");
                        var responseToSend = messageProcessor.ProcessCommand(command).Result;
                        if (!string.IsNullOrEmpty(responseToSend))
                        {
                            Write(responseToSend);
                        }
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
                NLog.LogManager.Shutdown();
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
            m_Logger.Debug(buffer);

            return JsonConvert.DeserializeObject<JObject>(new string(buffer));
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