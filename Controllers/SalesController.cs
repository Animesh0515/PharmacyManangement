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
                        if (model.IsNewCustomer)
                        {

                            string customerQuery = "Insert into Customer (CustomerName,MobileNumber,Address,Email,CreatedDate,CreatedBy,DeletedFlag)" +
                                "values('" + model.Customer.CustomerName + "','" + model.Customer.MobileNumber + "','" + model.Customer.Address + "','" + model.Customer.Email + "','" + DateTime.Now + "'," + (int)Session["UserId"] + ",'N');" +
                                "SELECT SCOPE_IDENTITY();"; //getting the Id of inserted row

                            using (SqlCommand cmd = new SqlCommand(customerQuery, conn))
                            {
                                cmd.CommandType = CommandType.Text;
                                CustomerId = Convert.ToInt32(cmd.ExecuteScalar());

                            }
                        }
                        else
                        {
                            CustomerId = model.Customer.CustomerId;
                        }
                        string customerPurchaseQuery = "Insert into CustomerPurchase (CustomerId,Discount,GrandTotal,PurchasedDate,DeletedFlag)" +
                            "values('"+CustomerId+"','"+model.CustomerPurchase.Discount+"','"+model.CustomerPurchase.GrandTotal+"','"+model.CustomerPurchase.PurchasedDate+"','N');" +
                            "SELECT SCOPE_IDENTITY();";
                        using (SqlCommand cmd = new SqlCommand(customerPurchaseQuery, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            CustomerPurchaseId = Convert.ToInt32(cmd.ExecuteScalar());

                        }
                        string customerPurchasedMedicineQuery = String.Empty;
                        foreach (var data in model.CustomerPurchasedMedicine)
                        {
                            customerPurchasedMedicineQuery += "Insert into CustomerPurchasedMedicine(MedicineId,CustomerPurchasedId,Quantity,Rate,Amount,DeletedFlag)" +
                                "values('"+data.MedicineId+"','"+CustomerPurchaseId+"','"+data.Quantity+"','"+data.Price+"','"+data.TotalAmount +"','N');";
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
                    transactionscope.Dispose();
                    return false;
                }
            }

        }


        public ActionResult GetSales()
        {            
            return View();
        }
        
        public ActionResult EditSales(int id)
        {            
            return View();
        }
        public ActionResult GetSalesData()
        {
            try
            {
                List<SalesModel> modelList = new List<SalesModel>();
                using (SqlConnection conn= new SqlConnection(connectionString))
                {
                    string query = "Select c.CustomerId,c.CustomerName,c.MobileNumber,c.Address,c.Email, cp.CustomerPurchaseId, cp.Discount,cp.GrandTotal,cp.PurchasedDate from Customer c join CustomerPurchase cp on c.CustomerId=cp.CustomerId where cp.DeletedFlag='N'";
                    using (SqlCommand cmd= new SqlCommand(query,conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        var reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            SalesModel model = new SalesModel();
                            CustomerModel customer = new CustomerModel();
                            CustomerPurchase customerpurchase = new CustomerPurchase();
                            customer.CustomerId = int.Parse(reader[0].ToString());
                            customer.CustomerName = reader[1].ToString();
                            customer.MobileNumber=long.Parse(reader[2].ToString());
                            customer.Address = reader[3].ToString();
                            customer.Email = reader[4].ToString();

                            customerpurchase.CustomerPurchaseId=int.Parse(reader[5].ToString());
                            customerpurchase.Discount = decimal.Parse(reader[6].ToString());
                            customerpurchase.GrandTotal = decimal.Parse(reader[7].ToString());
                            customerpurchase.PurchasedDate = DateTime.Parse(reader[8].ToString()).ToString("yyyy-MM-dd");

                            model.Customer = customer;
                            model.CustomerPurchase = customerpurchase;
                            modelList.Add(model);
                        }
                    }
                    return Json(modelList, JsonRequestBehavior.AllowGet);
                }

            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");
            }
        }

        public ActionResult GetSalesDetail(int CustomerPurchaseId)
        {
            try
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SalesModel model = new SalesModel();

                    string query = "Select c.CustomerId,c.CustomerName,c.MobileNumber,c.Address,c.Email, cp.CustomerPurchaseId, cp.Discount,cp.GrandTotal,cp.PurchasedDate from Customer c join CustomerPurchase cp on c.CustomerId=cp.CustomerId where cp.DeletedFlag='N' and cp.CustomerPurchaseId="+ CustomerPurchaseId;
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.CommandType = CommandType.Text;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
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

                            }
                        }
                    }

                        string medcinePurchasequery = "Select mp.CustomerPurchasedMedicineId,mp.MedicineId, mp.CustomerPurchasedId, mp.Quantity,mp.Rate,mp.Amount,mp.DeletedFlag, m.PackingType from CustomerPurchasedMedicine mp join Medicines m on mp.MedicineId=m.MedicineId where mp.DeletedFlag='N' and mp.CustomerPurchasedId=" + CustomerPurchaseId;
                        using (SqlCommand cmd= new SqlCommand(medcinePurchasequery,conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            var rdr = cmd.ExecuteReader();
                            while(rdr.Read())
                            {
                                CustomerPurchasedMedicine customerPurchasedMedicine = new CustomerPurchasedMedicine();
                                customerPurchasedMedicine.CustomerPurchasedMedicineId = int.Parse(rdr[0].ToString());
                                customerPurchasedMedicine.MedicineId = int.Parse(rdr[1].ToString());
                                customerPurchasedMedicine.CustomerPurchaseId = int.Parse(rdr[2].ToString());
                                customerPurchasedMedicine.Quantity = int.Parse(rdr[3].ToString());
                                customerPurchasedMedicine.Price = decimal.Parse(rdr[4].ToString());
                                customerPurchasedMedicine.TotalAmount = decimal.Parse(rdr[5].ToString());
                                customerPurchasedMedicine.DeletedFlag = char.Parse(rdr[6].ToString());
                                customerPurchasedMedicine.PackingType = rdr[7].ToString();
                                model.CustomerPurchasedMedicine.Add(customerPurchasedMedicine);
                            }
                        }
                    
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");
            }
        }

        [HttpPost]
        public bool EditSales(SalesModel model)
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
                        //updating customer if new then adding new customer
                        if (model.IsNewCustomer)
                        {

                            string customerQuery = "Insert into Customer (CustomerName,MobileNumber,Address,Email,CreatedDate,CreatedBy,DeletedFlag)" +
                                "values('" + model.Customer.CustomerName + "','" + model.Customer.MobileNumber + "','" + model.Customer.Address + "','" + model.Customer.Email + "','" + DateTime.Now + "'," + (int)Session["UserId"] + ",'N');" +
                                "SELECT SCOPE_IDENTITY();"; //getting the Id of inserted row

                            using (SqlCommand cmd = new SqlCommand(customerQuery, conn))
                            {
                                cmd.CommandType = CommandType.Text;
                                CustomerId = Convert.ToInt32(cmd.ExecuteScalar());

                            }
                        }
                        else
                        {
                            CustomerId = model.Customer.CustomerId;
                            string customerUpdatequery = "update Customer set CustomerName='" + model.Customer.CustomerName + "',MobileNumber='" + model.Customer.MobileNumber + "',Address='" + model.Customer.Address + "',Email='" + model.Customer.Email + "'where CustomerId=" + model.Customer.CustomerId;
                            using (SqlCommand cmd = new SqlCommand(customerUpdatequery, conn))
                            {                                
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        //Updating Customer Purchase
                        string customerPurchaseUpdatequery = "update CustomerPurchase set CustomerId='" + CustomerId + "',Discount='" + model.CustomerPurchase.Discount + "',GrandTotal='" + model.CustomerPurchase.GrandTotal + "',PurchasedDate='"+model.CustomerPurchase.PurchasedDate+"' where CustomerPurchaseId=" + model.CustomerPurchase.CustomerPurchaseId;
                        using (SqlCommand cmd = new SqlCommand(customerPurchaseUpdatequery, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }

                        string customerPurchasedMedicineQuery = String.Empty;
                        foreach (var data in model.CustomerPurchasedMedicine)
                        {
                            if(data.CustomerPurchasedMedicineId !=0)
                            {
                                customerPurchasedMedicineQuery += " Delete from CustomerPurchasedMedicine where CustomerPurchasedMedicineId=" + data.CustomerPurchasedMedicineId + ";";
                            }
                            //customerPurchasedMedicineQuery += "Update CustomerPurchasedMedicine set MedicineId='" + data.MedicineId + "',Quantity='" + data.Quantity + "',Rate='" + data.Price + "',Amount='" + data.TotalAmount + "' where CustomerPurchasedMedicineId="+data.CustomerPurchasedMedicineId+";";
                            customerPurchasedMedicineQuery += "Insert into CustomerPurchasedMedicine(MedicineId,CustomerPurchasedId,Quantity,Rate,Amount,DeletedFlag)" +
                                "values('" + data.MedicineId + "','" + model.CustomerPurchase.CustomerPurchaseId + "','" + data.Quantity + "','" + data.Price + "','" + data.TotalAmount + "','N');";
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
                    transactionscope.Dispose();
                    return false;
                }
            }
        }

    }
}