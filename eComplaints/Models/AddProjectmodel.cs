using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eProjects.Models.ProjectViewModels
{
    public class AddProjectModel
    {
        [Display(Name = "Project Name*")]
        public string ProjectName { get; set; }

        [Display(Name = "Project Leader*")]
        public string ProjectLeader { get; set; }

        [Display(Name = "Startup Leader*")]
        public string StartupLeader { get; set; }

        [Display(Name = "CM#")]
        public int Cm { get; set; }

        [Display(Name = "PC&IS Resource*")]
        public string Pcisresource { get; set; }

        [Display(Name = "FiscalYear")]
        public string FiscalYear { get; set; }

        [Display(Name = "Plant AE")]
        public bool PlantAe { get; set; }

        [Display(Name = "Team Charter")]
        public bool TeamCharter { get; set; }

        [Display(Name = "Priority*")]
        public string Priority { get; set; }

        [Display(Name = "Class*")]
        public string Class { get; set; }

        [Display(Name = "Leading Department*")]
        public string LeadingDepartment { get; set; }

        [Display(Name = "Impacted Department*")]
        public string ImpactedDepartment { get; set; }

        [Display(Name = "ORA")]
        public decimal Ora { get; set; }

        [Display(Name = "Front End Eng Start Date")]
        public DateTime FrontEndEngStart { get; set; }

        [Display(Name = "Front End Eng End Date")]
        public DateTime FrontEndEngEnd { get; set; }

        [Display(Name = "Funding Start Date")]
        public DateTime FundingDateStart { get; set; }

        [Display(Name = "Funding End Date")]
        public DateTime FundingDateEnd { get; set; }

        [Display(Name = "Design Procure Start Date")]
        public DateTime DesignProcureStart { get; set; }

        [Display(Name = "Design Procure End Date")]
        public DateTime DesignProcureEnd { get; set; }

        [Display(Name = "Construction Start Date")]
        public DateTime ConstructionStart { get; set; }

        [Display(Name = "Construction End Date")]
        public DateTime ConstructionEnd { get; set; }

        [Display(Name = "Startup Start Date")]
        public DateTime Startupstart { get; set; }

        [Display(Name = "Startup End Date")]
        public DateTime StartupEnd { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

    }
}
