using PharmacyManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Net.Http.Formatting;

namespace PharmacyManagement.Controllers
{
    public class CustomerController : BaseController
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();

         [HttpGet]
        public ActionResult EditCustomer()
        {
            return View();
        }

        //Fetching all data of Customer
        public ActionResult GetCustomer()
        {
            List<CustomerModel> customerlist = new List<CustomerModel>(); //creating list for customer data

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select CustomerId, CustomerName, MobileNumber, Address, Email from Customer where DeletedFlag='N'"; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader=cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                CustomerModel customer = new CustomerModel(); //creating customer object
                                customer.CustomerId = int.Parse(reader[0].ToString());
                                customer.CustomerName = reader[1].ToString();
                                customer.MobileNumber = long.Parse(reader[2].ToString());
                                customer.Address = reader[3].ToString();
                                customer.Email = reader[4].ToString();
                                customerlist.Add(customer);
                            }
                            return Json(customerlist, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "An error occurred while retrieving the data. Please try again later.");

            }
        }


        //getting customer detail of particualr customer
        [HttpPost]
        public ActionResult GetIndividualCustomer(int CustomerId)
        {
            List<CustomerModel> customerlist = new List<CustomerModel>(); //creating list for customer data

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Select * from Customer where DeletedFlag='N' and CustomerId="+CustomerId; //getting all customer data that are not deleted. DeletedFlag='N' denotes not deleted.
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader=cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                CustomerModel customer = new CustomerModel(); //creating customer object
                                customer.CustomerId = int.Parse(reader[0].ToString());
                                customer.CustomerName = reader[1].ToString();
                                customer.MobileNumber = long.Parse(reader[2].ToString());
                                customer.Address = reader[3].ToString();
                                customer.Email = reader[4].ToString();
                                customerlist.Add(customer);
                            }
                            return Json(customerlist, JsonRequestBehavior.AllowGet);

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
        public bool EditCustomer(CustomerModel customer)
        {
            try
            {
                using (SqlConnection conn= new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Update Customer set CustomerName='"+customer.CustomerName+"', MobileNumber='"+customer.MobileNumber+"', Address='"+customer.Address+"',Email='"+customer.Email+"' where CustomerId='"+ customer.CustomerId + "'";
                    using (SqlCommand cmd= new SqlCommand(query,conn))
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
        public bool DeleteCustomer(int Id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "Update Customer set DeletedFlag='Y' where CustomerId="+Id;
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
        public bool RemoveCustomer(CustomerModel customer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryCount = "Update Customer set DeletedFlag='Y' where CustomerId = " + customer.CustomerId + ";";
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