using AElf.Modularity;
using AElf.OS;
using BlockChainKit.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BlockChainKit.AElf
{
    [DependsOn(typeof(BlockChainKitModule))]
    public class BlockChainKitAElfModule : AElfModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AccountOptions>(context.Services.GetConfiguration().GetSection("Account"));
            var configuration = context.Services.GetConfiguration();
            Configure<AElfProviderOptions>(configuration.GetSection("AElfProvider"));
            
            context.Services.AddSingleton<IAElfChainKit, AElfChainKit>();
        }
    }
}