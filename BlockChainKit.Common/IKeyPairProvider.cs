using System.Threading.Tasks;
using AElf.Cryptography.ECDSA;
using AElf.OS;
using AElf.OS.Account.Infrastructure;
using Microsoft.Extensions.Options;

namespace BlockChainKit.Common
{
    public interface IKeyPairProvider
    {
        public Task<byte[]> GetPublicKeyAsync();
        public Task<byte[]> GetPrivateKeyAsync();
    }

    class KeyPairProvider : IKeyPairProvider
    {
        private readonly AccountOptions _accountOptions;
        private readonly IKeyStore _keyStore;

        public KeyPairProvider(IKeyStore keyStore, IOptionsSnapshot<AccountOptions> options)
        {
            _keyStore = keyStore;
            _accountOptions = options.Value;
        }

        public async Task<byte[]> GetPublicKeyAsync()
        {
            return (await ReadKeyPairAsync()).PublicKey;
        }

        public async Task<byte[]> GetPrivateKeyAsync()
        {
            return (await ReadKeyPairAsync()).PrivateKey;
        }

        private async Task<ECKeyPair> ReadKeyPairAsync()
        {
            var nodeAccount = _accountOptions.NodeAccount;
            var nodePassword = _accountOptions.NodeAccountPassword ?? string.Empty;
            var accountKeyPair = _keyStore.GetAccountKeyPair(nodeAccount);
            if (accountKeyPair == null)
            {
                await _keyStore.UnlockAccountAsync(nodeAccount, nodePassword);
                accountKeyPair = _keyStore.GetAccountKeyPair(nodeAccount);
            }

            return accountKeyPair;
        }
    }
}