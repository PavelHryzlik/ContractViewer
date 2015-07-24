using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GridMvc.DataAnnotations;
using VDS.RDF.Query.Algebra;

namespace ContractViewer.Models
{
    public class Publisher
    {
        [Display(Name = "Název")]
        [GridColumn(Title = "Název")]
        public string Name { get; set; }

        [NotMappedColumn]
        public string ImageUrl { get; set; }
    }
}