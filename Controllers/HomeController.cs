using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PharmacyManagement.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Dashboard()
        {
            return View();
        }

        public void GetDashboardData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command= new SqlCommand())
                {
                    string queries = "Select Count(1) from Customer where DeletedFlag='N';Select Count(1) from Suppliers where DeletedFlag='N';Select Count(1) from Medicines where DeletedFlag='N'; Select MedicineName";
                }
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}