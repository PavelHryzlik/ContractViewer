using System;
using GridMvc.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ContractViewer.Models
{
    public class Version
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [Display(Name = "Číslo verze")]
        [GridColumn(Title = "Číslo verze")]
        public string VersionOrder { get; set; }

        [Display(Name = "Datum publikace")]
        [GridColumn(Title = "Datum publikace", Format = "{0:dd/MM/yyyy hh:mm:ss}")]
        public DateTime Issued { get; set; }

        [Display(Name = "Identifikátor verze")]
        [GridColumn(Title = "Identifikátor verze")]
        public string ContractUri { get; set; }
    }
}