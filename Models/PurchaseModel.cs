using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class PurchaseModel
    {

        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public string BatchNumber { get; set; }
        public string PaymentType { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime PurchasedDate { get; set; }

        public List<MedicinePurchasedModel> MedicinePurchasedModels { get; set; }
    }

    public class MedicinePurchasedModel
    {
        public int MedicinePurchasedId { get; set; }
        public int MedicineId { get; set; }
        public int PurchasId { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public char DeletedFlag { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }

    }
}