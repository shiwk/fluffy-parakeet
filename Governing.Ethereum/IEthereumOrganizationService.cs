using System.Collections.Generic;
using System.Threading.Tasks;

namespace Governing.Ethereum
{
    public interface IEthereumOrganizationService
    {
        public Task<List<string>> GetOrganizationMembersAsync();
        public Task<int> GetOrganizationVersionAsync();
    }
}