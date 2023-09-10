
using ExcelUpload_into_Employee.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcelUpload_into_Employee.Controllers
{
    public class EmployeeUploadController : Controller
    {

        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DisplayExcelData(IFormFile excelFile)
        {
            List<Employee> employees = new List<Employee>();

            if (excelFile != null && excelFile.Length > 0)
            {
                using (var package = new ExcelPackage(excelFile.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        employees.Add(new Employee
                        {
                            Name = worksheet.Cells[row, 1].Text,
                            Age = int.Parse(worksheet.Cells[row, 2].Text),
                            StateName = worksheet.Cells[row, 3].Text,
                            CountryName = worksheet.Cells[row, 4].Text,
                            Designation = worksheet.Cells[row, 5].Text,
                            Email = worksheet.Cells[row, 6].Text,
                            JoinedDate = DateTime.Parse(worksheet.Cells[row, 7].Text),
                            IsMarried = bool.Parse(worksheet.Cells[row, 8].Text),
                            Salary = decimal.Parse(worksheet.Cells[row, 9].Text),
                            Role = worksheet.Cells[row, 10].Text,
                            DateOfBirth = DateTime.Parse(worksheet.Cells[row, 11].Text),
                            MobileNumber = worksheet.Cells[row, 12].Text
                        });
                    }
                }
            }

            return PartialView("_EmployeeTablePartial", employees);
        }

    }
}