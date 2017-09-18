using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ManageViewModels
{
    public class AddOriginatorModel
    {
        [Required(ErrorMessage ="Introduceti un nume de utilizator!")]
        [Display(Name = "UserName")]
        public string OriginatorName { get; set; }

        [Required(ErrorMessage ="Introduceti numele complet al originatorului")]
        [Display(Name ="Nume complet")]
        public string FullName { get; set; }
    }
}
