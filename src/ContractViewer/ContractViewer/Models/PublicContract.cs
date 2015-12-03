using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents party entity
    /// </summary>
    public class PublicContract
    {
        [Display(Name = "Dodavatel IČ")]
        [GridColumn(Title = "Dodavatel IČ")]
        public string Id { get; set; }

        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [NotMappedColumn]
        [Display(Name = "Evidenční číslo ISVZUS")]
        public string EvNumber { get; set; }

        [Display(Name = "Identifikátor zakázky")]
        [GridColumn(Title = "Identifikátor zakázky")]
        public string ContractId { get; set; }

        [Display(Name = "Předmět")]
        [GridColumn(Title = "Předmět")]
        public string Title { get; set; }

        [Display(Name = "Dodavatel")]
        [GridColumn(Title = "Dodavatel")]
        [NotMappedColumn]
        public string Supplier { get; set; }

        [NotMappedColumn]
        public string SupplierUri { get; set; }

        [Display(Name = "Částka bez DPH")]
        [NotMappedColumn]
        public string AmountNoVat { get; set; }

        [Display(Name = "Částka vč. DPH")]
        [GridColumn(Title = "Částka vč. DPH")]
        public string Amount { get; set; }
    }
}