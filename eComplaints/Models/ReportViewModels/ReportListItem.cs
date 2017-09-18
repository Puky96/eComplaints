using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ReportViewModels
{
    public class ReportListItem
    {
        public int ReportId { get; set; }

        public string IdentitficationNumber { get; set; }

        public string PhenomenaCategory { get; set; }

        public string Problem { get; set; }

        public DateTime IssuedDate { get; set; }

        public string Supplier { get; set; }

        public string IsRelevant { get; set; }
    }
}
