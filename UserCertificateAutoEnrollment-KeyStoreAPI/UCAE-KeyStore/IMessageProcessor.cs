using Newtonsoft.Json.Linq;

namespace UCAE_KeyStore
{
    public interface IMessageProcessor
    {
        public Task<string> ProcessCommand(JObject command);
    }
}
