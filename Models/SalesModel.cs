using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class SalesModel
    {
        public CustomerModel Customer { get; set; }
        public CustomerPurchase CustomerPurchase { get; set; }
        public List<CustomerPurchasedMedicine> CustomerPurchasedMedicine { get; set; }
    }

    public class CustomerPurchase
    {
        public int CustomerPurchaseId { get; set; }
        public int CustomerId { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime PurchasedDate { get; set; }
        public char DeletedFlag { get; set; }


    }
    public class CustomerPurchasedMedicine
    {
        public int CustomerPurchasedMedicineId { get; set; }
        public int MedicineId { get; set; }
        public int CustomerPurchaseId { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public char DeletedFlag { get; set; }
    }
}