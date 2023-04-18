using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyManagement.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public long MobileNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public char Role { get; set; }
    }
}