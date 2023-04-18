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

    public class MedicineController : BaseController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        // GET: Medicine
        public ActionResult AddMedicine()
        {
            return View();
        }

        [HttpPost]
        public bool AddMedicine(MedicineModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Insert into Medicines (MedicineName, Description, Price, CreatedDate, CreatedBy, DeletedFlag, PackingType, ExpiryDate) values('" + model.MedicineName + "','" + model.Description + "','" + model.Price + "','" + DateTime.Now + "','" + (int)Session["UserId"] + "','N', '"+model.PackingType+"', '" + model.ExpiryDate + "')";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        return true;

                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        [HttpGet]
        public ActionResult EditMedicine()
        {
            return View();
        }

        public ActionResult GetMedicine()
        {
            List<MedicineModel> medicinelst = new List<MedicineModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select Medicineid, MedicineName, Description, Price, PackingType, CreatedDate, ExpiryDate from Medicines where DeletedFlag='N'"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MedicineModel medicine = new MedicineModel(); //creating customer object
                                medicine.MedicineId = int.Parse(reader[0].ToString());
                                medicine.MedicineName = reader[1].ToString();
                                medicine.Description = reader[2].ToString();
                                medicine.Price = decimal.Parse(reader[3].ToString());
                                medicine.PackingType = reader[4].ToString();
                                medicine.CreatedDate = DateTime.Parse(reader[5].ToString()).ToString("yyyy-MM-dd");
                                medicine.ExpiryDate = DateTime.Parse(reader[6].ToString()).ToString("yyyy-MM-dd");
                                medicinelst.Add(medicine);
                            }
                            return Json(medicinelst, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json(medicinelst, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpPost]
        public bool EditMedicine(MedicineModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Update Medicines  set MedicineName='" + model.MedicineName + "', Price='" + model.Price + "', Description='" + model.Description + "', PackingType='" + model.PackingType + "', ExpiryDate='" + model.ExpiryDate + "' where MedicineId=" + model.MedicineId + "";
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
        public bool RemoveMedicine(MedicineModel medicine)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryCount = "Update Medicines set DeletedFlag='Y' where MedicineId = " + medicine.MedicineId + ";";
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