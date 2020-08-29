using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Governing.AElf
{
    public interface IAElfChainKnownHeightProvider
    {
        Task<long> GetKnownBlockHeightAsync();
        Task SetKnownBlockHeightAsync(long height);
    }

    class AElfChainKnownHeightProvider : IAElfChainKnownHeightProvider
    {
        private long _knownHeight;
        public ILogger<AElfChainKnownHeightProvider> Logger { get; set; }

        public Task<long> GetKnownBlockHeightAsync()
        {
            return Task.FromResult(_knownHeight);
        }

        public Task SetKnownBlockHeightAsync(long height)
        {
            _knownHeight = height;
            Logger.LogDebug($"Known height set to {height}");
            return Task.CompletedTask;
        }
    }
}