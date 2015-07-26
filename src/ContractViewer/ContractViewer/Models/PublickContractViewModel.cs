using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractViewer.Models
{
    public class PublickContractViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<PublicContract> PublicContract { get; set; }
    }
}