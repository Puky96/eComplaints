using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ManageViewModels
{
    public class AddInvestigatorModel
    {
        [Required(ErrorMessage ="Introduceti un nume de utilizator")]
        [Display(Name ="Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Introduceti numele complet al investigatorului!")]
        [Display(Name ="Nume complet")]
        public string FullName { get; set; }

        [Required(ErrorMessage ="Alegeti un departament")]
        [Display(Name ="Departament")]
        public string Department { get; set; }
    }
}
