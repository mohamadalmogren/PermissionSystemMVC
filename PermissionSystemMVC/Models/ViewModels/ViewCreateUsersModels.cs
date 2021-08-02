using System;
using System.ComponentModel.DataAnnotations;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class ViewCreateUsersModels
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
    }
}
