using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Message : IIdentifiableEntity
    {
        public int ID { get; set; }
       
        public string email { get; set; }
        public string message { get; set; }
        public string sendmail { get; set; }
        public string subject { get; set; }
        public int EntityId
        { get { return ID; }
            set { value = ID; }
        }
    }
}