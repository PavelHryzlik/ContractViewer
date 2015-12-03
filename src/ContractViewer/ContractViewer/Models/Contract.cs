using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents contract entity
    /// </summary>
    public class Contract
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [NotMappedColumn]
        [Display(Name = "Doména")]
        public string BaseDomain { get; set; }

        [Display(Name = "Id")]
        [GridColumn(Title = "Lok. Id")]
        public string ContractId { get; set; }

        [Display(Name = "Verze")]
        [GridColumn(Title = "Verze")]
        public string Version { get; set; }

        [Display(Name = "Publikující")]
        [NotMappedColumn]
        public string Publisher { get; set; }

        [NotMappedColumn]
        public string PublisherId { get; set; }

        [Display(Name = "Dokument")]
        [NotMappedColumn]
        public string Document { get; set; }

        [Display(Name = "Zodpovědné osoby")]
        [NotMappedColumn]
        public IEnumerable<string> ResponsiblePersons { get; set; }

        [Display(Name = "Evidenční číslo veřejné zakázky")]
        [NotMappedColumn]
        public string AwardId { get; set; }

        [NotMappedColumn]
        public string AwardUrl { get; set; }

        [Display(Name = "Předmět")]
        [GridColumn(Title = "Předmět")]
        public string Title { get; set; }

        [Display(Name = "Typ smlouvy")]
        [GridColumn(Title = "Typ smlouvy")]
        public string ContractType { get; set; }

        [Display(Name = "Popis předmětu smlouvy")]
        [NotMappedColumn]
        public string Description { get; set; }

        [Display(Name = "Datum vystavení")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [GridColumn(Title = "Datum vystavení", Format = "{0:dd/MM/yyyy}")]
        public DateTime DateSigned { get; set; }

        [Display(Name = "Datum účinnosti smlouvy")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [GridColumn(Title = "Datum účinnosti", Format = "{0:dd/MM/yyyy}")]
        public DateTime? ValidFrom { get; set; }

        [Display(Name = "Datum ukončení účinnosti smlouvy")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [GridColumn(Title = "Konec účinnosti", Format = "{0:dd/MM/yyyy}")]
        public DateTime? ValidUntil { get; set; }

        [Display(Name = "Kompetence")]
        [NotMappedColumn]
        public string Competency { get; set; }

        [Display(Name = "Anonymizováno")]
        [NotMappedColumn]
        public string Anonymised { get; set; }

        [Display(Name = "Částka bez DPH")]
        [NotMappedColumn]
        public string AmountNoVat { get; set; }

        [Display(Name = "Částka vč. DPH")]
        [GridColumn(Title = "Částka vč. DPH")]
        public string Amount { get; set; }

        [Display(Name = "Měna")]
        [NotMappedColumn]
        public string Currency { get; set; }
    }
}