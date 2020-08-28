using System.Collections.Generic;
using AElf.Types;

namespace Governing.AElf
{
    public class AElfEventListeningOptions
    {
        public string ListeningContractAddress { get; set; }
        public string InterestedOrganizationAddress { get; set; }
        public List<string> EventNames { get; set; }
    }
}