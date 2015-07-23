using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractViewer.Models
{
    public class PublisherViewModel
    {
        public Publisher Publisher { get; set; }
        public IEnumerable<Contract> Contracts { get; set; }
    }
}