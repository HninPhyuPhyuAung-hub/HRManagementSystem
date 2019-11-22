using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Expense : IIdentifiableEntity
    {
        public int ID { get; set; }

        public string Title { get; set; }
        public string descriptiion { get; set; }
        public int? amount { get; set; }
        public DateTime? date { get; set; }
        public int EntityId
        {
            get { return ID; }
            set { value = ID; }
        }
    }
}