using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Leave : IIdentifiableEntity
    {
        public int LeaveId { get; set; }
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Days { get; set; }
        public string Reason { get; set; }
        public string SupervisorProve { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string LeaveReason { get; set; }
        public string Position { get; set; }
        //public int? total { get; set; }
        //public string leavetype { get; set; }
        public int EntityId
        {
            get { return LeaveId; }
            set { value = LeaveId; }
        }
    }
}