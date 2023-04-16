using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace PharmacyManagement.Controllers
{
    public class ReportController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        // GET: Report
        public ActionResult SalesReport()
        {
            return View();
        }

        public ActionResult PurchaseReport()
        {
            return View();
        }

        public ActionResult GetSalesReport()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select c.CustomerName, cp.GrandTotal, cp.PurchasedDate"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PurchaseModel purchase = new PurchaseModel(); //creating customer object
                                purchase.PurchaseId = int.Parse(reader[0].ToString());
                                purchase.SupplierId = int.Parse(reader[1].ToString());
                                purchase.SupplierName = reader[2].ToString();
                                purchase.BatchNumber = reader[3].ToString();
                                purchase.PaymentType = reader[4].ToString();
                                purchase.GrandTotal = decimal.Parse(reader[5].ToString());
                                purchase.PurchasedDate = DateTime.Parse(reader[6].ToString()).ToString("yyyy-MM-dd");
                                purchase.DeletedFlag = char.Parse(reader[7].ToString());
                                //purchaseList.Add(purchase);
                            }
                            //return Json(purchaseList, JsonRequestBehavior.AllowGet);
                            return null;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");

            }

        }
    }
}