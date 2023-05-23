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
                    string query = "Select Medicineid, MedicineName, Description, Price, PackingType, CreatedDate, Quantity from Medicines where DeletedFlag='N'"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
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
                                medicine.Quantity=int.Parse(reader[6].ToString());
                                medicinelst.Add(medicine);
                            }
                            return Json(medicinelst, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");


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
                    string query = "Update Medicines  set MedicineName='" + model.MedicineName + "', Price='" + model.Price + "', Description='" + model.Description + "', PackingType='" + model.PackingType + "',Quantity='"+model.Quantity+ "' where MedicineId=" + model.MedicineId + "";
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

        public void updateStock(int MedicineId, int Quantity,string Action)
        {
            using (SqlConnection conn= new SqlConnection(connectionString))
            {
                conn.Open();
                string query = String.Empty;
                if(Action == "Add")
                {
                    query = "Update Medicines set Quantity=Quantity+" + Quantity + " where MedicineId=" + MedicineId + ";";
                }
                else
                {
                    query = "Update Medicines set Quantity=Quantity-" + Quantity + " where MedicineId=" + MedicineId + ";";
                }
                using (SqlCommand cmd= new SqlCommand(query,conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }



        [HttpGet]
        public ActionResult ExpiredMedicine()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetExpiredMedicine()
        {
            
            try
            {
                List <ExpiredMedicineModel> medicinelst = new List <ExpiredMedicineModel>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT m.MedicineId, m.MedicineName, m.Description, m.PackingType  FROM Medicines m INNER JOIN MedicinePurchased mp ON m.MedicineId = mp.MedicineId WHERE m.DeletedFlag = 'N' and mp.ExpiryDate < '" + DateTime.Now.ToString("yyyy-MM-dd") + "' GROUP BY m.MedicineId, m.MedicineName, m.Description, m.PackingType;"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MedicineModel medicine = new MedicineModel();
                                ExpiredMedicineModel expMedicines = new ExpiredMedicineModel();
                                medicine.MedicineId = int.Parse(reader[0].ToString());
                                medicine.MedicineName = reader[1].ToString();
                                medicine.Description = reader[2].ToString();
                                medicine.PackingType = reader[3].ToString();
                                expMedicines.Medicine = medicine;
                                var ExpiredMedicineDetail = getDetail(medicine.MedicineId);
                                expMedicines.ExpiredMedicineDetail = ExpiredMedicineDetail;
                                medicinelst.Add(expMedicines);
                            }
                        }
                    }
                    return Json(medicinelst, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");
            }
        }

        public List<ExpiredMedicineDetail> getDetail(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                List<ExpiredMedicineDetail> medicineDetails = new List<ExpiredMedicineDetail>();
                conn.Open();
                string queryForMP = "SELECT s.SupplierName, mp.ExpiryDate, p.BatchNumber FROM Suppliers s INNER JOIN purchase p ON s.SupplierId = p.SupplierId INNER JOIN MedicinePurchased mp ON p.PurchaseId = mp.PurchaseId WHERE mp.ExpiryDate < '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND mp.medicineid =" + id + " AND mp.DeletedFlag = 'N' AND p.DeletedFlag = 'N'"; //getting all expired medicine details that are not deleted. DeletedFlag='N' denotes not deleted.
                using (SqlCommand command = new SqlCommand(queryForMP, conn))
                {

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader readerForMP = command.ExecuteReader())
                    {
                        while (readerForMP.Read())
                        {
                            ExpiredMedicineDetail data = new ExpiredMedicineDetail();
                            data.SupplierName = readerForMP[0].ToString();
                            data.BatchNumber = int.Parse(readerForMP[2].ToString());
                            data.ExpiryDate = DateTime.Parse(readerForMP[1].ToString()).ToString("yyyy-MM-dd");
                            medicineDetails.Add(data);
                            
                        }
                        return medicineDetails;
                    }
                }
            }
        }

    }


}