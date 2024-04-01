using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeManagementSystem.Models
{
    public class Membership
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Username")]
        [Required(ErrorMessage ="*mandatory")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "*mandatory")]
        public string Password { get; set; }
    }
}