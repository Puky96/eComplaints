using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eComplaints.Models
{
    public class LDAPAuthentication
    {
        [Required(ErrorMessage ="Introduceti un nume de utilizator")]
        [Display(Name ="Username: ")]
        public string UserName { get; set; }
        

        [Required(ErrorMessage ="Introduceti o parola")]
        [Display(Name ="Parola: ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
