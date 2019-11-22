using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class leavedetail
    {
        public int? casual { get; set; }
        public int? paid { get; set; }
        public int? sick { get; set; }
        public int? unpaid { get; set; }
    }
}