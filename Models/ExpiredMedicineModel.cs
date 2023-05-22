using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class ExpiredMedicineModel
    {
        public ExpiredMedicineModel()
        {
            ExpiredMedicineDetail = new List<ExpiredMedicineDetail>();
        }

        public MedicineModel Medicine { get; set; }
        public List<ExpiredMedicineDetail> ExpiredMedicineDetail { get; set; }
    }

    public class ExpiredMedicineDetail
    {
        public int BatchNumber { get; set; }
        public string SupplierName { get; set; }
        public string ExpiryDate { get; set;}
    }
}