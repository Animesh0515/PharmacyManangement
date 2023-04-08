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
                    string purchaseQuery = "Insert into Purchase(SupplierId,BatchNumber,PaymentType,GrandTotal,PurchasedDate)" +
                                        "values('"+model.SupplierId+"','"+model.BatchNumber+"','"+model.PaymentType+"','"+model.GrandTotal+"','"+model.PurchasedDate+"');";
                    using (SqlCommand cmd = new SqlCommand(purchaseQuery, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        
                    }
                    string medicinePurchasedQuery = String.Empty;
                    foreach (var data in model.MedicinePurchasedModels)
                    {
                        medicinePurchasedQuery += "Insert into MedicinePurchased (MedicineId,PurchaseId,Price,ExpiryDate,CreatedDate,CreatedBy,DeletedFlag,Quantity,TotalAmount)" +
                                                  "values('" + data.MedicineId + "','" + purchaseId + "','" + data.Price + "','" + data.ExpiryDate + "','" + DateTime.Now + "','" + (int)Session["UserId"] + "','N','" + data.Quantity + "','" + data.TotalAmount + "');";
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
       
    }
}