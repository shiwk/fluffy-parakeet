using System;
using System.Threading.Tasks;
using BlockChainKit.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
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

        public ILogger<EthereumChainKit> Logger { get; set; }

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
            try
            {
                var web3 = await GetOrGenerateWeb3Instance();
                var handler = web3.Eth.GetContractTransactionHandler<T>();
                var gas = await handler.EstimateGasAsync(contractAddress, functionMessage);
                // Web3.Convert.
                var gasPrice = new HexBigInteger(UnitConversion.Convert.ToWei(100, UnitConversion.EthUnit.Gwei));
                functionMessage.Gas = gas;
                functionMessage.GasPrice = gasPrice;
                return await handler.SendRequestAsync(contractAddress, functionMessage);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                throw;
            }
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