using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long MobileNumber { get; set; } //used long here because int was not accepting 10 digit value
        public string Email { get; set; }
        public string Address { get; set; }
    }
}