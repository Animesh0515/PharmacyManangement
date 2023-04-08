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

    public class MedicineController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        // GET: Medicine
        public ActionResult GetMedicine()
        {
            List<MedicineModel> medicinelst = new List<MedicineModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select * from Medicines where DeletedFlag='N'"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
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
                                medicine.CreatedDate = DateTime.Parse(reader[4].ToString());
                                medicine.CreatedBy = int.Parse(reader[5].ToString());
                                medicine.DeletedFlag = char.Parse(reader[6].ToString());
                                medicine.PackingType = reader[7].ToString();
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
    }
}