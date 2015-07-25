using System.Collections.Generic;

namespace ContractViewer.Models
{
    public class PublisherViewModel
    {
        public Publisher Publisher { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
    }
}