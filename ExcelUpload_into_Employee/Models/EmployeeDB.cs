using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ExcelUpload_into_Employee.Models
{
    public class EmployeeDB
    {
        string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<Employee> ListAll()
        {
            List<Employee> lst = new List<Employee>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("SelectEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    lst.Add(new Employee
                    {
                        EmployeeID = Convert.ToInt32(rdr["EmployeeId"]),
                        Name = rdr["Name"].ToString(),
                        Age = Convert.ToInt32(rdr["Age"]),
                        Designation = rdr["Designation"].ToString(),
                        Email = rdr["Email"].ToString(),
                        JoinedDate = Convert.ToDateTime(rdr["JoinedDate"]),
                        IsMarried = Convert.ToBoolean(rdr["IsMarried"]),
                        Salary = Convert.ToDecimal(rdr["Salary"]),
                        Role = rdr["Role"].ToString(),
                        StateID = Convert.ToInt32(rdr["StateID"]),
                        CountryID = Convert.ToInt32(rdr["CountryID"]),
                        StateName = rdr["State"].ToString(),
                        CountryName = rdr["Country"].ToString(),
                        DateOfBirth = Convert.ToDateTime(rdr["DateOfBirth"]),  // Handle nullable DateOfBirth
                        MobileNumber = rdr["MobileNumber"].ToString()  // Handle MobileNumber
                    });
                }
                return lst;
            }
        }



        public int Add(Employee emp)
        {
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("InsertUpdateEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", emp.EmployeeID);
                com.Parameters.AddWithValue("@Name", emp.Name);
                com.Parameters.AddWithValue("@Age", emp.Age);
                com.Parameters.AddWithValue("@Designation", emp.Designation);
                com.Parameters.AddWithValue("@Email", emp.Email);
                com.Parameters.AddWithValue("@JoinedDate", emp.JoinedDate);
                com.Parameters.AddWithValue("@IsMarried", emp.IsMarried);
                com.Parameters.AddWithValue("@Salary", emp.Salary);
                com.Parameters.AddWithValue("@Role", emp.Role);
                com.Parameters.AddWithValue("@StateID", emp.StateID);
                com.Parameters.AddWithValue("@CountryID", emp.CountryID);
                com.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                com.Parameters.AddWithValue("@MobileNumber", emp.MobileNumber);
                com.Parameters.AddWithValue("@Action", "Insert");
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int Update(Employee emp)
        {
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("InsertUpdateEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", emp.EmployeeID);
                com.Parameters.AddWithValue("@Name", emp.Name);
                com.Parameters.AddWithValue("@Age", emp.Age);
                com.Parameters.AddWithValue("@Designation", emp.Designation);
                com.Parameters.AddWithValue("@Email", emp.Email);
                com.Parameters.AddWithValue("@JoinedDate", emp.JoinedDate);
                com.Parameters.AddWithValue("@IsMarried", emp.IsMarried);
                com.Parameters.AddWithValue("@Salary", emp.Salary);
                com.Parameters.AddWithValue("@Role", emp.Role);
                com.Parameters.AddWithValue("@StateID", emp.StateID);
                com.Parameters.AddWithValue("@CountryID", emp.CountryID);
                com.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                com.Parameters.AddWithValue("@MobileNumber", emp.MobileNumber);
                com.Parameters.AddWithValue("@Action", "Update");
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int Delete(int EmployeeID)
        {
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("DeleteEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", EmployeeID);
                i = com.ExecuteNonQuery();
            }
            return i;
        }



        public List<Country> GetCountries()
        {
            List<Country> countries = new List<Country>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Country", con);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Country country = new Country
                    {
                        CountryID = Convert.ToInt32(rdr["CountryID"]),
                        CountryName = rdr["CountryName"].ToString()
                    };
                    countries.Add(country);
                }
            }
            return countries;
        }

        public List<State> GetStatesByCountry(int countryId)
        {
            List<State> states = new List<State>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM State WHERE CountryID = @CountryID", con);
                cmd.Parameters.AddWithValue("@CountryID", countryId);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    State state = new State
                    {
                        StateID = Convert.ToInt32(rdr["StateID"]),
                        StateName = rdr["StateName"].ToString(),
                        CountryID = Convert.ToInt32(rdr["CountryID"])
                    };
                    states.Add(state);
                }
            }
            return states;
        }

    }
}