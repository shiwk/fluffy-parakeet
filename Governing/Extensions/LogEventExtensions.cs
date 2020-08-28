using AElf.Types;
using Governing.Ethereum;

namespace Governing.Extensions
{
    public static class LogEventExtensions
    {
        public static ActionRequest ToEthereumActionRequest(this LogEvent logEvent)
        {
            return new ActionRequest
            {
                ActionName = logEvent.Name,
                Data = logEvent.NonIndexed
            };
        }
    }
}