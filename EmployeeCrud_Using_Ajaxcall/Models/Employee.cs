using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeCrud_Using_Ajaxcall.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int StateID { get; set; }
        public virtual State State { get; set; }
        public int CountryID { get; set; }
        public virtual Country Country { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool IsMarried { get; set; }
        public decimal Salary { get; set; }
        public string Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MobileNumber { get; set; }
    }
}