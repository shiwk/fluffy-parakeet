using AElf.Modularity;
using AElf.OS;
using BlockChainKit.Common;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BlockChainKit.Ethereum
{
    [DependsOn(typeof(OSAElfModule), typeof(BlockChainKitModule))]
    public class BlockChainKitEthereumModule : AElfModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<EthereumProviderOptions>(configuration.GetSection("EthereumProvider"));
            
            context.Services.AddSingleton<IEthereumChainKit, EthereumChainKit>();
        }
    }
}