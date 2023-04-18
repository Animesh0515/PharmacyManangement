using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class MedicineModel
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CreatedDate { get; set; }
        public string ExpiryDate { get; set; }
        public int  CreatedBy { get; set; }
        public char DeletedFlag { get; set; }
        public string PackingType { get; set; }
    }
}