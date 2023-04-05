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
                    string query = "Select SupplierId, SupplierName,Email,Address,ContactNumber,CreatedDate from Suppliers where DeletedFlag='N' ";
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
                                model.ContactNumber = int.Parse(reader[4].ToString());
                                model.CreatedDate = DateTime.Parse(reader[5].ToString());
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


    }
}