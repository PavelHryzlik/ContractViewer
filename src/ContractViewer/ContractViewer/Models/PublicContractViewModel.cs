using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents viewmodel for public contracts view
    /// </summary>
    public class PublicContractViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<PublicContract> PublicContract { get; set; }
    }
}