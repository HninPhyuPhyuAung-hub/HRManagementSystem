
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class EmployeePaging
    {
        public IPagedList<Employees> employeeList { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string nextLink { get; set; }
        public string prevLink { get; set; }
    }

    public class EmployeeList
    {
        public IEnumerable<Employees> employeeList { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string nextLink { get; set; }
        public string prevLink { get; set; }
    }
}