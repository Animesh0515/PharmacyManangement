using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PharmacyManagement.Controllers
{
    
    public class SupplierController : BaseController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        // GET: Supplier
        public ActionResult AddSupplier()
        {
            return View();
        } 
        [HttpPost]
        public bool AddSupplier(SupplierModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Insert into Suppliers (SupplierName, ContactNumber,Email, Address,CreatedDate,CreatedBy,DeletedFlag) values('" + model.SupplierName + "','" + model.ContactNumber + "','" + model.Email + "','" + model.Address + "','"+DateTime.Now+"','"+ (int)Session["UserId"] + "','N')";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        return true;

                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult EditSupplier()
        {
            return View();
        }

        public ActionResult GetSupplier()
        {
            List<SupplierModel> modelList = new List<SupplierModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select SupplierId, SupplierName,Email,Address,ContactNumber,CreatedDate from Suppliers where DeletedFlag='N' order by CreatedDate desc ";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (var reader= cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                SupplierModel model = new SupplierModel();
                                model.SupplierId = int.Parse(reader[0].ToString());
                                model.SupplierName = reader[1].ToString();
                                model.Email = reader[2].ToString();
                                model.Address = reader[3].ToString();
                                model.ContactNumber = Int64.Parse(reader[4].ToString());
                                model.CreatedDate = DateTime.Parse(reader[5].ToString()).ToString("yyyy-MM-dd");                                
                                modelList.Add(model);
                            }
                        }
                    }
                    return Json(modelList, JsonRequestBehavior.AllowGet);
                    
                }
            }
            catch (Exception ex)
            {
                return Json(modelList, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public bool EditSupplier(SupplierModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Update Suppliers  set SupplierName='"+model.SupplierName+"', ContactNumber='"+model.ContactNumber+"',Address='"+model.Address+"',Email='"+model.Email+"' where SupplierId="+model.SupplierId+"";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
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

        [HttpPost]
        public bool RemoveSupplier(SupplierModel supplier)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryCount = "Update Suppliers set DeletedFlag='Y' where SupplierId = " + supplier.SupplierId + ";";
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