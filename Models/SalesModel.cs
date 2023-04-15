using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class SalesModel
    {
        public SalesModel()
        {
            CustomerPurchasedMedicine= new  List<CustomerPurchasedMedicine>();
        }
        
        public CustomerModel Customer { get; set; }
        public CustomerPurchase CustomerPurchase { get; set; }
        public List<CustomerPurchasedMedicine> CustomerPurchasedMedicine { get; set; }
        public bool IsNewCustomer { get; set; }
    }

    public class CustomerPurchase
    {
        public int CustomerPurchaseId { get; set; }
        public int CustomerId { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public string PurchasedDate { get; set; }
        public char DeletedFlag { get; set; }


    }
    public class CustomerPurchasedMedicine
    {
        public int CustomerPurchasedMedicineId { get; set; }
        public int MedicineId { get; set; }
        public string PackingType { get; set; }
        public int CustomerPurchaseId { get; set; }
        public int Quantity { get; set; }    
        public decimal Price { get; set; }    //Rate
        public decimal TotalAmount { get; set; }
        public char DeletedFlag { get; set; }
    }
}