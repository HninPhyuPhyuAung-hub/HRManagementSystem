using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class SalaryCheck
    {
        public int payId { get; set; }
        public int EmpId { get; set; }
        public DateTime? paydate { get; set; }
        public decimal basicsalary { get; set; }
        public decimal rateprhour { get; set; }
        public decimal allowance { get; set; }
        public decimal OT { get; set; }
        public decimal Absencehour { get; set; }
        public decimal cutsalary { get; set; }
        public decimal grosspay { get; set; }
        public decimal netpay { get; set; }
        public string EmailAddress { get; set; }
        public string Position { get; set; }
        public int EntityId
        {
            get { return payId; }
            set { payId = value; }
        }
    }
}