using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractViewer.Models
{
    public class IndexViewModel
    {
        public GoogleMapViewModel GoogleMap { get; set; }
        public IEnumerable<Publisher> Subjects { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
    }
}