using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Leaejson
    {
        public string[] Name { get; set; }
        public string[] Department { get; set; }
        public string[] Position { get; set; }
        public int[] Days { get; set; }
        public int[] LeaveId { get; set; }
        public string[] FromDate { get; set; }
        public string[] TodDate { get; set; }
        public string[] Reason { get; set; }
        public string[] LeaveReason { get; set; }
        public string[] SupervisorProve { get; set; }
        public int[] EmpId { get; set; }
       
    }

    public class chartjson
    {
        public string[] date { get; set; }
        public int[] count { get; set; }
    }

    public class paymentjson
    {
        public string[] Name { get; set; }
        public int[] EmpId { get; set; }
        public decimal?[] totalallownace { get; set; }
        public decimal?[] total_deduction { get; set; }
        public int[] salaryid { get; set; }
        public decimal?[] tax { get; set; }
        public decimal?[] net_salary { get; set; }
        public string[] status { get; set; }
        public decimal?[] ot { get; set; }
        public decimal?[] bonus { get; set; }
        public decimal?[] basic { get; set; }
        public string[] date { get; set; }
        public string[] Bankacc { get; set; }
        public decimal?[] unpaid { get; set; }
        public decimal?[] hour { get; set; }


    }

}