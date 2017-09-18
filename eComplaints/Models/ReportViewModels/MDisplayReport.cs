using eComplaints.DBModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models.ReportViewModels
{
    public class MDisplayReport
    {
        [Display(Name = "ID Plangere")]
        public int Id { get; set; }

        [Display(Name = "Poza problema")]
        public string ImagePath { get; set; }

        public string Originator { get; set; }

        [Display(Name = "Poza problema 2")]
        public string ImagePath1 { get; set; }

        [Display(Name = "Poze problema 3")]
        public string ImagePath2 { get; set; }

        [Display(Name = "Poza eticheta palet")]
        public string EtiqueteImage { get; set; }

        [Display(Name = "Echipament")]
        public string Equipment { get; set; }

        [Display(Name = "Coordonator Linie")]
        public string LineCoordinator { get; set; }

        [Display(Name = "Linie")]
        public string Line { get; set; }

        [Display(Name = "Data/Ora")]
        public DateTime DateHour { get; set; }

        public string GCAS { get; set; }

        public string BatchSAP { get; set; }

        public string PO { get; set; }

        public string VendorBatch { get; set; }

        public TimeSpan DownTime { get; set; }

        [Display(Name = "Numar opriri")]
        public int NumberOfStops { get; set; }

        [Display(Name = "Mostra: ")]
        public bool HasSample { get; set; }

        [Display(Name = "Lot blocat: ")]
        public bool BlockedBatch { get; set; }

        [Display(Name = "Cantitate")]
        public int Quantity { get; set; }

        [Display(Name = "Tip material")]
        public string PhenomenaCategory { get; set; }

        [Display(Name = "Problema")]
        public string Problem { get; set; }

        [Display(Name = "Aproba plangerea")]
        public bool Approved { get; set; }

        [Display (Name = "Motiv")]
        public string Reason { get; set; }

    }
}
