using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    public class Contract
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [NotMappedColumn]
        [Display(Name = "Doména")]
        public string BaseDomain { get; set; }

        [Display(Name = "Id")]
        [GridColumn(Title = "Lokální Id")]
        public string ContractId { get; set; }
        
        [Display(Name = "Verze")]
        [GridColumn(Title = "Verze")]
        public string Version { get; set; }

        [Display(Name = "Publikující")]
        [NotMappedColumn]
        public string Publisher { get; set; }

        [Display(Name = "Předmět")]
        [GridColumn(Title = "Předmět")]
        public string Title { get; set; }

        [Display(Name = "Datum vystavení")]
        [GridColumn(Title = "Datum vystavení", Format = "{0:dd/MM/yyyy}")]
        public DateTime Created { get; set; }

        [Display(Name = "Částka vč. DPH")]
        [GridColumn(Title = "Částka vč. DPH")]
        public string Price { get; set; }
    }
}