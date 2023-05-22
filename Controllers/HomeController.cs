using PharmacyManagement.Models;
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

        public DashBoardModel GetDashboardData()
        {
            DashBoardModel model= new DashBoardModel();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command= new SqlCommand())
                {
                    string queries = "Select Count(1) from Customer where DeletedFlag='N';" +
                        "Select Count(1) from Suppliers where DeletedFlag='N';" +
                        "Select Count(1) from Medicines where DeletedFlag='N'; " +
                        "Select Count(1) from Medicines where DeletedFlag='N' and Quantity<1; " +
                        "Select Count(1) from CustomerPurchase where DeletedFlag='N'; " +
                        "Select Count(1) from Purchase where DeletedFlag='N';" +
                        "Select MedicineName, PackingType, mp.CreatedDate, ExpiryDate from Medicines m join MedicinePurchased mp on m.MedicineId=mp.MedicineId where ExpiryDate < CONVERT(DATE, GETDATE()) and m.DeletedFlag='N' and mp.DeletedFlag='N';" +
                        "select MedicineName, mp.CreatedDate from Medicines m join MedicinePurchased mp on m.MedicineId=mp.MedicineId where mp.CreatedDate > DATEADD(DAY, -7, GETDATE());";

                    command.CommandText = queries;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.CustomerCount= reader.GetInt32(0);                            
                        }

                        // Move to the next result set
                        reader.NextResult();

                        // Process the second query result
                        if (reader.Read())
                        {
                            model.SupplierCount = reader.GetInt32(0);
                            
                        }
                       
                        reader.NextResult();

                        // Process the third query result
                        if (reader.Read())
                        {
                            model.MedicineCount = reader.GetInt32(0);
                            
                        }              

                        
                        reader.NextResult();

                        // Process the fourth query result
                        if (reader.Read())
                        {
                            model.MedicineOutOfStock= reader.GetInt32(0);
                            
                        }
                        
                        reader.NextResult();

                        // Process the fourth query result
                        if (reader.Read())
                        {
                            model.SalesCount= reader.GetInt32(0);
                            
                        }
                        
                        reader.NextResult();

                        // Process the fourth query result
                        if (reader.Read())
                        {
                            model.PurchaseCount= reader.GetInt32(0);
                            
                        }
                        
                        reader.NextResult();

                        // Process the fourth query result
                        while (reader.Read())
                        {
                            ExpiredMedicine expiredMed= new ExpiredMedicine();
                            expiredMed.MedicineName= reader.GetString(0);
                            expiredMed.PackingType= reader.GetString(1);
                            expiredMed.CreatedDate= reader.GetString(2);
                            expiredMed.ExpiryDate= reader.GetString(3);
                            
                            model.ExpiredMedicines.Add(expiredMed);
                        }
                        
                        reader.NextResult();

                        // Process the fourth query result
                        while (reader.Read())
                        {
                            MedicineModel medicine = new MedicineModel();
                            medicine.MedicineName= reader.GetString(0);
                            medicine.CreatedDate= reader.GetString(1);
                            
                            model.RecentMedicines.Add(medicine);
                        }

                    }

                    return model;
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