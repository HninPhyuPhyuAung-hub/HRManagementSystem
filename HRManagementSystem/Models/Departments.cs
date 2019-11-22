using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Departments : IIdentifiableEntity
    {
        public int Id { get; set; }
          public string DpName { get; set; }
        
        public int EntityId
        {
            get { return Id; }
            set { Id = value; }
        }
    }
}