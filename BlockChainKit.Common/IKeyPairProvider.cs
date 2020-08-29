using System.Threading.Tasks;
using AElf.Cryptography.ECDSA;
using AElf.OS;
using Microsoft.Extensions.Options;

namespace BlockChainKit.Common
{
    public interface IKeyPairProvider
    {
        public Task<byte[]> GetPublicKeyAsync();
        public Task<byte[]> GetPrivateKeyAsync();
    }

    public class KeyPairProvider : IKeyPairProvider
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
            var keyPair = await GetKeyPairAsync();
            return keyPair.PublicKey;
        }

        public async Task<byte[]> GetPrivateKeyAsync()
        {
            var keyPair = await GetKeyPairAsync();
            return keyPair.PrivateKey;
        }

        private async Task<ECKeyPair> GetKeyPairAsync()
        {
            return await _keyStore.GetAccountKeyPairAsync(_accountOptions.NodeAccount,
                _accountOptions.NodeAccountPassword);
        }
    }
}