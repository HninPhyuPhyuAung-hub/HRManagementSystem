using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Application : IIdentifiableEntity
    {
        public int AppId { get; set; }
        public string ApplicantName { get; set; }
        public string Position { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public int EntityId
        {
            get { return AppId; }
            set { value = AppId; }
        }
    }
}