using System;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents milestone entity
    /// </summary>
    public class Milestone
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [Display(Name = "Název")]
        [GridColumn(Title = "Název")]
        public string Title { get; set; }

        [Display(Name = "Datum")]
        [GridColumn(Title = "Datum", Format = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }
    }
}