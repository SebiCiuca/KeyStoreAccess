using Microsoft.Extensions.DependencyInjection;
using NLog;
using Sentry.Protocol;
using UCAE_KeyStore_TestApp.Helpers;

namespace UCAE_KeyStore_TestApp
{
    internal class Program
    {
        static Logger m_Logger;
        const string NLOG_SESSION_KEY = "SessionKey";
        static void Main(string[] args)
        {
            m_Logger = LogManager.Setup().GetCurrentClassLogger();
            m_Logger.Info("App Started!");

            try
            {
                var serviceProvider = RegisterDependencies.RegisterAppDependencies();
                using (serviceProvider as IDisposable)
                {
                    var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor>();

                    Console.WriteLine("Command available:");
                    Console.WriteLine("1.Command to get local certificates");
                    Console.WriteLine("4.Get current domain user");
                    Console.WriteLine("6.Ping");
                    Console.WriteLine("7.Exit");

                    while (true)
                    {                     
                        var command = Console.ReadLine();
                        var canBeParsed = int.TryParse(command, out int commandId);

                        if (!canBeParsed)
                        {
                            Console.WriteLine("Command not available");
                        }
                        if (commandId == 1 || commandId == 4 || commandId == 6)
                        {
                            var commandModel = new CommandModel
                            {
                                CommandId = commandId,
                                SessionKey = "filler"
                            };

                            var result = messageProcessor.ProcessCommand(commandModel).Result;

                            Console.WriteLine("Command result");
                            Console.WriteLine(result);
                        }

                        if (commandId == 7)
                        {
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, $"Stopped program because of exception. Exception message{ex.Message}");

                throw;
            }
            finally
            {
                m_Logger.Info("Logger shutting down");

                LogManager.Shutdown();
            }

        }
    }
}