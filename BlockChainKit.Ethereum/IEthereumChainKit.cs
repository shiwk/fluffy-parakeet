using System.Threading.Tasks;
using BlockChainKit.Common;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace BlockChainKit.Ethereum
{
    public interface IEthereumChainKit
    {
        public Task<Account> GetAccountAsync();
        public Task<string> SendAsync<T>(string contractAddress, T functionMessage) where T : FunctionMessage, new();
    }

    class EthereumChainKit : IEthereumChainKit
    {
        private Web3 _web3Instance;
        private readonly IKeyPairProvider _keyPairProvider;
        private readonly EthereumProviderOptions _ethereumProviderOptions;


        public EthereumChainKit(IOptionsSnapshot<EthereumProviderOptions> optionsSnapshot,
            IKeyPairProvider keyPairProvider)
        {
            _keyPairProvider = keyPairProvider;
            _ethereumProviderOptions = optionsSnapshot.Value;
        }

        public async Task<Account> GetAccountAsync()
        {
            var privateKey = await _keyPairProvider.GetPrivateKeyAsync();
            Account account = new Account(privateKey);
            return account;
        }

        public async Task<string> SendAsync<T>(string contractAddress, T functionMessage) where T : FunctionMessage, new()
        {
            var web3 = await GetOrGenerateWeb3Instance();
            var handler = web3.Eth.GetContractTransactionHandler<T>();
            return await handler.SendRequestAsync(contractAddress, functionMessage);
        }

        private async Task<Web3> GetOrGenerateWeb3Instance()
        {
            if (_web3Instance != null)
                return _web3Instance;
            var account = await GetAccountAsync();
            _web3Instance = new Web3(account, _ethereumProviderOptions.ProviderHost);

            return _web3Instance;
        }
    }
}