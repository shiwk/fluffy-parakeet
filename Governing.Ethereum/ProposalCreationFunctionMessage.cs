using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Governing.Ethereum
{
    public class ProposalCreationFunctionMessage : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public string To { get; set; }

        [Parameter("uint256", "_value", 2)]
        public BigInteger Value { get; set; }
        
        [Parameter("bytes", "_data", 3)]
        public byte[] Data { get; set; }
        
        [Parameter("uint256", "_expiration", 4)]
        public BigInteger Expiration { get; set; }
    }
}