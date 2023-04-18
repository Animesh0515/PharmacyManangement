using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PharmacyManagement.Controllers
{
    public class AccountController : Controller
    {
        //defining connection string
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public bool Login(LoginModel model)
        {
            try
            {
                var encodedPasssword = Utility.AccountCreationHelper.Base64Encode(model.Password);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select UserId,UserName,Password,Role from users where username='"+model.Username+"' and DeletedFlag='N'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userPassword = reader[2].ToString();
                                if (userPassword != null)
                                {
                                    if (encodedPasssword == userPassword.ToString())
                                    {
                                        Session["UserId"] = int.Parse(reader[0].ToString());
                                        Session["Username"] = reader[1].ToString();
                                        Session["Role"] = reader[3].ToString();
                                        Session["IsLoggedIn"] = true;
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;

                                }
                            }
                            return false;
                        }

                    }
                }
               

            }
            catch (Exception)
            {
                return false;

            }
        }

        public ActionResult Signup()
        {
            return View();
        }

        //Need to check username
        [HttpPost]
        public bool Signup(UserModel users)
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
    }
}