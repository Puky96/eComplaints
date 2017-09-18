using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ReportViewModels
{
    public class ReportDismissalModel
    {
        [Display(Name ="Id")]
        public int ReportId { get; set; }

        [Required]
        [Display(Name ="Motiv")]
        public string Reason { get; set; }

        //Tracking
        [Required]
        public string Status { get; set; }


        public DateTime? DueDate { get; set; }

        [Required]
        [Display(Name = "Problema repetata")]
        public bool RepeatedIssue { get; set; }

        [Required]
        [Display(Name = "Problema cronica")]
        public bool Chronic { get; set; }

        [Required]
        public string Supplier { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
        [Display(Name ="Data productie lot")]
        public DateTime? LotProductionDate { get; set; }

        //confirmed DT
        [Required]
        [Display(Name = "Ore")]
        public int Hours { get; set; }

        [Required]
        [Display(Name = "Minute")]
        public int Minutes { get; set; }

        [Required]
        [Display(Name = "Secunde")]
        public int Seconds { get; set; }

        [Required]
        [Display(Name = "Stopuri confirmate")]
        public int ConfirmedStops { get; set; }

        [Display(Name = "Decizie blocare material")]
        public string BlockedMaterialDecision { get; set; }

        [Required]
        [Display(Name = "Problema reclamata")]
        public string Issue { get; set; }

        [Display(Name = "Comentarii")]
        public string Comments { get; set; }
    }
}
