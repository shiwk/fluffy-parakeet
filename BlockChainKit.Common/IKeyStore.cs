using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using AElf.Cryptography.Exceptions;
using AElf.OS.Account.Infrastructure;
using Nethereum.KeyStore;
using Nethereum.KeyStore.Crypto;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace BlockChainKit.Common
{
    public interface IKeyStore
    {
        Task<ECKeyPair> GetAccountKeyPairAsync(string address, string password);
    }
    
    public class AElfKeyStore : IKeyStore, ISingletonDependency
    {
        private const string KeyFileExtension = ".json";
        private const string KeyFolderName = "keys";
        private const string ApplicationFolderName = "aelf";
        private string _appDataPath;
        private readonly List<Account> _unlockedAccounts = new List<Account>();
        
        public async Task<ECKeyPair> GetAccountKeyPairAsync(string address, string password)
        {
            var kp = _unlockedAccounts.FirstOrDefault(oa => oa.AccountName == address)?.KeyPair;
            if (kp == null)
            {
                await UnlockAccountAsync(address, password);
                kp = _unlockedAccounts.FirstOrDefault(oa => oa.AccountName == address)?.KeyPair;
            }

            return kp;
        }


        private async Task UnlockAccountAsync(string address, string password)
        {
            var keyPair = await ReadKeyPairAsync(address, password);
            var unlockedAccount = new Account(address) {KeyPair = keyPair};

            _unlockedAccounts.Add(unlockedAccount);
        }
        
        private async Task<ECKeyPair> ReadKeyPairAsync(string address, string password)
        {
            try
            {
                var keyFilePath = GetKeyFileFullPath(address);
                var privateKey = await Task.Run(() =>
                {
                    using (var textReader = File.OpenText(keyFilePath))
                    {
                        var json = textReader.ReadToEnd();
                        return new KeyStoreService().DecryptKeyStoreFromJson(password, json);
                    }
                });

                return CryptoHelper.FromPrivateKey(privateKey);
            }
            catch (FileNotFoundException ex)
            {
                throw new KeyStoreNotFoundException("Keystore file not found.", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new KeyStoreNotFoundException("Invalid keystore path.", ex);
            }
            catch (DecryptionException ex)
            {
                throw new InvalidPasswordException("Invalid password.", ex);
            }
        }

        private string GetKeyFileFullPath(string address)
        {
            var path = GetKeyFileFullPathStrict(address);
            if (!File.Exists(path))
                File.Create(path);
            return path;
        }

        private string GetKeystoreDirectoryPath()
        {
            return Path.Combine(GetAppDataPath(), KeyFolderName);
        }
        
        private string GetKeyFileFullPathStrict(string address)
        {
            var dirPath = GetKeystoreDirectoryPath();
            var filePath = Path.Combine(dirPath, address);
            var filePathWithExtension = Path.ChangeExtension(filePath, KeyFileExtension);
            return filePathWithExtension;
        }

        private string GetAppDataPath()
        {
            if (string.IsNullOrWhiteSpace(_appDataPath))
            {
                _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationFolderName);

                if (!Directory.Exists(_appDataPath))
                {
                    Directory.CreateDirectory(_appDataPath);
                }
            }

            return _appDataPath;
        }
    }
}