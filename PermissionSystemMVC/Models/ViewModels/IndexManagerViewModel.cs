using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class IndexManagerViewModel
    {
        public string Name { get; set; }
        public string Requests { get; set; }
        public string Hours { get; set; }
        public string Approved { get; set; }
        public string Rejected { get; set; }
        public string New { get; set; }

    }
}
