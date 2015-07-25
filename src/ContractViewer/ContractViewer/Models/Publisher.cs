using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
    public class Publisher
    {
        [Display(Name = "IČ")]
        [GridColumn(Title = "IČ")]
        public string Ic { get; set; }

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

    }

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
        public ICollection<OpeningHoursOfTheDay> OpeningHoursOfTheDay { get; set; }
    }

    public class OpeningHoursOfTheDay
    {
        public DayOfWeekCz DayOfWeek { get; set; }

        public ICollection<OpeningHour> OpeningHours { get; set; }
    }

    public class OpeningHour
    {
        public TimeSpan Open { get; set; }

        public TimeSpan Close { get; set; }
    }

    public enum DayOfWeekCz
    {     
        [Display(Name = "Pondělí")]
        Monday,
        [Display(Name = "Úterý")]
        Tuesday,
        [Display(Name = "Středa")]
        Wednesday,
        [Display(Name = "Čtvrtek")]
        Thursday,
        [Display(Name = "Pátek")]
        Friday,
        [Display( Name = "Sobota")]
        Saturday,
        [Display(Name = "Neděle")]
        Sunday
    }

}