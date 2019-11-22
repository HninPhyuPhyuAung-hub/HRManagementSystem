using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class FullName
    {
        [Required]
        [MaxLength(50)]
        [MinLength(1)]
        [Column("Name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Column("MiddleName")]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(1)]
        [Column("LastName")]
        public string LastName { get; set; }

    }
}