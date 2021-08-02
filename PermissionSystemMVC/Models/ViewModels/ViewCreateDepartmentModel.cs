using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class ViewCreateDepartmentModel
    {
        public int Id { get; set; }

        [Display(Name="Department Name")]
        public String Name { get; set; }
    }
}
