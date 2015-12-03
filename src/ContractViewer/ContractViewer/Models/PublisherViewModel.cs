using System.Collections.Generic;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents viewmodel for publisher detail view
    /// </summary>
    public class PublisherViewModel
    {
        public Publisher Publisher { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
    }
}