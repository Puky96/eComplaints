using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ReportViewModels
{
    public class ReportCreatorModel
    {
        [Display(Name = "Data inceput")]
        [Required(ErrorMessage ="Introduceti data de inceput")]
        public DateTime? BeginDate { get; set; }

        [Display(Name = "Data sfarsit")]
        [Required(ErrorMessage = "Introduceti data de sfarsit")]
        public DateTime? EndDate { get; set; }

        public string Suppplier { get; set; }

        [Display(Name = "Aprobata")]
        public int? Approved { get; set; }

        public string Status { get; set; }

        [Display(Name = "Problema")]
        public string Issue { get; set; }

        [Display(Name = "Repetata")]
        public int? Repeated { get; set; }

        public string Brand { get; set; }

        public string Size { get; set; }

        [Display(Name = "Linie")]
        public int? Line { get; set; }

        [Display(Name = "Material Blocat")]
        public string MaterialBlocked { get; set; }

        public string GCAS { get; set; }

        [Display(Name = "Departament")]
        public int? Department { get; set; }

        public List<ReportListItem> Reports { get; set; }
    }
}
