using System;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents amendment entity
    /// </summary>
    public class Amendment
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [NotMappedColumn]
        [Display(Name = "Doména")]
        public string BaseDomain { get; set; }

        [Display(Name = "Dokument")]
        [NotMappedColumn]
        public string Document { get; set; }

        [Display(Name = "Id")]
        [GridColumn(Title = "Lokální Id")]
        public int AmendmentId { get; set; }

        [Display(Name = "Číslo dodatku")]
        [GridColumn(Title = "Číslo dodatku")]
        public string Number { get; set; }

        [Display(Name = "Předmět")]
        [GridColumn(Title = "Předmět")]
        public string Title { get; set; }

        [Display(Name = "Datum podpisu")]
        [GridColumn(Title = "Datum podpisu", Format = "{0:dd/MM/yyyy}")]
        public DateTime DateSigned { get; set; } 
    }
}