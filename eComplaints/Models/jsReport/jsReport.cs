using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.jsReport
{
    public class jsReport
    {
        public int Id { get; set; }

        public string IdentificationNumber { get; set; }

        public string Date { get; set; }

        public string IsRelevant { get; set; }

        public string Originator { get; set; }

        public string Equipment { get; set; }

        public string Area { get; set; }
        
        public string LineCoordinator { get; set; }

        public string  DateHour { get; set; }

        public string Gcas { get; set; }

        public string BatchSap { get; set; }

        public string VendorBatch { get; set; }

        public string Po { get; set; }

        public string PhenomenaDescription { get; set; }

        public TimeSpan Downtime { get; set; }

        public int NumberOfStops { get; set; }

        public string HasSample { get; set; }

        public string BlockedBatch { get; set; }

        public string BatchNo { get; set; }

        public int Quantity { get; set; }

        public string ImagePath { get; set; }

        public string ImagePath1 { get; set; }

        public string ImagePath2 { get; set; }

        public string EtiqueteImagePath { get; set; }

        public string PhenomenaCategory { get; set; }

        public string Problem { get; set; }

    }
}
