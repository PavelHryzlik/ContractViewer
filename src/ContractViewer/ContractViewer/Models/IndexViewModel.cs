using System.Collections.Generic;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents viewmodel for main page
    /// </summary>
    public class IndexViewModel
    {
        public GoogleMapViewModel GoogleMap { get; set; }
        public IEnumerable<Publisher> Subjects { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
    }
}