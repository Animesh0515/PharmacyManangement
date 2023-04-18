using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PharmacyManagement.Controllers
{
    
    public class ProfileController : BaseController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        [HttpGet]
        public ActionResult EditProfile()
        {
            return View();
        }

        public ActionResult GetProfile()
        {
            List<UserModel> userlst = new List<UserModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var id = Session["UserId"];
                    string query = "Select UserId, Name, MobileNumber, Address, Email, Username, Password, Role, Image from Users where DeletedFlag='N' and UserId = " + id + ";"; //getting all user that are not deleted. DeletedFlag='N' denotes not deleted.
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserModel user = new UserModel(); //creating customer object
                                user.UserId = int.Parse(reader[0].ToString());
                                user.Name = reader[1].ToString();
                                user.MobileNumber = int.Parse(reader[2].ToString());
                                user.Address = reader[3].ToString();
                                user.Email = reader[4].ToString();
                                user.Username = reader[5].ToString();
                                user.Password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader[6].ToString())).ToString();
                                user.Role =char.Parse(reader[7].ToString());
                                user.Image = reader[8].ToString();
                                userlst.Add(user);
                            }
                            return Json(userlst, JsonRequestBehavior.AllowGet);

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
        public bool EditProfile(UserModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var id = Session["UserId"];
                    string query = "Update Users  set Name='" + model.Name + "', MobileNumber='" +model.MobileNumber + "', Address='" + model.Address + "', Email='" + model.Email + "', Username ='" + model.Username + "', Password='" + Utility.AccountCreationHelper.Base64Encode(model.Password) + "' where UserId=" + id + ";";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        Session["Username"] = model.Username;
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
        public bool UploadImage()
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                var userID = Request.Form[0];
                var fileName = Guid.NewGuid().ToString() + ".jpg";                
                string path = Path.Combine(Server.MapPath("~/Assets/Profile/"), fileName);
                file.SaveAs(path);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();                    
                    string query = "Update Users  set Image='/Assets/Profile/"+fileName+"' where UserId=" + userID + ";";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();                        
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }


        [HttpPost]
        public bool DeleteProfileImage(int UserId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Update Users  set Image=NULL where UserId=" + UserId + ";";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }


    }
}