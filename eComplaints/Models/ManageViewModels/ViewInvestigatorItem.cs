using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ManageViewModels
{
    public class ViewInvestigatorItem
    {
        [Display(Name ="Id Investigator")]
        public string Id { get; set; }

        [Display(Name ="UserName")]
        public string Username { get; set; }

        [Display(Name ="Departament")]
        public string Department { get; set; }

    }
}
