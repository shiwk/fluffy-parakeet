using AElf.Modularity;
using BlockChainKit.AElf;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Governing.AElf
{
    [DependsOn(typeof(BlockChainKitAElfModule))]
    public class GoverningAElfModule : AElfModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AElfEventListeningOptions>(configuration.GetSection("AElfEventListening"));

            context.Services.AddTransient<IAElfBlockChainService, AElfBlockChainService>();
            context.Services.AddTransient<IAElfLogEventListeningService, AElfLogEventListeningService>();
            context.Services.AddSingleton<IAElfChainKnownHeightProvider, AElfChainKnownHeightProvider>();
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            var aelfChainKit = context.ServiceProvider.GetService<IAElfChainKit>();
            var aelfChainKnownHeightProvider = context.ServiceProvider.GetService<IAElfChainKnownHeightProvider>();

            var libHeight = AsyncHelper.RunSync(() => aelfChainKit.GetChainLibHeightAsync());
            AsyncHelper.RunSync(() => aelfChainKnownHeightProvider.SetKnownBlockHeightAsync(libHeight));
        }
    }
}