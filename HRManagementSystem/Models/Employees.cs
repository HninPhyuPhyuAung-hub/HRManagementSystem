using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Employees : IIdentifiableEntity
    {
        public int EmpId { get; set; }

        [Required(ErrorMessage = "Please enter Employee name!")]
        public string Name { get; set; }

       

        [Required(ErrorMessage = "Please fill phonenumber!")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please fill email!")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        public decimal? Salary { get; set; }
        public bool IsContract { get; set; }


        
       
         [DisplayName("Upload Photo")]
        public string Photo { get; set; }

        [Required]
        public string Department { get; set; }


       // [RegularExpression(@"([a-zA-Z])+(.csv|.pdf|.xls|.xlsx|.doc)$", ErrorMessage = "Only Pdf or Csv files allowed.")]
        [DisplayName("Upload Resume")]
        public string Resume { get; set; }
        public string Address { get; set; }
        public string MaritalStatus { get; set; }
        public string Sex { get; set; }
        public DateTime? StartDate { get; set; }
        public string Education { get; set; }
        public string NRC { get; set; }
        public string Manager { get; set; }
        public DateTime? Birthday { get; set; }
        public string SpousePh { get; set; }
        public string SpouseName { get; set; }
        public string Position { get; set; }
        public string ContactName { get; set; }
        public string ContactPh { get; set; }
       
        public string Bankacc { get; set; }

        public string Role { get; set; }
        public int EntityId
        {
            get { return EmpId; }
            set { EmpId = value; }
        }
    }
}