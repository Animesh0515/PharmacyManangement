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
    
    public class UserController : BaseController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        public ActionResult AddUser()
        {
            return View();
        }
        
        [HttpPost]
        public bool AddUser(UserModel users)
        {
            try
            {
                string encodedPassword = Utility.AccountCreationHelper.Base64Encode(users.Password);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Insert into Users (Name,MobileNumber,Address,Email,Username,Password,Role,DeletedFlag)" +
                        "values('" + users.Name + "','" + users.MobileNumber + "','" + users.Address + "','" + users.Email + "','" + users.Username + "','" + encodedPassword + "','" + users.Role + "','N')";
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
        public ActionResult EditUser()
        {
            return View();
        }

        public ActionResult GetUser()
        {
            List<UserModel> userlst = new List<UserModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select UserId, Name, MobileNumber, Address, Email, Username, Password, Role from Users where DeletedFlag='N'"; //getting all user that are not deleted. DeletedFlag='N' denotes not deleted.
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
                                reader[6].ToString();
                                user.Role =char.Parse(reader[7].ToString());
                                userlst.Add(user);
                            }
                            return Json(userlst, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(userlst, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public bool EditUser(UserModel model)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Update Users  set Name='" + model.Name + "', MobileNumber='" +model.MobileNumber + "', Address='" + model.Address + "', Email='" + model.Email + "', Username ='" + model.Username + "', Password='" + Utility.AccountCreationHelper.Base64Encode(model.Password) + "', Role='" + model.Role + "' where UserId=" + model.UserId + ";";
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
        public bool RemoveUser(UserModel user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryCount = "Update Users set DeletedFlag='Y' where UserId = " + user.UserId + ";";
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