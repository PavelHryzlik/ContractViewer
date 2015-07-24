﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GridMvc.DataAnnotations;

namespace ContractViewer.Models
{
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