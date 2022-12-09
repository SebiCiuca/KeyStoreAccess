using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCAE_KeyStore_TestApp
{
    public interface IMessageProcessor
    {
        public Task<string> ProcessCommand(CommandModel command);
    }
}
