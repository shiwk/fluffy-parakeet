using AElf;
using AElf.Modularity;
using Governing.AElf;
using Governing.Ethereum;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Governing
{
    [DependsOn(typeof(GoverningAElfModule), typeof(GoverningEthereumModule), typeof(AbpAspNetCoreModule))]
    public class GoverningModule : AElfModule
    {
        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            var backgroundWorkerManager = context.ServiceProvider.GetRequiredService<IBackgroundWorkerManager>();
            backgroundWorkerManager.Add(context.ServiceProvider.GetService<BlockChainMonitorJob>());
        }
    }
}