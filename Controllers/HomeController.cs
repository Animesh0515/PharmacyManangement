using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PharmacyManagement.Controllers
{
    public class HomeController : BaseController
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

        public ActionResult GetDashboardData()
        {
            DashBoardModel model= new DashBoardModel();
            DateTime current = DateTime.Today;
            string today = current.ToString("yyyy-MM-dd");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string queries = "Select Count(1) from Customer where DeletedFlag='N';" +
                        $"Select Count(1) from Customer where DeletedFlag='N' and CreatedDate = '{today}';" +
                        "Select Count(1) from Suppliers where DeletedFlag='N';" +
                        $"Select Count(1) from Suppliers where DeletedFlag='N' and CreatedDate = '{today}';" +
                        "Select Count(1) from Medicines where DeletedFlag='N'; " +
                        $"Select Count(1) from Medicines where DeletedFlag='N' and CreatedDate = '{today}';" +
                        "Select Count(1) from Medicines where DeletedFlag='N' and Quantity<1; " +
                        $"Select Count(1) from Medicines where DeletedFlag='N'and CreatedDate = '{today}' and Quantity<1;" +
                        "Select Count(1) from CustomerPurchase where DeletedFlag='N'; " +
                        $"Select Count(1) from CustomerPurchase where DeletedFlag='N'and PurchasedDate = '{today}';" +
                        "Select Count(1) from Purchase where DeletedFlag='N';" +
                        $"Select Count(1) from Purchase where DeletedFlag='N'and PurchasedDate = '{today}';" +
                        "Select MedicineName, PackingType, mp.CreatedDate, ExpiryDate from Medicines m join MedicinePurchased mp on m.MedicineId=mp.MedicineId where ExpiryDate < CONVERT(DATE, GETDATE()) and m.DeletedFlag='N' and mp.DeletedFlag='N';" +
                        "select MedicineName, mp.CreatedDate from Medicines m join MedicinePurchased mp on m.MedicineId=mp.MedicineId where mp.CreatedDate > DATEADD(DAY, -7, GETDATE());";

                using (SqlCommand command= new SqlCommand(queries,conn))       
                { 
                    command.CommandType=CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.CustomerCount = reader.GetInt32(0);
                        }

                        // Move to the next result set
                        reader.NextResult();

                        if (reader.Read())
                        {
                            float totalData = model.CustomerCount;
                            int todayData = reader.GetInt32(0);
                            float percent = (100 * todayData)/totalData;
                            model.CustomerPercent = (totalData == 0) ? 100 : (int)Math.Round(percent,0);

                        }

                        // Move to the next result set
                        reader.NextResult();

                        // Process the second query result
                        if (reader.Read())
                        {
                            model.SupplierCount = reader.GetInt32(0);
                            
                        }
                       
                        reader.NextResult();

                        // Process the second query result
                        if (reader.Read())
                        {
                            float totalData = model.SupplierCount;
                            int todayData = reader.GetInt32(0);
                            float percent = (100 * todayData) / totalData;
                            model.SupplierPercent = (totalData == 0) ? 100 : (int)Math.Round(percent, 0);

                        }

                        reader.NextResult();

                        // Process the third query result
                        if (reader.Read())
                        {
                            model.MedicineCount = reader.GetInt32(0);
                            
                        }
                        
                        reader.NextResult();

                        // Process the third query result
                        if (reader.Read())
                        {
                            float totalData = model.MedicineCount;
                            int todayData = reader.GetInt32(0);
                            float percent = (100 * todayData) / totalData;
                            model.MedicinePercent = (totalData == 0) ? 100 : (int)Math.Round(percent, 0);

                        }

                        reader.NextResult();


                        if (reader.Read())
                        {
                            model.MedicineOutOfStock = reader.GetInt32(0);
                            
                        }
                        
                        reader.NextResult();

                        if (reader.Read())
                        {
                            float totalData = model.MedicineOutOfStock;
                            int todayData = reader.GetInt32(0);
                            float percent = (100 * todayData) / totalData;
                            model.OutOfStockPercent = (totalData == 0) ? 100 : (int)Math.Round(percent, 0);
                        }

                        reader.NextResult();

                        if (reader.Read())
                        {
                            model.SalesCount= reader.GetInt32(0);
                            
                        }
                        
                        reader.NextResult();

                        if (reader.Read())
                        {
                            float totalData = model.SalesCount;
                            int todayData = reader.GetInt32(0);
                            float percent = (100 * todayData) / totalData;
                            model.SalesPercent = (totalData == 0) ? 100 : (int)Math.Round(percent, 0);
                        }

                        reader.NextResult();


                        if (reader.Read())
                        {
                            model.PurchaseCount= reader.GetInt32(0);
                            
                        }
                        
                        reader.NextResult();
                        
                        if (reader.Read())
                        {
                            float totalData = model.PurchaseCount;
                            int todayData = reader.GetInt32(0);
                            float percent = (100 * todayData) / totalData;
                            model.PurchasePercent = (totalData == 0) ? 100 : (int)Math.Round(percent, 0);

                        }

                        reader.NextResult();


                        while (reader.Read())
                        {
                            ExpiredMedicine expiredMed= new ExpiredMedicine();
                            expiredMed.MedicineName= reader.GetString(0);
                            expiredMed.PackingType= reader.GetString(1);
                            expiredMed.CreatedDate= reader.GetDateTime(2).ToString("dd-MM-yyyy");
                            expiredMed.ExpiryDate= reader.GetDateTime(3).ToString("dd-MM-yyyy");
                            
                            model.ExpiredMedicines.Add(expiredMed);
                        }
                        
                        reader.NextResult();

                       
                        while (reader.Read())
                        {
                            MedicineModel medicine = new MedicineModel();
                            medicine.MedicineName= reader.GetString(0);
                            medicine.CreatedDate= reader.GetDateTime(1).ToString();
                            
                            model.RecentMedicines.Add(medicine);
                        }

                    }

                    return Json(model, JsonRequestBehavior.AllowGet);
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