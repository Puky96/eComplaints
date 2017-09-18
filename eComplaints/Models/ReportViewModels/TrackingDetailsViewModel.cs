using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ReportViewModels
{
    public class TrackingDetailsViewModel
    {
        public int ReportId { get; set; }

        [Display(Name ="Id")]
        public string IdentificationNumber { get; set; }

        [Display(Name = "Data rezolvare(programata)")]
        public string DueDate { get; set; }

        [Display(Name = "Luna")]
        public string Month { get; set; }

        [Display(Name = "Problema")]
        public string Issue { get; set; }

        [Display(Name = "Repetata")]
        public string Repeated { get; set; }

        [Display(Name = "Cronica")]
        public string Chronic { get; set; }

        public string Owner { get; set; }

        public string Status { get; set; }

        [Display(Name = "Tip material")]
        public string MaterialType { get; set; }

        [Display(Name = "Supplier")]
        public string Supplier { get; set; }

        [Display(Name = "Confirmata")]
        public string ComplaintConfirmed { get; set; }

        public string BrandSize { get; set; }

        public string GCAS { get; set; }

        [Display(Name = "Data productie lot")]
        public string LotProductionDate { get; set; }

        [Display(Name = "Linie")]
        public string Line { get; set; }

        [Display(Name = "Material Blocat")]
        public string MaterialBlocked { get; set; }

        [Display(Name = "Cantitate Blocata")]
        public int QuantityBlocked { get; set; }

        [Display(Name = "Decizie blocare material")]
        public string BlockedMaterialDecision { get; set; }

        [Display(Name = "Claimed DT")]
        public TimeSpan ClaimedDT { get; set; }

        [Display(Name = "Claimed # of Stops")]
        public int ClaimedNrOfStops { get; set; }

        [Display(Name = "Confirmed DT")]
        public TimeSpan ConfirmedDT { get; set; }

        [Display(Name = "Confirmed # of Stops")]
        public int ConfirmedNrOfStops { get; set; }

        [Display(Name = "PO")]
        public string PO { get; set; }

        [Display(Name = "Comentarii")]
        public string Comments { get; set; }

        [Display(Name = "Actiuni inchidere")]
        public string ActionsForClosing { get; set; }

        [Display(Name = "Obiecte")]
        public string Objects { get; set; }


    }
}
