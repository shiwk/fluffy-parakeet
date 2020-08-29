using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Client.Service;
using AElf.Types;
using BlockChainKit.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockChainKit.AElf
{
    public interface IAElfChainKit
    {
        Task<long> GetChainHeightAsync();
        Task<long> GetChainLibHeightAsync();
        Task<BlockDto> GetBlockByHashAsync(Hash hash);
        Task<BlockDto> GetBlockByHeightAsync(long height);
        Task<TransactionResultDto> GetTransactionResultAsync(Hash txId);
        Task<List<TransactionResultDto>> GetTransactionResultsByBlockHashAsync(Hash blockHash);
        Task<Address> GetAccountAsync();
    }

    class AElfChainKit : IAElfChainKit
    {
        private readonly AElfClient _aelfClient;
        private readonly IKeyPairProvider _keyPairProvider;
        public ILogger<AElfChainKit> Logger { get; set; }
        
        public AElfChainKit(IOptionsSnapshot<AElfProviderOptions> optionsSnapshot, IKeyPairProvider keyPairProvider)
        {
            _keyPairProvider = keyPairProvider;
            _aelfClient = new AElfClient(optionsSnapshot.Value.ProviderHost);
        }

        public async Task<long> GetChainHeightAsync()
        {
            return await _aelfClient.GetBlockHeightAsync();
        }

        public async Task<long> GetChainLibHeightAsync()
        {
            return (await _aelfClient.GetChainStatusAsync()).LastIrreversibleBlockHeight;
        }

        public async Task<BlockDto> GetBlockByHashAsync(Hash hash)
        {
            return await _aelfClient.GetBlockByHashAsync(hash.ToHex());
        }

        public async Task<BlockDto> GetBlockByHeightAsync(long height)
        {
            return await _aelfClient.GetBlockByHeightAsync(height);
        }

        public async Task<TransactionResultDto> GetTransactionResultAsync(Hash txId)
        {
            return await _aelfClient.GetTransactionResultAsync(txId.ToHex());
        }

        public async Task<List<TransactionResultDto>> GetTransactionResultsByBlockHashAsync(Hash blockHash)
        {
            return await _aelfClient.GetTransactionResultsAsync(blockHash.ToHex());
        }

        public async Task<Address> GetAccountAsync()
        {
            return Address.FromPublicKey(await _keyPairProvider.GetPublicKeyAsync());
        }
    }
}