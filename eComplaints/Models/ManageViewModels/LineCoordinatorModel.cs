using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ManageViewModels
{
    public class LineCoordinatorModel
    { 

        [Display(Name = "Coordonator de Linie")]
        [Required(ErrorMessage ="Introduceti numele coordonatorului")]
        public string CoordinatorName { get; set; }

        [Display(Name ="Linie")]
        [Required (ErrorMessage ="Introduceti o Linie")]
        public string Area { get; set; }
    }
}
