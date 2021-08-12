using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionSystemMVC.Models.ViewModels
{
    public class ListRequestViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Employee")]
        public string CreatedBy { get; set; }


        [Display(Name = "Prmisssion Type")]
        public string PrmisssionType { get; set; }

        [Display(Name = "promission Date")]
        public string DatePrmission { get; set; }

        [Display(Name = "From Time")]
        public string FromTime { get; set; }

        [Display(Name = "To Time")]
        public string ToTime { get; set; }

        [Display(Name = "Created Date")]
        public string CreateDate { get; set; }
        public string Status { get; set; }

    }
}
