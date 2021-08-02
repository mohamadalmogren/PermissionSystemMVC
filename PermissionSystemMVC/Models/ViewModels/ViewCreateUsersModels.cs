using System;
using System.ComponentModel.DataAnnotations;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class ViewCreateUsersModels
    {
        [Display(Name ="UserName")]
        public String UserName { get; set; }
        [Display(Name ="Email")]
        public string Email { get; set; }
        public string Password { get; set; }

        [Display(Name ="Full Name")]
        public String Name { get; set; }
        public string Role { get; set; }
        public Department Department { get; set; }
        [Display(Name = "Department")]

        public int DepartmentId { get; set; }
    }
}
