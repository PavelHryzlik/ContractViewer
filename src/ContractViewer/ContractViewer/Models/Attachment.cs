using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents attachment entity
    /// </summary>
    public class Attachment
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
        public int AttachmentId { get; set; }

        [Display(Name = "Předmět")]
        [GridColumn(Title = "Předmět")]
        public string Title { get; set; }

        [Display(Name = "Číslo přílohy")]
        [GridColumn(Title = "Číslo přílohy")]
        public int Number { get; set; }
    }
}