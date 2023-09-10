using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcelUpload_into_Employee.Models
{
    public class State
    {

        public int StateID { get; set; }
        public string StateName { get; set; }
        public int CountryID { get; set; }
        public virtual Country Country { get; set; }
    }
}