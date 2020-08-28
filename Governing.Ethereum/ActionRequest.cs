using Google.Protobuf;

namespace Governing.Ethereum
{
    public class ActionRequest
    {
        public string ActionName { get; set; }
        public ByteString Data { get; set; }
    }
}