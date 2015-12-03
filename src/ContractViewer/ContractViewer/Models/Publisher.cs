using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    /// <summary>
    /// Class represents publisher entity
    /// </summary>
    public class Publisher
    {
        [Display(Name = "IČ")]
        [GridColumn(Title = "IČ")]
        public string Id { get; set; }

        [Display(Name = "Název")]
        [GridColumn(Title = "Název")]
        public string Name { get; set; }

        [NotMappedColumn]
        public ICollection<LocalPlace> LocalPlaces { get; set; }

        [NotMappedColumn]
        public string Url { get; set; }

        [NotMappedColumn]
        public string ImageUrl { get; set; }

        [NotMappedColumn]
        public string AresUrl { get; set; }

        [NotMappedColumn]
        public GeoPoint GeoPoint { get; set; }

        [NotMappedColumn]
        public int NumberOfContracts { get; set; }
    }

    /// <summary>
    /// Class represents local place information of the publisher
    /// </summary>
    public class LocalPlace
    {
        public string Url { get; set; }

        [Display(Name = "Ulice")]
        [GridColumn(Title = "Ulice")]
        public string StreetAddress { get; set; }

        [Display(Name = "PSČ")]
        [GridColumn(Title = "PSČ")]
        public string PostalCode { get; set; }

        [NotMappedColumn]
        public SortedDictionary<DayOfWeekCz, ICollection<OpeningHour>> OpeningHours { get; set; }
    }

    /// <summary>
    /// Class represents opening hour
    /// </summary>
    public class OpeningHour
    {
        public TimeSpan Open { get; set; }

        public TimeSpan Close { get; set; }
    }

    /// <summary>
    /// Class represents geological point
    /// </summary>
    public class GeoPoint
    {
        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }
    }

    /// <summary>
    /// Enum of day of the week
    /// </summary>
    public enum DayOfWeekCz
    {     
        [Display(Name = "Pondělí")]
        Monday = 0,
        [Display(Name = "Úterý")]
        Tuesday = 1,
        [Display(Name = "Středa")]
        Wednesday = 2,
        [Display(Name = "Čtvrtek")]
        Thursday = 3,
        [Display(Name = "Pátek")]
        Friday = 4,
        [Display( Name = "Sobota")]
        Saturday = 5,
        [Display(Name = "Neděle")]
        Sunday = 6
    }

}