using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class notice : IIdentifiableEntity

    {
        public int ID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public DateTime? date { get; set; }
        public int EntityId
        {
            get { return ID; }
            set { value = ID; }
        }
    }
}