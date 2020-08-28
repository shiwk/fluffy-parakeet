using System.Linq;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using Governing.AElf;
using Governing.Ethereum;
using Governing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Governing
{
    public class BlockChainMonitorJob : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly IAElfBlockChainService _aelfBlockChainService;
        private readonly IAElfLogEventListeningService _aelfLogEventListeningService;
        private readonly ITaskQueueManager _taskQueueManager;
        private readonly IEthereumActionService _ethereumActionService;
        
        public ILogger<BlockChainMonitorJob> Logger { get; set; }

        public BlockChainMonitorJob(AbpTimer timer, IServiceScopeFactory serviceScopeFactory,
            IAElfBlockChainService aelfBlockChainService, ITaskQueueManager taskQueueManager,
            IAElfLogEventListeningService aelfLogEventListeningService,
            IEthereumActionService ethereumActionService) : base(timer, serviceScopeFactory)
        {
            _aelfBlockChainService = aelfBlockChainService;
            _taskQueueManager = taskQueueManager;
            _aelfLogEventListeningService = aelfLogEventListeningService;
            _ethereumActionService = ethereumActionService;
            
            Timer.Period = Constants.MonitorJobTimer;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogDebug("Try to obtain blocks..");
            
            var newBlocks = await _aelfBlockChainService.ObtainBlocksAsync();
            if (newBlocks == null || newBlocks.Count == 0)
                return;
            
            Logger.LogDebug($"Obtained {newBlocks.Count} blocks.");
            foreach (var block in newBlocks)
            {
                _taskQueueManager.Enqueue(async () => await ProcessBlockAsync(block), Constants.EventFilterQueueName);
            }
        }

        private async Task ProcessBlockAsync(BlockDto blockDto)
        {
            var interestedEvents = await _aelfLogEventListeningService.FilterInterestedEventsAsync(blockDto);
            if (interestedEvents == null || interestedEvents.Count == 0)
                return;

            Logger.LogDebug($"Found event {string.Join(",", interestedEvents.Select(e => e.Name).ToList())}.");
            
            foreach (var logEvent in interestedEvents)
            {
                _taskQueueManager.Enqueue(
                    async () =>
                        await _ethereumActionService.ProcessActionRequestAsync(logEvent.ToEthereumActionRequest()),
                    Constants.EventHandleQueueName);
            }
        }
    }
}