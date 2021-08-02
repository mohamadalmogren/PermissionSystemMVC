using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class UserListViewModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
    }
}
