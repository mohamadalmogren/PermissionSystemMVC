using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public ICollection<string> Roles { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

    }
}
