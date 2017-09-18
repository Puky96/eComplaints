using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eComplaints.Models.ReportViewModels
{
    public class MInvestigatorReport
    {
        [Display(Name = "Categorie")]
        public string PhenomenaCategory { get; set; }

        [Display (Name ="Problema reala")]
        public string QuestionName { get; set; }

        [Required]
        [Display(Name = "Detalii investigatie")]
        public string InvestigationDetails { get; set; }
        
        [Display(Name ="Alta problema")]
        public string AdditionalProblem { get; set; }

        [Display(Name = "Alta problema(engleza)")]
        public string AdditionalProblemEnglish { get; set; }

        public int ReportId { get; set; }

        [Display(Name = "Vendor  BATCH")]
        public string VendorBatch { get; set; }

        [Display(Name = "Batch SAP")]
        public string BatchSAP { get; set; }

        [Display(Name = "Batch No")]
        public string BatchNo { get; set; }

        //Tracking Details 
        [Required]
        public string Status { get; set; }
        

        public DateTime? DueDate { get; set; }

        [Required]
        [Display(Name ="Problema repetata")]
        public bool RepeatedIssue { get; set; }

        [Required]
        [Display(Name ="Problema cronica")]
        public bool Chronic { get; set; }
        
        [Required]
        public string Supplier { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
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
