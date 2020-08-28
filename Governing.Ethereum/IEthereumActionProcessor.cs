using System.Threading.Tasks;
using Google.Protobuf;

namespace Governing.Ethereum
{
    public interface IEthereumActionProcessor
    {
        string ActionName { get; }
        Task ProcessAsync(ByteString actionRequestData);
    }
}