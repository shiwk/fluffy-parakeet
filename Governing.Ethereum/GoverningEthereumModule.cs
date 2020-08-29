using AElf.Modularity;
using BlockChainKit.Ethereum;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Governing.Ethereum
{
    [DependsOn(typeof(BlockChainKitEthereumModule))]
    public class GoverningEthereumModule : AElfModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<EthereumMSigContractOptions>(configuration.GetSection("EthereumMSigContract"));

            context.Services.AddTransient<IEthereumActionService, EthereumActionService>();
            context.Services.AddTransient<IEthereumActionProcessor, MemberAddedActionProcessor>();
        }
    }
}