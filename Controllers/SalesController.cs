using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PharmacyManagement.Controllers
{
    public class SalesController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        // GET: Sales
        public ActionResult AddSales()
        {
            return View();
        }

        public bool SaveSales(SalesModel model)
        {
            using (var transactionscope = new TransactionScope())
            {
                try
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        int CustomerId = 0;
                        int CustomerPurchaseId = 0;
                        string customerQuery = "Insert into Customer (CustomerName,MobileNumber,Addrerss,Email,CreatedDate,CreatedBy,DeletedFlag)" +
                            "values('"+model.Customer.CustomerName+"','"+model.Customer.MobileNumber+"','"+model.Customer.Address+"','"+model.Customer.Email+"','"+DateTime.Now+"',"+ (int)Session["UserId"]+",'N');" +
                            "SELECT SCOPE_IDENTITY();"; //getting the Id of inserted row

                        using (SqlCommand cmd = new SqlCommand(customerQuery, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            CustomerId = Convert.ToInt32(cmd.ExecuteScalar());

                        }
                        string customerPurchaseQuery = "Insert into CustomerPurchase (CustomerId,Discount,PurchasedDate,DeletedFlag)" +
                            "values('"+CustomerId+"','"+model.CustomerPurchase.Discount+"','"+model.CustomerPurchase.PurchasedDate+"','N');" +
                            "SELECT SCOPE_IDENTITY();";
                        using (SqlCommand cmd = new SqlCommand(customerPurchaseQuery, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            CustomerPurchaseId = Convert.ToInt32(cmd.ExecuteScalar());

                        }
                        string customerPurchasedMedicineQuery = String.Empty;
                        foreach (var data in model.CustomerPurchasedMedicine)
                        {
                            customerPurchasedMedicineQuery += "Insert into CustomerMedicinePurchased(MedicineId,CustomerPurchasedId,Quantity,Rate,Amount,DeletedFlag)" +
                                "values('"+data.MedicineId+"','"+CustomerPurchaseId+"','"+data.Quantity+"','"+data.Rate+"','"+data.Amount+"');";
                        }
                        using (SqlCommand cmd = new SqlCommand(customerPurchasedMedicineQuery, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();

                        }
                        //commit transaction
                        transactionscope.Complete();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    //rollback trasnaction
                    transactionscope.Complete();
                    return false;
                }
            }

        }
    }
}