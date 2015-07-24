using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractViewer.Models
{
    public class ContractViewModel
    {
        public Contract Contract { get; set; }

        public IEnumerable<Party> Parties { get; set; }
        public IEnumerable<Amendment> Amendments { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public IEnumerable<Milestone> Milestones { get; set; }
    }
}