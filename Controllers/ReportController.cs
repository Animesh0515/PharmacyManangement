using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;

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
                List<SalesModel> salesList = new List<SalesModel>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SalesController salesController = new SalesController();

                    string query = "Select c.CustomerId,c.CustomerName,c.MobileNumber,c.Address,c.Email, cp.CustomerPurchaseId, cp.Discount,cp.GrandTotal,cp.PurchasedDate from Customer c join CustomerPurchase cp on c.CustomerId=cp.CustomerId where cp.DeletedFlag='N'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SalesModel model = new SalesModel();
                                CustomerModel customer = new CustomerModel();
                                CustomerPurchase customerpurchase = new CustomerPurchase();
                                customer.CustomerId = int.Parse(reader[0].ToString());
                                customer.CustomerName = reader[1].ToString();
                                customer.MobileNumber = long.Parse(reader[2].ToString());
                                customer.Address = reader[3].ToString();
                                customer.Email = reader[4].ToString();

                                customerpurchase.CustomerPurchaseId = int.Parse(reader[5].ToString());
                                customerpurchase.Discount = decimal.Parse(reader[6].ToString());
                                customerpurchase.GrandTotal = decimal.Parse(reader[7].ToString());
                                customerpurchase.PurchasedDate = DateTime.Parse(reader[8].ToString()).ToString("yyyy-MM-dd");
                                model.Customer = customer;
                                model.CustomerPurchase = customerpurchase;

                                //getting medicine detail from sales controller
                               var customerPurchasedMedicine= salesController.getMedicinePurchased(customerpurchase.CustomerPurchaseId);
                                model.CustomerPurchasedMedicine = customerPurchasedMedicine;
                                salesList.Add(model);

                            }
                        }
                    }

                    

                    return Json(salesList, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");
            }

        }

        public ActionResult GetPurchaseReport()
        {
            List<PurchaseModel> purchaseList = new List<PurchaseModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    PurchaseController purchaseController = new PurchaseController();

                    string query = "Select P.PurchaseId, s.SupplierId, s.SupplierName,P.BatchNumber,P.PaymentType,P.GrandTotal,P.PurchasedDate,P.DeletedFlag from Purchase P join Suppliers S on P.SupplierId=S.SupplierId where P.DeletedFlag='N'"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
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

                                //getting medicine details
                                purchase.MedicinePurchasedModels = purchaseController.GetMedicinePurchased(purchase.PurchaseId);
                                purchaseList.Add(purchase);
                            }
                            return Json(purchaseList, JsonRequestBehavior.AllowGet);

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