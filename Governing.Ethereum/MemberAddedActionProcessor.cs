using System.Threading.Tasks;
using AElf.Kernel;
using BlockChainKit.Ethereum;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Volo.Abp.DependencyInjection;

namespace Governing.Ethereum
{
    public class MemberAddedActionProcessor : IEthereumActionProcessor, ITransientDependency
    {
        public string ActionName => "MemberAdded";
        private readonly EthereumMSigContractOptions _ethereumMSigContractOptions;
        private readonly IEthereumChainKit _ethereumChainKit;

        public ILogger<MemberAddedActionProcessor> Logger { get; set; }
        
        public MemberAddedActionProcessor(IOptionsSnapshot<EthereumMSigContractOptions> optionsSnapShot,
            IEthereumChainKit ethereumChainKit)
        {
            _ethereumChainKit = ethereumChainKit;
            _ethereumMSigContractOptions = optionsSnapShot.Value;
        }

        public async Task ProcessAsync(ByteString actionRequestData)
        {
            var account = await _ethereumChainKit.GetAccountAsync();
            var proposalCreationMessage = new ProposalCreationFunctionMessage
            {
                AmountToSend = 0,
                Data = new MemberAddFunctionMessage
                {
                    Member = account.Address
                }.GetCallData(),
                Expiration = TimestampHelper.GetUtcNow().Seconds + Constants.MemberAddedProposalExpiration,
                Value = 0,
                To = _ethereumMSigContractOptions.ContractAddress
            };

            var response =
                await _ethereumChainKit.SendAsync(_ethereumMSigContractOptions.ContractAddress, proposalCreationMessage);
            Logger.LogInformation($"MemberAdded Processing response: {response}");
        }
    }
    
    [Function("addMember")]
    public class MemberAddFunctionMessage : FunctionMessage
    {
        [Parameter("address", "_member", 1)]
        public string Member { get; set; }
    }
}