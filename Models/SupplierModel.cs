using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class SupplierModel
    {

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public char DeletedFlag { get; set; }
    }
}