using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ManageViewModels
{
    public class EditLineCoordinatorViewModel
    {
        public int AreaId { get; set; }

        [Display(Name = "Linie")]
        public string AreaName { get; set; }

        public int Id { get; set; }

        [Display(Name ="Nume")]
        [Required]
        public string LineCoordinatorName { get; set; }
    }
}
