using System;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents party entity
    /// </summary>
    public class Party
    {
        [NotMappedColumn]
        [Display(Name = "Adresa zdroje")]
        public string Uri { get; set; }

        [Display(Name = "IČ")]
        [NotMappedColumn]
        public string Id { get; set; }

        [Display(Name = "Lokální Id")]
        [GridColumn(Title = "Lokální Id")]
        public string LocalId { get; set; }

        [Display(Name = "Název")]
        [GridColumn(Title = "Název")]
        public string Name { get; set; }

        [Display(Name = "Stát")]
        [GridColumn(Title = "Stát")]
        public string Country { get; set; }

        [Display(Name = "Ulice")]
        [GridColumn(Title = "Ulice")]
        public string StreetAddres { get; set; }

        [Display(Name = "Město")]
        [GridColumn(Title = "Město")]
        public string Locality { get; set; }

        [Display(Name = "PSČ")]
        [GridColumn(Title = "PSČ")]
        public string PostalCode { get; set; }

        [Display(Name = "Plátce DPH")]
        [GridColumn(Title = "Plátce DPH")]
        public string PaysVat { get; set; }

        [Display(Name = "Adresa")]
        [GridColumn(Title = "Adresa")]
        public string Address 
        { 
            get
            {
                if (!String.IsNullOrEmpty(StreetAddres) && !String.IsNullOrEmpty(PostalCode) && !String.IsNullOrEmpty(Locality))
                    return StreetAddres + " " + PostalCode + " " + Locality;
                return Address;
            }
        }
    }
}