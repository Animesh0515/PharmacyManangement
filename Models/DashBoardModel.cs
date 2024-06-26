﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyManagement.Models
{
    public class DashBoardModel
    {
        public DashBoardModel() 
        {
            ExpiredMedicines = new List<ExpiredMedicine>();
            RecentMedicines = new List<MedicineModel>();
        }
        public int CustomerCount { get; set; }
        public float CustomerPercent { get; set; }
        public int SupplierCount { get; set; }
        public float SupplierPercent { get; set; }
        public int MedicineCount { get; set; }
        public float MedicinePercent { get; set; }
        public int MedicineOutOfStock { get; set; }
        public float OutOfStockPercent { get; set; }

        public int SalesCount { get; set; }
        public float SalesPercent { get; set; }
        public int  PurchaseCount { get; set; }
        public float PurchasePercent { get; set; }


        public List<ExpiredMedicine> ExpiredMedicines { get; set; }
        public List<MedicineModel> RecentMedicines { get; set; }


    }

    public class ExpiredMedicine
    {
        public string MedicineName { get; set; }
        public string PackingType { get; set; }
        public string CreatedDate { get; set; }
        public string ExpiryDate { get; set; }
    }
}
