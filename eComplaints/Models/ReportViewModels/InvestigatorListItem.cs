using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ReportViewModels
{
    public class InvestigatorListItem
    {
        [Display(Name ="# Plangere")]
        public int ReportId { get; set; }

        [Display(Name ="Nr plangere")]
        public string IdentificationNumber { get; set; }

        [Display(Name = "Linie")]
        public string Line { get; set; }

        [Display(Name = "Tip defect")]
        public string PhenomenaCategory { get; set; }

        [Display(Name = "Defect")]
        public string Problem { get; set; }

        [Display(Name ="Data")]
        public DateTime Date { get; set; }

        [Display(Name ="Relevanta")]
        public string Approved { get; set; }

        [Display(Name ="Tracking Status")]
        public string TrackingStatus { get; set; }

    }
}
