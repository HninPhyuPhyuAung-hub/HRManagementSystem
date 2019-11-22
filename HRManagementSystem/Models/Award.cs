using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Award : IIdentifiableEntity
    {
        public int ID { get; set; }
        public string gift { get; set; }
        public string awardname { get; set; }
        public int? amount { get; set; }
        public string awardemployee { get; set; }
        
        public DateTime? date { get; set; }
        public int EntityId
        {
            get { return ID; }
            set { value = ID; }
        }
    }
}