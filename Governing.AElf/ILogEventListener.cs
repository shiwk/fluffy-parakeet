using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.CSharp.Core;
using AElf.Kernel;
using AElf.Types;
using BlockChainKit.AElf;
using Google.Protobuf;

namespace Governing.AElf
{
    public interface ILogEventListener
    {
        bool FoundInBlock(BlockDto block);
        Task<List<LogEvent>> FilterFromTransactionResultAsync(TransactionResultDto transactionResult);
    }

    public interface ILogEventListener<T> : ILogEventListener where T : IEvent<T>
    {
    }

    abstract class LogEventListenerBase<T> : ILogEventListener<T> where T : IEvent<T>, new()
    {
        protected readonly AElfEventListeningOptions AElfEventListeningOptions;
        protected readonly IAElfChainKit AElfChainKit;

        protected LogEventListenerBase(AElfEventListeningOptions aElfEventListeningOptions, IAElfChainKit aelfChainKit)
        {
            AElfEventListeningOptions = aElfEventListeningOptions;
            AElfChainKit = aelfChainKit;
        }

        protected InterestedEvent ListeningEvent { get; set; }

        public bool FoundInBlock(BlockDto block)
        {
            var expectedBloom = ListeningEvent.LogEvent.GetBloom();
            var blockBloom = new Bloom(ByteString.FromBase64(block.Header.Bloom).ToByteArray());
            return expectedBloom.IsIn(blockBloom);
        }

        public async Task<List<LogEvent>> FilterFromTransactionResultAsync(TransactionResultDto transactionResult)
        {
            var interestedEvents = new List<LogEvent>();

            var resultBloom = new Bloom(ByteString.FromBase64(transactionResult.Bloom).ToByteArray());

            if (!ListeningEvent.Bloom.IsIn(resultBloom))
            {
                // not found in the transaction result
                return interestedEvents;
            }

            foreach (var log in transactionResult.Logs)
            {
                var logEvent = new LogEvent
                {
                    Name = log.Name,
                    Address = Address.FromBase58(log.Address),
                    NonIndexed = ByteString.FromBase64(log.NonIndexed),
                    Indexed = {log.Indexed.Select(ByteString.FromBase64)}
                };

                if (logEvent.Address != ListeningEvent.LogEvent.Address ||
                    logEvent.Name != ListeningEvent.LogEvent.Name || !await IsInterestedEvent(logEvent))
                    continue;
                interestedEvents.Add(logEvent);
            }

            return interestedEvents;
        }

        protected abstract Task<bool> IsInterestedEvent(LogEvent log);
    }

    public class InterestedEvent
    {
        public LogEvent LogEvent { get; set; }
        public Bloom Bloom { get; set; }
    }
}