using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ReportViewModels
{
    public class TrackingEditViewModel
    {
        public int ReportId { get; set; }

        //View Fields
        [Display(Name = "Id")]
        public string IdentificationNumber { get; set; }

        [Display(Name ="Data rezolvare")]
        public string DueDate { get; set; }

        [Display(Name = "Material")]
        public string MaterialType { get; set; }

        [Display(Name ="Problema")]
        public string Issue { get; set; }

        public string Supplier { get; set; }

        //Edit fields
        [Display(Name ="Comentarii")]
        public string Comments { get; set; }

        [Display(Name ="Actiuni inchidere")]
        public string ActionsForClosing { get; set; }

        [Display(Name ="Obiecte")]
        public string Objects { get; set; }

        public string Status { get; set; }

        
    }
}
