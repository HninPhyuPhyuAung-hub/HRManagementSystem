using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Recircument : IIdentifiableEntity
    {
        public int ReId { get; set; }
        public string Position { get; set; }
        public int numberofvac { get; set; }
        public DateTime? date { get; set; }
       
        public int EntityId
        {
            get { return ReId; }
            set { value = ReId; }
        }
    }
}