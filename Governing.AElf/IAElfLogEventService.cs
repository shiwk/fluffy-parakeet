using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Types;
using BlockChainKit.AElf;

namespace Governing.AElf
{
    public interface IAElfLogEventListeningService
    {
        Task<List<LogEvent>> FilterInterestedEventsAsync(BlockDto blockDto);
    }

    class AElfLogEventListeningService : IAElfLogEventListeningService
    {
        private readonly IAElfChainKit _aelfChainKit;
        private readonly List<ILogEventListener> _logEventListeners;
        
        public AElfLogEventListeningService(IAElfChainKit aelfChainKit, IEnumerable<ILogEventListener> logEventListeners)
        {
            _aelfChainKit = aelfChainKit;
            _logEventListeners = logEventListeners.ToList();
        }

        public async Task<List<LogEvent>> FilterInterestedEventsAsync(BlockDto blockDto)
        {
            var results = new List<LogEvent>();
            var listeners = _logEventListeners.Where(l => l.FoundInBlock(blockDto)).ToList();
            if (listeners.Count == 0)
                return results;
            
            var txResults = await _aelfChainKit.GetTransactionResultsByBlockHashAsync(Hash.LoadFromHex(blockDto.BlockHash));
            
            foreach (var logEventListener in listeners)
            {
                foreach (var txResult in txResults)
                {
                    var interestedEvents = await logEventListener.FilterFromTransactionResultAsync(txResult);
                    results.AddRange(interestedEvents);
                }
            }

            return results;
        }
    }
}