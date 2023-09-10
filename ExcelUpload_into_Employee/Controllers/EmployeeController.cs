using ExcelUpload_into_Employee.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcelUpload_into_Employee.Controllers
{

    public class Employee1Controller : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        EmployeeDB empDB = new EmployeeDB();

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult List()
        {
            return Json(empDB.ListAll(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Add(Employee emp)
        {
            return Json(empDB.Add(emp), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetbyID(int EmployeeID)
        {
            var Employee = empDB.ListAll().Find(x => x.EmployeeID.Equals(EmployeeID));
            return Json(Employee, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update(Employee emp)
        {
            return Json(empDB.Update(emp), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(int EmployeeID)
        {
            return Json(empDB.Delete(EmployeeID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStatesByCountry(int countryId)
        {
            var states = empDB.GetStatesByCountry(countryId);
            return Json(states, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCountries()
        {
            return Json(empDB.GetCountries(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportToExcel()
        {
            var employees = empDB.ListAll();

            // Create an Excel package using EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Add column headers
                worksheet.Cells["A1"].Value = "EmployeeID";
                worksheet.Cells["B1"].Value = "Name";
                worksheet.Cells["C1"].Value = "Age";
                worksheet.Cells["D1"].Value = "Designation";
                worksheet.Cells["E1"].Value = "Email";
                worksheet.Cells["F1"].Value = "Joined Date";
                worksheet.Cells["G1"].Value = "Is Married";
                worksheet.Cells["H1"].Value = "Salary";
                worksheet.Cells["I1"].Value = "Role";
                worksheet.Cells["J1"].Value = "State";
                worksheet.Cells["K1"].Value = "Country";
                worksheet.Cells["J1"].Value = "DateOfBirth";
                worksheet.Cells["K1"].Value = "MobileNumber";

                // Add employee data
                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells["A" + row].Value = employee.EmployeeID;
                    worksheet.Cells["B" + row].Value = employee.Name;
                    worksheet.Cells["C" + row].Value = employee.Age;
                    worksheet.Cells["D" + row].Value = employee.Designation;
                    worksheet.Cells["E" + row].Value = employee.Email;
                    worksheet.Cells["F" + row].Value = employee.JoinedDate;
                    worksheet.Cells["G" + row].Value = employee.IsMarried ? "Yes" : "No";
                    worksheet.Cells["H" + row].Value = employee.Salary;
                    worksheet.Cells["I" + row].Value = employee.Role;
                    worksheet.Cells["J" + row].Value = employee.StateName;
                    worksheet.Cells["K" + row].Value = employee.CountryName;
                    worksheet.Cells["J" + row].Value = employee.DateOfBirth;
                    worksheet.Cells["K" + row].Value = employee.MobileNumber;

                    row++;
                }

                // Set the response content type and headers
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=Employees.xlsx");

                // Write the Excel package to the response output stream
                Response.BinaryWrite(package.GetAsByteArray());
                Response.End();
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase file)
        {
            
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Define the path to save the uploaded file temporarily
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    file.SaveAs(path);

                    // Read data from the uploaded Excel file using OleDb
                    var connectionString = GetExcelConnectionString(path);
                    var dataTable = ReadExcelData(connectionString);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        // Process the data and insert into SQL tables
                        ImportDataIntoDatabase(dataTable);

                        // Optionally, delete the temporary Excel file
                        System.IO.File.Delete(path);

                        ViewBag.Message = "File uploaded and data imported successfully.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "The uploaded Excel file is empty or invalid.";
                    }
                }
              
                catch (SqlException sqlEx)
                {
                    Console.WriteLine("SQL Error: " + sqlEx.Message);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please select a file to upload.";
            }

            return View();
        }


        private string GetExcelConnectionString(string filePath)
        {
            // Connection string for Excel file based on the file extension
            var fileExtension = Path.GetExtension(filePath);
            string connectionString = "";

            if (fileExtension == ".xls")
            {
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=YES;'";
            }
            else if (fileExtension == ".xlsx")
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0;HDR=YES;'";
            }

            return connectionString;
        }

        private DataTable ReadExcelData(string connectionString)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", conn);
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        private void ImportDataIntoDatabase(DataTable dataTable)
        {
            string connectionString = cs;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create dictionaries to map StateName to StateID and CountryName to CountryID
                Dictionary<string, int> stateNameToStateID = new Dictionary<string, int>();
                Dictionary<string, int> countryNameToCountryID = new Dictionary<string, int>();

                // Populate the dictionaries with data from the State and Country tables
                using (SqlCommand stateCommand = new SqlCommand("SELECT StateID, StateName FROM State", connection))
                {
                    using (SqlDataReader stateReader = stateCommand.ExecuteReader())
                    {
                        while (stateReader.Read())
                        {
                            int stateID = stateReader.GetInt32(0);
                            string stateName = stateReader.GetString(1);
                            stateNameToStateID[stateName] = stateID;
                        }
                    }
                }

                using (SqlCommand countryCommand = new SqlCommand("SELECT CountryID, CountryName FROM Country", connection))
                {
                    using (SqlDataReader countryReader = countryCommand.ExecuteReader())
                    {
                        while (countryReader.Read())
                        {
                            int countryID = countryReader.GetInt32(0);
                            string countryName = countryReader.GetString(1);
                            countryNameToCountryID[countryName] = countryID;
                        }
                    }
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    double mobileNumber;
                    if (!double.TryParse(row["MobileNumber"].ToString(), out mobileNumber))
                    {
                        // Handle the case where parsing fails (e.g., log an error or use a default value).
                        // For simplicity, we're skipping the row in this example.
                        Console.WriteLine("Error parsing MobileNumber. Skipping row.");
                        continue;
                    }

                    // Check for duplicates based on Name, DateOfBirth, and MobileNumber
                    using (SqlCommand duplicateCheckCommand = new SqlCommand())
                    {
                        duplicateCheckCommand.Connection = connection;
                        string duplicateCheckQuery = "SELECT COUNT(*) FROM Employee WHERE Name = @Name AND DateOfBirth = @DateOfBirth AND MobileNumber = @MobileNumber";
                        duplicateCheckCommand.CommandText = duplicateCheckQuery;
                        duplicateCheckCommand.Parameters.AddWithValue("@Name", row["Name"]);
                        duplicateCheckCommand.Parameters.AddWithValue("@DateOfBirth", row["DateOfBirth"]);
                        duplicateCheckCommand.Parameters.AddWithValue("@MobileNumber", mobileNumber);

                        int duplicateCount = (int)duplicateCheckCommand.ExecuteScalar();

                        if (duplicateCount == 0)
                        {
                            // Example SQL query to insert data into the Employee table
                            string insertQuery = "INSERT INTO Employee (Name, Age, StateID, CountryID, Designation, Email, JoinedDate, IsMarried, Salary, Role, DateOfBirth, MobileNumber) " +
                            "VALUES (@Name, @Age, @StateID, @CountryID, @Designation, @Email, @JoinedDate, @IsMarried, @Salary, @Role, @DateOfBirth, @MobileNumber)";

                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@Name", row["Name"]);
                                command.Parameters.AddWithValue("@Age", row["Age"]);

                                string stateName = row["StateID"] != null ? row["StateID"].ToString() : string.Empty;
                                string countryName = row["CountryID"] != null ? row["CountryID"].ToString() : string.Empty;

                                if (stateNameToStateID.ContainsKey(stateName))
                                {
                                    // Map StateName to StateID
                                    int stateID = stateNameToStateID[stateName];
                                    command.Parameters.AddWithValue("@StateID", stateID);
                                }
                                else
                                {
                                    // Handle the case where StateName is not found in the dictionary
                                    Console.WriteLine($"StateName not found: {stateName}");
                                    // Here, you might want to set a default StateID or skip the row.
                                    // For simplicity, setting it to DBNull.Value:
                                    command.Parameters.AddWithValue("@StateID", DBNull.Value);
                                }

                                if (countryNameToCountryID.ContainsKey(countryName))
                                {
                                    // Map CountryName to CountryID
                                    int countryID = countryNameToCountryID[countryName];
                                    command.Parameters.AddWithValue("@CountryID", countryID);
                                }
                                else
                                {
                                    // Handle the case where CountryName is not found in the dictionary
                                    Console.WriteLine($"CountryName not found: {countryName}");
                                    // Here, you might want to set a default CountryID or skip the row.
                                    // For simplicity, setting it to DBNull.Value:
                                    command.Parameters.AddWithValue("@CountryID", DBNull.Value);
                                }

                                // Continue setting other parameters
                                command.Parameters.AddWithValue("@Designation", row["Designation"]);
                                command.Parameters.AddWithValue("@Email", row["Email"]);
                                command.Parameters.AddWithValue("@JoinedDate", row["JoinedDate"]);
                                command.Parameters.AddWithValue("@IsMarried", row["IsMarried"]);
                                command.Parameters.AddWithValue("@Salary", row["Salary"]);
                                command.Parameters.AddWithValue("@Role", row["Role"]);
                                command.Parameters.AddWithValue("@DateOfBirth", row["DateOfBirth"]);
                                command.Parameters.AddWithValue("@MobileNumber", mobileNumber);

                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Handle the duplicate record (e.g., log or skip it)
                            Console.WriteLine("Duplicate record found and skipped.");
                        }
                    }
                }
            }
        }

    }
}