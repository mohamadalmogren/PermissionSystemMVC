using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionSystemMVC.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        public Department Department { get; set; }

        [Display(Name ="Department")]
        public int DepartmentId { get; set; }
    }
}
