using GridMvc.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ContractViewer.Models
{
    public class Version
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }
    }
}