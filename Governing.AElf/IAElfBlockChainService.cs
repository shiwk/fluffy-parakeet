using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Client.Dto;
using BlockChainKit.AElf;

namespace Governing.AElf
{
    public interface IAElfBlockChainService
    {
        Task<List<BlockDto>> ObtainBlocksAsync();
    }

    class AElfBlockChainService : IAElfBlockChainService
    {
        private readonly IAElfChainKnownHeightProvider _aelfChainKnownHeightProvider;
        private readonly IAElfChainKit _aelfChainKit;

        public AElfBlockChainService(IAElfChainKnownHeightProvider aelfChainKnownHeightProvider, IAElfChainKit aelfChainKit)
        {
            _aelfChainKnownHeightProvider = aelfChainKnownHeightProvider;
            _aelfChainKit = aelfChainKit;
        }

        public async Task<List<BlockDto>> ObtainBlocksAsync()
        {
            var res = new List<BlockDto>();
            var knownHeight = await _aelfChainKnownHeightProvider.GetKnownBlockHeightAsync();
            var libHeight = await _aelfChainKit.GetChainLibHeightAsync();

            if (libHeight <= knownHeight)
                return res;

            for (long height = knownHeight + 1; height <= libHeight; height++)
            {
                var blockDto = await _aelfChainKit.GetBlockByHeightAsync(height);
                res.Add(blockDto);
            }

            await _aelfChainKnownHeightProvider.SetKnownBlockHeightAsync(libHeight);
            return res;
        }
    }
}