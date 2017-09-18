using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ManageViewModels
{
    public class LineCoordinatorItem
    {
        public int Id { get; set; }

        [Display(Name ="Nume")]
        public string LineCoordinatorName { get; set; }

        [Display(Name = "Linie")]
        public string Area { get; set; }
    }
}
