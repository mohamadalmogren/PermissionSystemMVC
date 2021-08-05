using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class UserListViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        [Display(Name ="Department")]
        public string Departmentname { get; set; }
        public string Roles { get; set; }
    }
}
