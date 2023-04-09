using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Reflection;

namespace PharmacyManagement.Controllers
{
    public class PurchaseController : BaseController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        public ActionResult AddPurchase()
        {
            return View();
        }
        public ActionResult GetPurchase()
        {
            return View();
        }

        [HttpPost]
        public bool SavePurchase(PurchaseModel model)
        {
            try
            {
                int purchaseId=0;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryCount = "SELECT MAX(PurchaseId) + 1 AS next_value FROM Purchase";
                    using (SqlCommand cmd = new SqlCommand(queryCount, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        purchaseId=(int)cmd.ExecuteScalar();
                        if (purchaseId == 0)
                        {
                            return false;
                        }
                    }                    
                    string purchaseQuery = "Insert into Purchase(SupplierId,BatchNumber,PaymentType,GrandTotal,PurchasedDate,DeletedFlag,CreatedBy)" +
                                        "values('"+model.SupplierId+"','"+model.BatchNumber+"','"+model.PaymentType+"','"+model.GrandTotal+"','"+model.PurchasedDate+ "','N''" + (int)Session["UserId"] + "');";
                    using (SqlCommand cmd = new SqlCommand(purchaseQuery, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        
                    }
                    string medicinePurchasedQuery = String.Empty;
                    foreach (var data in model.MedicinePurchasedModels)
                    {
                        medicinePurchasedQuery += "Insert into MedicinePurchased (MedicineId,PurchaseId,Price,ExpiryDate,CreatedDate,CreatedBy,DeletedFlag,Quantity,TotalAmount)" +
                                                  "values('" + data.MedicineId + "','" + purchaseId + "','" + data.Price + "','" + data.ExpiryDate + "','" + DateTime.Now + "','N','" + data.Quantity + "','" + data.TotalAmount + "');";
                    }
                    using (SqlCommand cmd = new SqlCommand(medicinePurchasedQuery, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult GetAllPurchase()
        {
            List<PurchaseModel> purchaseList = new List<PurchaseModel>(); //creating list for customer data

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
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
       
        //public ActionResult PurchaseInvoiceTemplate(int id)
        [HttpPost]
        public ActionResult PurchaseInvoiceTemplate(PurchaseModel model)
        {
            //PurchaseModel model = new PurchaseModel();
            var MedicinePurchasedModels = GetMedicinePurchased(model.PurchaseId);
            //var MedicinePurchasedModels = GetMedicinePurchased(id);
            if (MedicinePurchasedModels != null)
            {
                model.MedicinePurchasedModels = MedicinePurchasedModels;
                return PartialView(model);
                //return View("PurchaseInvoiceTemplate", model: model, layout: null);
            }
            else
            {
                
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");
            }
        }


        public List<MedicinePurchasedModel> GetMedicinePurchased(int PurchaseId)
        {
            List<MedicinePurchasedModel> purchaseList = new List<MedicinePurchasedModel>(); //creating list for customer data

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select mp.*, m.MedicineName from MedicinePurchased mp join Medicines m on mp.MedicineId=m.MedicineId where mp.PurchaseId="+ PurchaseId+" and mp.DeletedFlag ='N'"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MedicinePurchasedModel purchase = new MedicinePurchasedModel(); //creating customer object
                                purchase.MedicinePurchasedId = int.Parse(reader[0].ToString());
                                purchase.MedicineId = int.Parse(reader[1].ToString());
                                purchase.PurchasId = int.Parse(reader[2].ToString());
                                purchase.Price = decimal.Parse(reader[3].ToString());
                                purchase.ExpiryDate = DateTime.Parse(reader[4].ToString()).ToString("yyyy-MM-dd");
                                purchase.CreatedDate = DateTime.Parse(reader[5].ToString()).ToString("yyyy-MM-dd");
                                purchase.DeletedFlag = char.Parse(reader[6].ToString());
                                purchase.Quantity = int.Parse(reader[7].ToString());
                                purchase.TotalAmount = decimal.Parse(reader[8].ToString());
                                purchase.MedicineName = reader[9].ToString();
                                purchaseList.Add(purchase);
                            }
                            return purchaseList;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return purchaseList;
            }
        }

        [HttpPost]
        public bool DeletePurchase(int PurchaseId)
        {
            try
            {                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryCount = "update Purchase set DeletedFlag='Y' where PurchaseId="+ PurchaseId;
                    using (SqlCommand cmd = new SqlCommand(queryCount, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        return true;
                    }                   
                    
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}