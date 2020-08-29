using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Governing.Ethereum
{
    public interface IEthereumActionService
    {
        Task ProcessActionRequestAsync(ActionRequest actionRequest);
    }

    class EthereumActionService : IEthereumActionService
    {
        private readonly List<IEthereumActionProcessor> _ethereumActionProcessors;
        public ILogger<EthereumActionService> Logger { get; set; }

        public EthereumActionService(IEnumerable<IEthereumActionProcessor> ethereumActionProviders)
        {
            _ethereumActionProcessors = ethereumActionProviders.ToList();
        }

        public async Task ProcessActionRequestAsync(ActionRequest actionRequest)
        {
            var actionProcessors = _ethereumActionProcessors.Where(p => p.ActionName == actionRequest.ActionName);
            foreach (var actionProcessor in actionProcessors)
            {
                await actionProcessor.ProcessAsync(actionRequest.Data);
            }
        }
    }
}