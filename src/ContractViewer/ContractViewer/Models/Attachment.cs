using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    public class Attachment
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [NotMappedColumn]
        [Display(Name = "Doména")]
        public string BaseDomain { get; set; }

        [Display(Name = "Dokument")]
        [GridColumn(Title = "Dokument")]
        public string Document { get; set; }

        [Display(Name = "Id")]
        [GridColumn(Title = "Lokální Id")]
        public string AttachmentId { get; set; }

        [Display(Name = "Předmět")]
        [GridColumn(Title = "Předmět")]
        public string Title { get; set; }

        [Display(Name = "Číslo přílohy")]
        [GridColumn(Title = "Číslo přílohy")]
        public string Number { get; set; }
    }
}