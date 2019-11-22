﻿using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Account : IIdentifiableEntity
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public Boolean IsDeleted { get; set; }
        public int EmpId { get; set; }
        public int EntityId
        {
            get { return ID; }
            set { value = ID; }
        }
    }
}