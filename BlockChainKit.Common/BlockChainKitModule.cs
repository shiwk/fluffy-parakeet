using AElf.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace BlockChainKit.Common
{
    public class BlockChainKitModule : AElfModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IKeyPairProvider, KeyPairProvider>();
        }
    }
}