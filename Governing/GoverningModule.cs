using AElf;
using AElf.Modularity;
using AElf.RuntimeSetup;
using Governing.AElf;
using Governing.Ethereum;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Governing
{
    [DependsOn(typeof(AbpBackgroundWorkersModule),
        typeof(GoverningAElfModule), typeof(GoverningEthereumModule), typeof(AbpAspNetCoreModule), typeof(RuntimeSetupAElfModule))]
    public class GoverningModule : AElfModule
    {
        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            var taskQueueManager = context.ServiceProvider.GetService<ITaskQueueManager>();

            taskQueueManager.CreateQueue(Constants.EventFilterQueueName);
            taskQueueManager.CreateQueue(Constants.EventHandleQueueName);
            
            var backgroundWorkerManager = context.ServiceProvider.GetRequiredService<IBackgroundWorkerManager>();
            backgroundWorkerManager.Add(context.ServiceProvider.GetService<BlockChainMonitorJob>());
        }
    }
}