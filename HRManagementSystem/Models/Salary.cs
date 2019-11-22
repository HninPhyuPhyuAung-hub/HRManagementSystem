using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Salary: IIdentifiableEntity
    {
        public int salaryid { get; set; }
        public decimal? basic { get; set; }
        public decimal? totalallownace { get; set; }
          public decimal? total_deduction { get; set; }
            public decimal? tax { get; set; }
           public decimal? net_salary { get; set; }
           public string status { get; set; }
           public int EmpId { get; set; }
           public decimal? unpaid { get; set; }
           public decimal? hour { get; set; }
            public DateTime? date { get; set; }
        public string Name { get; set; }
        public string Bankacc { get; set; }
        public decimal? ot { get; set; }
        public decimal? bonus { get; set; }
        public int EntityId
        {
            get { return salaryid; }
            set {  salaryid=value; }
        }
    }
}